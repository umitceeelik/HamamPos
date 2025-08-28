// Burada "composition root" gibi davranıyoruz:
// - Tekil SessionState ve ApiClient örnekleri oluşturulur.
// - Uygulama LoginWindow ile başlar.
// - Başarılı login sonrasında MainWindow açılır.
// - İleride: Role'e göre yönlendirme (Admin -> Ana Menü, User -> direkt POS)

using System.Threading.Tasks;
using System.Windows;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;
using HamamPos.Desktop.ViewModels;
using HamamPos.Desktop.Views;

namespace HamamPos.Desktop;

public partial class App : Application
{
    private SessionState _session = null!;
    private ApiClient _api = null!;

    public SessionState Session => _session;
    public ApiClient Api => _api;

    // App.xaml.cs
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _session = new SessionState();
        _api = new ApiClient(_session, baseUrl: "http://localhost:5005");

        var loginVm = new LoginViewModel(_api, _session);
        var loginWin = new LoginWindow { DataContext = loginVm };

        // Uygulama, son pencere kapanınca kapanmasın; geçişi biz kontrol edelim
        Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        loginVm.OnLoggedIn = async () =>
        {
            await loginWin.Dispatcher.InvokeAsync(() =>
            {
                // --- Main'i oluştur ve göster
                var mainVm = new MainViewModel(_api, _session);
                var main = new MainWindow { DataContext = mainVm };

                // Uygulamanın ana penceresi artık bu
                Current.MainWindow = main;
                main.Show();

                // Login'i şimdi kapat
                loginWin.Close();

                // Verileri asenkron yükle (fire-and-forget)
                _ = mainVm.LoadAsync();

                // Artık normal davranışa dönebiliriz
                Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
            });
        };

        loginWin.Show();
    }


    private Task OpenMainAsync()
    {
        // MainViewModel'i ver ve Load() çağır
        var mainVm = new MainViewModel(_api, _session);
        var main = new MainWindow { DataContext = mainVm };
        main.Show();
        _ = mainVm.LoadAsync(); // fire-and-forget, ekranda yükleniyor yazısı görünecek
        return Task.CompletedTask;
    }

}
