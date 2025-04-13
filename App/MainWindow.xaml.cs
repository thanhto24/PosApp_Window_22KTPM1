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
using Microsoft.UI.Windowing;
using WinUIEx;
using WinRT.Interop;
using System.Runtime.InteropServices;

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
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MAXIMIZE = 3;


        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Coffee App";

            // Maximize cửa sổ khi khởi động
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            ShowWindow(hWnd, SW_MAXIMIZE);

            // Copy ảnh
            CopyFolderImager.CopyProductImagesFolder();

            // Điều hướng đến Dashboard
            MainFrame.Navigate(typeof(Dashboard));
        }

    }
}
