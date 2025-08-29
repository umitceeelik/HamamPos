// Desktop/App.xaml.cs
using System.Windows;
using HamamPos.Desktop.Navigation;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;
using HamamPos.Desktop.ViewModels;

namespace HamamPos.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Tekil servisler
        var session = new SessionState();
        var api = new ApiClient(session, "http://localhost:5005");

        // Navigation altyapısı
        var store = new NavigationStore();
        var nav = new NavigationService(store);

        // Shell
        var shellVm = new ShellViewModel(store, nav);
        var shell = new MainWindow { DataContext = shellVm };
        shell.Show();

        // 1) Login ile başla
        var loginVm = new LoginViewModel(api, session);
        store.CurrentViewModel = loginVm;

        // Giriş başarılı olduğunda Ana Menüyü yükle
        loginVm.OnLoggedIn = async () =>
        {
            var homeVm = new HomeViewModel(store, nav, api, session);
            nav.Navigate(homeVm);
            await homeVm.LoadAsync(); // gerekiyorsa
        };
    }
}
