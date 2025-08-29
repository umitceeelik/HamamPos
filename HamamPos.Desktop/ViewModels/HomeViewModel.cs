using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HamamPos.Desktop.Navigation;
using HamamPos.Desktop.Services;
using HamamPos.Desktop.State;

namespace HamamPos.Desktop.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly NavigationStore _store;
    private readonly INavigationService _nav;
    private readonly ApiClient _api;
    private readonly SessionState _session;

    public HomeViewModel(NavigationStore store, INavigationService nav,
                         ApiClient api, SessionState session)
    { _store = store; _nav = nav; _api = api; _session = session; }

    public Task LoadAsync() => Task.CompletedTask;

    [RelayCommand]
    private void GoPos()
    {
        var posVm = new PosViewModel(_api, _session, _store, _nav);
        _nav.Navigate(posVm);
        _ = posVm.LoadAsync();
    }

    [RelayCommand] private void GoAdmin() { /* _nav.Navigate(new AdminViewModel(...)) */ }
    [RelayCommand] private void GoReports() { /* _nav.Navigate(new ReportsViewModel(...)) */ }

    [RelayCommand]
    private void GoHome()
    {
        var home = new HomeViewModel(_store, _nav, _api, _session);
        _nav.Navigate(home);
    }
}
