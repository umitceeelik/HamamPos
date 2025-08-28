// Amaç: Program.cs içindeki "middleware hattını" ve "endpoint map'lerini" modülerleştirmek.
// UseAppPipeline(): middleware sırası
// MapAppEndpoints(): Minimal API endpoint map'leri

using HamamPos.Api.Data;
using HamamPos.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace HamamPos.Api;

public static class WebAppExtensions
{
    /// <summary>
    /// Uygulama pipeline'ını kur: Swagger (dev), Auth, Authorization, (ileride) hata yakalama middleware'i vb.
    /// Program.cs -> app.UseAppPipeline();
    /// </summary>
    public static WebApplication UseAppPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Tüm Minimal API endpoint grupları tek yerden map'lenir.
    /// Program.cs -> app.MapAppEndpoints();
    /// </summary>
    public static IEndpointRouteBuilder MapAppEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => "ok");

        // LOGIN ÖNCE GELMELİ (Anonymous)
        app.MapAuthEndpoints();

        // Basit liste uçları (anonim kalabilir veya RequireAuthorization da yapabiliriz)
        app.MapGet("/units", async (AppDbContext db) =>
            await db.ServiceUnits.OrderBy(u => u.Type).ThenBy(u => u.Id).ToListAsync());

        app.MapGet("/products", async (AppDbContext db) =>
            await db.Products.Where(p => p.IsActive).OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync());

        // Tickets grubu (genelde auth gerektirir; TicketEndpoints.cs’de RequireAuthorization ekleyebilirsin)
        app.MapTicketEndpoints();

        return app;
    }
}
