// "Gün Başlat / Gün Sonu" mantığı için günlük oturum kaydı.
// Basit: tek departman/sistem varsayımı. İleride çoklu departman eklenebilir.
namespace HamamPos.Shared.Models;

public class DaySession
{
    public int Id { get; set; }

    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAtUtc { get; set; }

    // Açan/Kapayan kullanıcılar
    public string OpenedBy { get; set; } = default!;
    public string? ClosedBy { get; set; }

    public bool IsOpen => !ClosedAtUtc.HasValue;
}
