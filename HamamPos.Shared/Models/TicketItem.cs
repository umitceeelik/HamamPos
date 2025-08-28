// Adisyondaki tek satır. (Ürün + miktar + birim fiyat + opsiyonel indirim/fiyat değişikliği)
namespace HamamPos.Shared.Models;

public class TicketItem
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public int ProductId { get; set; }

    // Eklendiği andaki “görünen ürün adı”. Ürün adı değişse bile adisyonda sabit kalsın.
    public string ProductName { get; set; } = default!;

    // Miktar (numaratörden girilebilecek)
    public decimal Quantity { get; set; } = 1;

    // Satır birim fiyatı (BasePrice’dan farklı olabilir; “Fiyat Değiştir” ile override)
    public decimal UnitPrice { get; set; }

    // Satır toplam = Quantity * UnitPrice
    public decimal TotalPrice => Math.Round(Quantity * UnitPrice, 2);

    // İleride: Özellik/ekstra not/çalışan adı gibi metadata ekleyebiliriz (örn: Keseci)
    public string? Note { get; set; }
}
