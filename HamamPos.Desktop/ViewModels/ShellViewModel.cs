// Desktop/ViewModels/ShellViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HamamPos.Desktop.Navigation;

namespace HamamPos.Desktop.ViewModels;

/// <summary>Tek pencerenin DataContext’i; Store’u expose eder.</summary>
public partial class ShellViewModel : ObservableObject
{
    public NavigationStore Store { get; }
    private readonly INavigationService _nav;

    public ShellViewModel(NavigationStore store, INavigationService nav)
    {
        Store = store;
        _nav = nav;
    }

    // Ana Menüye dön
    [RelayCommand] public void GoHome(object vm) => _nav.Navigate(vm);
}
