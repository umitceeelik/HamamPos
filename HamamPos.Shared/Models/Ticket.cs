// Adisyon (bir odaya/dolaba açılmış sipariş/işlem). Gün başı-gün sonu arasında toplanır.
using System.ComponentModel.DataAnnotations.Schema;

namespace HamamPos.Shared.Models;

public class Ticket
{
    public int Id { get; set; }

    // Hangi birime (oda/dolap) ait
    public int ServiceUnitId { get; set; }

    // Açan kullanıcı (raporlama ve iz için)
    public string OpenedBy { get; set; } = default!;

    // Açılış/Kapanış zamanları
    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAtUtc { get; set; }

    // Kalemler (ürünler)
    public List<TicketItem> Items { get; set; } = new();

    // Ödemeler (nakit/kart)
    public List<Payment> Payments { get; set; } = new();

    // Adisyon kapandı mı?
    public bool IsClosed => ClosedAtUtc.HasValue;

    // Hesaplanan toplam (DB’de kolon tutmuyoruz; okurken hesaplanır)
    [NotMapped]
    public decimal Total => Items.Sum(i => i.TotalPrice);

    // Kalan bakiye (ödemeler düşülmüş)
    [NotMapped]
    public decimal Balance => Total - Payments.Sum(p => p.Amount);
}
