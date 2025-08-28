// LoginWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using HamamPos.Desktop.ViewModels;

namespace HamamPos.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password ?? string.Empty; // non-nullable Password ile uyumlu
            }
        }
    }
}
