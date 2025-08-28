// Amaç: Program.cs içindeki "servis kurulumlarını" (Db, Auth, Swagger, vb.) tek yere toplamak.
// Böylece Program.cs yalın ve okunabilir kalır.

using HamamPos.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HamamPos.Api;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Tüm DI servis kayıtlarını (DB, Auth, Swagger, CORS vb.) tek noktadan yapar.
    /// Program.cs -> builder.Services.AddAppServices(builder.Configuration)
    /// </summary>
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration cfg)
    {
        // --- Database (Sqlite) ---
        // appsettings.json -> "ConnectionStrings:Default"
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(cfg.GetConnectionString("Default")));

        // --- JWT Authentication ---
        // appsettings.json veya UserSecrets:
        // "Jwt:Key", "Jwt:Issuer", "Jwt:Audience"
        var key = cfg["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = cfg["Jwt:Issuer"] ?? "HamamPos";
        var audience = cfg["Jwt:Audience"] ?? "HamamPosClients";

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    // HS256 için en az 256-bit (32+ char) secret zorunlu!
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

        services.AddAuthorization();

        // --- Swagger (sadece dev'de arayüz göstereceğiz) ---
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
