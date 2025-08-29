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

        // Pos listeleri (şimdilik anonim; istersen RequireAuthorization ekleyebilirsin)
        app.MapUnitsEndpoints();
        app.MapProductsEndpoints();

        // Tickets grubu (genelde auth gerektirir; TicketEndpoints.cs’de RequireAuthorization ekleyebilirsin)
        app.MapTicketEndpoints();

        return app;
    }
}
