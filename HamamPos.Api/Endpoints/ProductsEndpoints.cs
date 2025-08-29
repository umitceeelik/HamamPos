// Amaç: /products uçlarını Program.cs'ten ayırmak.
// Şimdilik sadece aktif ürünleri listeliyoruz.

using HamamPos.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HamamPos.Api.Endpoints;

public static class ProductsEndpoints
{
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        // İstersen RequireAuthorization() ekleyebilirsin:
        // var grp = app.MapGroup("/products").RequireAuthorization();
        var grp = app.MapGroup("/products");

        // GET /products  -> POS ürün listesi
        grp.MapGet("/", async (AppDbContext db) =>
            await db.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToListAsync());

        return app;
    }
}