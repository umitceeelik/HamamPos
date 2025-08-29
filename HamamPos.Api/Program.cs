using HamamPos.Api;
using HamamPos.Api.Data;
using HamamPos.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Servisler (Db, Auth, Swagger) ---
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

// LAN'dan eriþim için
app.Urls.Add("http://0.0.0.0:5005");

// --- DB migrate + seed ---
await DbInitializer.InitializeAsync(app.Services);

// --- Middleware pipeline ---
app.UseAppPipeline();

// --- Endpoint map'leri ---
app.MapAppEndpoints();

// Açýk adisyonlar (yalnýzca top-level ifade!)
app.MapGet("/tickets/open", async (AppDbContext db) =>
    await db.Tickets
            .Where(t => t.ClosedAtUtc == null)
            .Select(t => new OpenTicketDto(t.Id, t.ServiceUnitId))
            .ToListAsync());


app.Run();
