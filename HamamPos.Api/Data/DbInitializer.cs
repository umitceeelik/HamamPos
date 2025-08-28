// Amaç: Migration + Seed işlemlerini Program.cs'ten ayırmak.
// Program.cs -> await DbInitializer.InitializeAsync(app.Services);

using HamamPos.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HamamPos.Api.Data;

public static class DbInitializer
{
    /// <summary>
    /// DB migrate + ilk seed (kullanıcı, birimler, ürünler).
    /// Production'da seed mantığı farklı olabilir; şimdilik dev/prototip için uygun.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Pending migration varsa uygula
        await db.Database.MigrateAsync();

        // Kullanıcılar
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"), Role = Role.Admin },
                new User { Username = "personel", PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"), Role = Role.User }
            );
        }

        // Birimler (24 oda + 40 dolap)
        if (!db.ServiceUnits.Any())
        {
            for (int i = 1; i <= 24; i++)
                db.ServiceUnits.Add(new ServiceUnit { Name = $"Oda {i}", Type = UnitType.Room });

            for (int i = 1; i <= 40; i++)
                db.ServiceUnits.Add(new ServiceUnit { Name = $"Dolap {i}", Type = UnitType.Locker });
        }

        // Ürünler (örnek)
        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Name = "Hamam", Category = "Hizmet", BasePrice = 400, VatRate = 10 },
                new Product { Name = "Kese", Category = "Hizmet", BasePrice = 150, VatRate = 10 },
                new Product { Name = "Masaj", Category = "Hizmet", BasePrice = 450, VatRate = 10 },
                new Product { Name = "Su", Category = "İçecek", BasePrice = 15, VatRate = 10 },
                new Product { Name = "Çay", Category = "İçecek", BasePrice = 10, VatRate = 10 }
            );
        }

        await db.SaveChangesAsync();
    }
}
