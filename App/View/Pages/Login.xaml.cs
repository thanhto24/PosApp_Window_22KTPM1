using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using App.View.Pages;
namespace App.View.Pages
{
    /// <summary>
    /// Trang đăng nhập.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (username == "admin" && password == "123")
            {
                // Điều hướng sang trang Home.xaml
                this.Frame.Navigate(typeof(Dashboard));
            }
            else
            {
                // Hiển thị thông báo lỗi
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Sai tên đăng nhập hoặc mật khẩu!",
                    CloseButtonText = "Thử lại",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
            }
        }
    }
}
