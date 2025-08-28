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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _session = new SessionState();
        _api = new ApiClient(_session, baseUrl: "http://localhost:5005"); // gerekirse IP değiştir

        var vm = new LoginViewModel(_api, _session);
        var win = new LoginWindow { DataContext = vm };

        vm.OnLoggedIn = async () =>
        {
            // Login'i kapat, ana pencereyi aç
            win.Dispatcher.Invoke(() => win.Close());
            await OpenMainAsync();
        };

        win.Show();
    }

    private Task OpenMainAsync()
    {
        var main = new MainWindow();
        // Basit demo context: "kim girdi / rolü ne"
        main.DataContext = new { User = _session.Username, Role = _session.Role };
        main.Show();
        return Task.CompletedTask;
    }
}
