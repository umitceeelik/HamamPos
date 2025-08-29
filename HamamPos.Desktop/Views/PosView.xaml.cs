// PosView.xaml.cs
using System.Windows.Controls;

namespace HamamPos.Desktop.Views;

public partial class PosView : UserControl
{
    private bool _loadedOnce;

    public PosView()
    {
        InitializeComponent();
        Loaded += PosView_Loaded;
        Unloaded += PosView_Unloaded;
    }

    private async void PosView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_loadedOnce) return;
        _loadedOnce = true;

        // İsterseniz burada VM.LoadAsync tetikleyebilirsiniz
        if (DataContext is ViewModels.PosViewModel vm)
        {
            if (vm.Units.Count == 0 || vm.Products.Count == 0)
                await vm.LoadAsync();
        }
    }

    private void PosView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        // event/abonelik temizliği gerekiyorsa
    }
}
