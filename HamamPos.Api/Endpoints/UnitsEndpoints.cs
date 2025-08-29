// Amaç: /units uçlarını Program.cs'ten ayırmak.
// Şimdilik sadece listeleme var (GET /units). İleride CRUD genişlerse aynı gruba ekleriz.

using HamamPos.Api.Data;
using HamamPos.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HamamPos.Api.Endpoints;

public static class UnitsEndpoints
{
    public static IEndpointRouteBuilder MapUnitsEndpoints(this IEndpointRouteBuilder app)
    {
        // İstersen RequireAuthorization() ekleyebilirsin:
        // var grp = app.MapGroup("/units").RequireAuthorization();
        var grp = app.MapGroup("/units");

        // GET /units  -> POS oda/dolap listesi
        grp.MapGet("/", async (AppDbContext db) =>
            await db.ServiceUnits
                    .OrderBy(u => u.Type)
                    .ThenBy(u => u.Id)
                    .ToListAsync());

        // GET /units/with-status -> dolu/boş bilgisi
        grp.MapGet("/with-status", async (AppDbContext db) =>
        {
            // Kapalı olmayan adisyonu olan birimler dolu kabul edilir
            var occupied = await db.Tickets
                                   .Where(t => t.ClosedAtUtc == null)
                                   .Select(t => t.ServiceUnitId)
                                   .Distinct()
                                   .ToListAsync();

            return Results.Ok(
                occupied.Select(id => new UnitStatusDto(id, true))
                        .ToList()
            );
        });

        return app;
    }
}
