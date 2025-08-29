namespace HamamPos.Desktop.Navigation;

public class NavigationService : INavigationService
{
    private readonly NavigationStore _store;
    public NavigationService(NavigationStore store) => _store = store;

    public void Navigate(object viewModel)
    {
        _store.CurrentViewModel = viewModel;
    }
}