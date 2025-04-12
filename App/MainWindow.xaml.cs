using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App.View.Pages;
using Microsoft.Data.Sqlite;
using App.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainWindow : Window
    {
        //private static readonly string DatabasePath = Path.Combine(AppContext.BaseDirectory, "database.db");

        public MainWindow()
        {
            this.InitializeComponent();

            //InitializeDatabase();

            //update ProductImages folder trong bin -> cải tiến UX
            CopyFolderImager.CopyProductImagesFolder();

            // Điều hướng đến Login.xaml khi ứng dụng khởi động
            MainFrame.Navigate(typeof(Login));
        }

    }
}
