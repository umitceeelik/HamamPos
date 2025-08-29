using System.Windows.Controls;
using HamamPos.Desktop.ViewModels;

namespace HamamPos.Desktop.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView() => InitializeComponent();

        // PasswordBox binding'i için
        private void Pwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
                vm.Password = pb.Password ?? string.Empty;
        }
    }
}
