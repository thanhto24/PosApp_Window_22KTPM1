using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Microsoft.UI;
using Microsoft.UI.Xaml;


namespace App.View.Pages
{
    public sealed partial class Management : Page
    {
        public ObservableCollection<MenuItem> ItemsInOut { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> ItemsAccess { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> ItemsCheck { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> ItemsOther { get; set; } = new ObservableCollection<MenuItem>();

        public Management()
        {
            this.InitializeComponent();

            ItemsInOut = new ObservableCollection<MenuItem>
            {
                new MenuItem("Phiếu mua hàng", "Quản lý mua hàng"),
                new MenuItem("Nhập hàng", "Quản lý các phiếu nhập hàng"),
                new MenuItem("Xuất hàng", "Quản lý các phiếu xuất hàng"),
                new MenuItem("Loại Nhập - Xuất hàng", "Loại Nhập - Xuất hàng"),
            };

            ItemsAccess = new ObservableCollection<MenuItem>
            {
                new MenuItem("Truy xuất hàng hóa", "Truy xuất hàng hóa"),
                new MenuItem("Tồn kho", "Quản lý thông tin tồn kho"),
            };

            ItemsCheck = new ObservableCollection<MenuItem>
            {
                new MenuItem("Quản lý phiếu kiểm kê", "Quản lý thông tin kiểm kho"),
                new MenuItem("Thêm phiếu kiểm kê", "Thêm phiếu kiểm kê"),
            };

            ItemsOther = new ObservableCollection<MenuItem>
            {
                new MenuItem("Sản xuất", "Sản xuất"),
                new MenuItem("Danh sách kho", "Quản lý danh sách kho"),
            };
            this.DataContext = this;
        }

        private void BorderContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Colors.LightBlue);
            }
        }

        private void BorderContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MenuItem menuItem)
            {
                if (menuItem.Title == "Tồn kho")
                {
                    this.Frame.Navigate(typeof(PurchaseOrderManagement));
                }
            }
        }


    }

    public class MenuItem
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public MenuItem(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
