// MVVM: View sadece XAML; mantık bu VM'de.
// - Username/Password, IsBusy, ErrorMessage: UI'ya bind edilir.
// - LoginCommand: butona basınca çalışır, API'ye gider.
// - OnLoggedIn: Başarılı girişte UI geçişini (pencere aç/kapat) dışarıya bırakır.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;
using System.Threading.Tasks;

namespace HamamPos.Desktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiClient _api;
    private readonly SessionState _session;

    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? errorMessage;

    // App.xaml.cs tarafında set edilecek: başarılı girişte nereye gideceğimizi o belirler
    public Func<Task>? OnLoggedIn { get; set; }

    public LoginViewModel(ApiClient api, SessionState session)
    {
        _api = api;
        _session = session;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Kullanıcı adı ve şifre gerekli.";
            return;
        }

        try
        {
            IsBusy = true;

            var result = await _api.LoginAsync(Username, Password);
            if (result is null)
            {
                ErrorMessage = "Giriş başarısız. Bilgileri kontrol edin.";
                return;
            }

            // Başarılı ise kontrolü dışarı ver (pencere geçişi)
            if (OnLoggedIn is not null)
                await OnLoggedIn();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Bağlantı hatası: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
