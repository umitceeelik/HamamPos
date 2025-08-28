// Satılan/işlenen ürün/hizmet. (Hamam, Kese, Çay vb.)
namespace HamamPos.Shared.Models;

public class Product
{
    public int Id { get; set; }

    // Ekrandaki buton/rapor isimleri
    public string Name { get; set; } = default!;

    // Menüde kategoriye göre gruplayacağız (HAMAM, MASAJ, SOĞUK İÇEÇEK vb.)
    public string Category { get; set; } = "GENEL";

    // Temel fiyat (porsiyon ya da özel fiyat yoksa bu kullanılır)
    public decimal BasePrice { get; set; }

    // KDV oranı (raporlar için şimdiden uygun)
    public decimal VatRate { get; set; } = 0;

    // Pasife alınan ürün POS’ta görünmesin
    public bool IsActive { get; set; } = true;

    // İleride: Porsiyonlar ve Özellik Grupları (opsiyonel ilişkiler) ekleyebiliriz.
}
