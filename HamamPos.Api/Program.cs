using HamamPos.Api;
using HamamPos.Api.Data;

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

app.Run();
