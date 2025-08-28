// Kullanıcı rolü: yetki kontrolünde kullanacağız (Admin => tüm menüler, User => sadece POS)
namespace HamamPos.Shared.Models;

public enum Role
{
    Admin = 1,
    User = 2
}
