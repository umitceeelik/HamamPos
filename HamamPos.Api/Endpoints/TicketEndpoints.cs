using HamamPos.Api.Data;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HamamPos.Api.Endpoints;

public static class TicketEndpoints
{
    public static IEndpointRouteBuilder MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/tickets"); // istersen .RequireAuthorization() ekleyebilirsin

        grp.MapPost("/open", OpenOrGet);
        grp.MapPost("/{ticketId:int}/items", AddItem);
        grp.MapPost("/{ticketId:int}/pay", Pay);
        grp.MapGet("/{ticketId:int}", GetTicket);

        return app;
    }

    // ---- Handlers ----

    private static async Task<IResult> OpenOrGet(AppDbContext db, OpenTicketRequest req, HttpContext http)
    {
        var existing = await db.Tickets
            .Include(t => t.Items).Include(t => t.Payments)
            .FirstOrDefaultAsync(t => t.ServiceUnitId == req.ServiceUnitId && t.ClosedAtUtc == null);

        if (existing is null)
        {
            var openedBy = http.User?.Identity?.Name ?? "system";
            existing = new Ticket
            {
                ServiceUnitId = req.ServiceUnitId,
                OpenedBy = openedBy,
                OpenedAtUtc = DateTime.UtcNow
            };
            db.Tickets.Add(existing);
            await db.SaveChangesAsync();
            await db.Entry(existing).Collection(x => x.Items).LoadAsync();
            await db.Entry(existing).Collection(x => x.Payments).LoadAsync();
        }

        return Results.Ok(ToDto(existing));
    }

    private static async Task<IResult> AddItem(AppDbContext db, int ticketId, AddItemRequest req)
    {
        var id = ticketId > 0 ? ticketId : req.TicketId;

        var t = await db.Tickets.Include(x => x.Items).Include(x => x.Payments)
                                .FirstOrDefaultAsync(x => x.Id == id && x.ClosedAtUtc == null);
        if (t is null) return Results.BadRequest("Adisyon bulunamadı / kapalı.");

        var p = await db.Products.FirstOrDefaultAsync(x => x.Id == req.ProductId && x.IsActive);
        if (p is null) return Results.BadRequest("Ürün bulunamadı / pasif.");

        t.Items.Add(new TicketItem
        {
            ProductId = p.Id,
            ProductName = p.Name,
            Quantity = req.Quantity,
            UnitPrice = p.BasePrice
        });

        await db.SaveChangesAsync();

        await db.Entry(t).Collection(x => x.Items).LoadAsync();
        await db.Entry(t).Collection(x => x.Payments).LoadAsync();

        return Results.Ok(ToDto(t));
    }

    private static async Task<IResult> Pay(AppDbContext db, int ticketId, PayTicketRequest req, HttpContext http)
    {
        var id = ticketId > 0 ? ticketId : req.TicketId;

        var t = await db.Tickets.Include(x => x.Items).Include(x => x.Payments)
                                .FirstOrDefaultAsync(x => x.Id == id && x.ClosedAtUtc == null);
        if (t is null) return Results.BadRequest("Adisyon bulunamadı / kapalı.");

        var total = t.Items.Sum(i => i.TotalPrice);
        var paid = t.Payments.Sum(p => p.Amount);
        var bal = Math.Round(total - paid, 2);
        if (bal <= 0) return Results.Ok(ToDto(t)); // zaten kapalı gibi

        var type = req.Method == PayMethod.Card ? PaymentType.Card : PaymentType.Cash;

        t.Payments.Add(new Payment
        {
            TicketId = t.Id,
            Amount = bal, // şimdilik tamamını kapat
            Type = type,
            CollectedBy = http.User?.Identity?.Name ?? "system",
            CollectedAtUtc = DateTime.UtcNow
        });

        // bakiye 0 olduysa kapat
        await db.SaveChangesAsync();

        total = t.Items.Sum(i => i.TotalPrice);
        paid = t.Payments.Sum(p => p.Amount);
        bal = Math.Round(total - paid, 2);
        if (bal <= 0 && t.ClosedAtUtc is null)
        {
            t.ClosedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        await db.Entry(t).Collection(x => x.Items).LoadAsync();
        await db.Entry(t).Collection(x => x.Payments).LoadAsync();

        return Results.Ok(ToDto(t));
    }

    private static async Task<IResult> GetTicket(AppDbContext db, int ticketId)
    {
        var t = await db.Tickets.Include(x => x.Items).Include(x => x.Payments)
                                .FirstOrDefaultAsync(x => x.Id == ticketId);
        return t is null ? Results.NotFound() : Results.Ok(ToDto(t));
    }

    // ---- helper ----
    private static TicketDto ToDto(Ticket t)
    {
        var items = t.Items
            .OrderBy(i => i.Id)
            .Select(i => new TicketItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
            .ToList();

        var pays = t.Payments
            .OrderBy(p => p.CollectedAtUtc)
            .Select(p => new PaymentDto(p.Type.ToString(), p.Amount, p.CollectedAtUtc))
            .ToList();

        var total = items.Sum(i => i.LineTotal);
        var paid = pays.Sum(p => p.Amount);
        var bal = Math.Round(total - paid, 2);

        return new TicketDto(t.Id, t.ServiceUnitId, t.OpenedBy, t.OpenedAtUtc, t.ClosedAtUtc,
                             total, paid, bal, t.ClosedAtUtc is not null, items, pays);
    }
}
