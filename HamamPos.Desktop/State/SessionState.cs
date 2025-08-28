// Uygulama çalışırken oturum bilgisini tek yerde tutar.
// UI ve servisler buradan "kullanıcı giriş yaptı mı?", "rol ne?" gibi bilgilere bakar.

namespace HamamPos.Desktop.State;

public class SessionState
{
    // API'den alınan JWT token (korumalı: dışarıdan set edilemez)
    public string? Token { get; private set; }

    // UI'da göstermek/rol bazlı yetkide kullanmak için basit alanlar
    public string? Username { get; private set; }
    public string? Role { get; private set; }

    // Hızlı kontrol: giriş yapılmış mı?
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    // Login sonrası ViewModel/Service buradan set eder
    public void Set(string username, string role, string token)
    {
        Username = username;
        Role = role;
        Token = token;
    }

    // Logout gibi senaryolarda çağrılır
    public void Clear()
    {
        Username = Role = Token = null;
    }
}
