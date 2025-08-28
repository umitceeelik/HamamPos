// Sistem kullanıcıları (login olacak kişiler). Şifre plaintext tutulmaz; hash saklarız.
namespace HamamPos.Shared.Models;

public class User
{
    public int Id { get; set; }

    // Ekranda ve token'da kullanacağız, benzersiz olmalı
    public string Username { get; set; } = default!;

    // BCrypt ile hash'lenmiş şifre (asla düz metin değil)
    public string PasswordHash { get; set; } = default!;

    // Rol bazlı yetki
    public Role Role { get; set; } = Role.User;

    // Kullanıcıyı pasife alıp girişini engellemek için
    public bool IsActive { get; set; } = true;
}
