﻿using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using App.View.Pages;
using Microsoft.Data.Sqlite;
using System.IO;
namespace App.View.Pages
{
    /// <summary>
    /// Trang đăng nhập.
    /// </summary>
    public sealed partial class Login : Page
    {
        private string DatabasePath = Path.Combine(AppContext.BaseDirectory, "database.db");

        public Login()
        {
            this.InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (checkLogin(username, password))
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
                    CloseButtonText = "Đóng",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
            }
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            AddUser(username, password);
            ContentDialog SignupDialog = new ContentDialog
            {
                Title = "Sign Up",
                Content = "Đăng ký thành công",
                CloseButtonText = "Đóng",
                XamlRoot = this.XamlRoot
            };
            _ = SignupDialog.ShowAsync();
        }

        private void AddUser(string username, string password)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password); // Nên mã hóa mật khẩu trước khi lưu
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

        private bool checkLogin(string username, string password)
        {
            if (username == null || password == null)
                return false;
            else if (username == "admin" && password == "123")
                return true;


            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                long count = (long)command.ExecuteScalar();
                connection.Close();

                return count > 0;
            }
        }
    }
}
