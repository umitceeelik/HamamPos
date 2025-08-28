// Ödeme türleri. Kasa ekranında filtre ve toplamlar için.
namespace HamamPos.Shared.Models;

public enum PaymentType
{
    Cash = 1,       // Nakit
    Card = 2,       // Kredi/Banka kartı
    Other = 99      // İleride: Multinet, hediye çeki vb.
}
