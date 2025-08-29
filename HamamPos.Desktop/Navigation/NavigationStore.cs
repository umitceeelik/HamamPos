using CommunityToolkit.Mvvm.ComponentModel;

namespace HamamPos.Desktop.Navigation;

/// <summary>Uygulamadaki aktif ekranı tutar.</summary>
public partial class NavigationStore : ObservableObject
{
    [ObservableProperty]
    private object? currentViewModel;
}