// Minimal API baþlatma:
// - Sqlite baðlanýr, migration/seed yapýlýr.
// - JWT auth ayarlanýr.
// - /auth/login ile token verilir.
// - /units ve /products POS ekraný için basit listeler döner.

using BCrypt.Net;
using HamamPos.Api.Data;
using HamamPos.Api.Endpoints;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using HamamPos.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.Urls.Add("http://0.0.0.0:5005"); // LAN üzerinden eriþim

// Migration + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"), Role = Role.Admin },
            new User { Username = "personel", PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"), Role = Role.User }
        );
    }

    if (!db.ServiceUnits.Any())
    {
        for (int i = 1; i <= 24; i++) db.ServiceUnits.Add(new ServiceUnit { Name = $"Oda {i}", Type = UnitType.Room });
        for (int i = 1; i <= 40; i++) db.ServiceUnits.Add(new ServiceUnit { Name = $"Dolap {i}", Type = UnitType.Locker });
    }

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Name = "Hamam", Category = "Hizmet", BasePrice = 400, VatRate = 10 },
            new Product { Name = "Kese", Category = "Hizmet", BasePrice = 150, VatRate = 10 },
            new Product { Name = "Masaj", Category = "Hizmet", BasePrice = 450, VatRate = 10 },
            new Product { Name = "Su", Category = "Ýçecek", BasePrice = 15, VatRate = 10 },
            new Product { Name = "Çay", Category = "Ýçecek", BasePrice = 10, VatRate = 10 }
        );
    }

    await db.SaveChangesAsync();
}

app.MapGet("/health", () => "ok");

// Auth: doðru kullanýcý/þifre ise token üret
app.MapPost("/auth/login", async (AppDbContext db, LoginRequest req) =>
{
    Console.WriteLine($"LOGIN TRY: {req.Username} / {req.Password}");
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username && u.IsActive);
    if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        return Results.Unauthorized();

    var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(12),
        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
    );

    return Results.Ok(new LoginResponse(user.Username, user.Role.ToString(), new JwtSecurityTokenHandler().WriteToken(token)));
});

// POS ekraný için temel listeler
app.MapGet("/units", async (AppDbContext db) =>
    await db.ServiceUnits.OrderBy(u => u.Type).ThenBy(u => u.Id).ToListAsync());

app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.Where(p => p.IsActive).OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync());

app.MapTicketEndpoints();

app.Run();