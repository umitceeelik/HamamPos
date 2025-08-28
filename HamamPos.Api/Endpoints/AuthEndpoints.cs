// Amaç: /auth/login minimal endpoint'ini tekrar kazandırmak.
// - Kullanıcıyı DB'den bulur
// - BCrypt ile şifre doğrular
// - JWT üretir (Issuer/Audience/Key appsettings veya UserSecrets)
// - Username + Role claim ekler

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HamamPos.Api.Data;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HamamPos.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        // /auth/login => anonymous (Authorize yok!)
        app.MapPost("/auth/login", Login);
        return app;
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest req,
        AppDbContext db,
        IConfiguration cfg)
    {
        // Kullanıcı aktif mi?
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username && u.IsActive);
        if (user is null) return Results.Unauthorized();

        // BCrypt password check
        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Results.Unauthorized();

        // JWT config
        var key = cfg["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = cfg["Jwt:Issuer"] ?? "HamamPos";
        var audience = cfg["Jwt:Audience"] ?? "HamamPosClients";

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        // Claims: Name + Role ekleyelim
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new LoginResponse(user.Username, user.Role.ToString(), tokenStr));
    }
}
