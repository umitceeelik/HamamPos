// Oda/Dolap gibi birimler. UI’da büyük grid üzerinde buton olarak görünecek.
namespace HamamPos.Shared.Models;

public class ServiceUnit
{
    public int Id { get; set; }

    // "Oda 1", "Dolap 5" gibi
    public string Name { get; set; } = default!;

    public UnitType Type { get; set; }

    // İleride gerekirse: Kat/konum/sıra vb. alanlar eklenebilir
    public bool IsActive { get; set; } = true;
}
