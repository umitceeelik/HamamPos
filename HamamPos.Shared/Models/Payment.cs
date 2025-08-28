// Adisyona yapılan ödemeler. Bir adisyonda birden fazla ödeme olabilir (kısmi ödeme).
namespace HamamPos.Shared.Models;

public class Payment
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public PaymentType Type { get; set; } = PaymentType.Cash;

    public decimal Amount { get; set; }

    // Ödemeyi alan kullanıcı + zaman
    public string CollectedBy { get; set; } = default!;
    public DateTime CollectedAtUtc { get; set; } = DateTime.UtcNow;
}
