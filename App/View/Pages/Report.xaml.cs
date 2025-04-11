using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Microsoft.UI;
using Microsoft.UI.Xaml;


namespace App.View.Pages
{
    public sealed partial class Report : Page
    {
        public ObservableCollection<MenuItemReport> ItemsSales { get; set; } = new ObservableCollection<MenuItemReport>();
        public ObservableCollection<MenuItemReport> ItemsCustomer { get; set; } = new ObservableCollection<MenuItemReport>();
        public ObservableCollection<MenuItemReport> ItemsInOut { get; set; } = new ObservableCollection<MenuItemReport>();
        public ObservableCollection<MenuItemReport> ItemsService { get; set; } = new ObservableCollection<MenuItemReport>();
        public ObservableCollection<MenuItemReport> ItemsOther { get; set; } = new ObservableCollection<MenuItemReport>();
        public ObservableCollection<MenuItemReport> Other { get; set; } = new ObservableCollection<MenuItemReport>();


        public Report ()
        {
            this.InitializeComponent();

            ItemsSales = new ObservableCollection<MenuItemReport>
            {
                new MenuItemReport("Tổng quan"),
                new MenuItemReport("Hóa đơn"),
                new MenuItemReport("Mặt hàng bán chạy"),
                new MenuItemReport("Khu vực bán chạy"),
                new MenuItemReport("Topping bán chạy"),
                new MenuItemReport("Báo cáo lịch sử ca"),
            };

            ItemsCustomer = new ObservableCollection<MenuItemReport>
            {
                new MenuItemReport("Doanh thu theo khách hàng"),
                new MenuItemReport("Công nợ khách hàng"),
                new MenuItemReport("Công nợ chi nhánh"),
            };

            ItemsInOut = new ObservableCollection<MenuItemReport>
            {
                new MenuItemReport("Báo cáo công nợ tổng hợp NCC"),
                new MenuItemReport("Báo cáo công nợ chi tiết NCC"),
            };

            ItemsService = new ObservableCollection<MenuItemReport>
            {
                new MenuItemReport("Nạp tiền thẻ"),
                new MenuItemReport("Số dư thẻ"),
                new MenuItemReport("Nạp tiền và sử dụng thẻ"),
                new MenuItemReport("Chi tiết số dư"),
                new MenuItemReport("Sử dụng thẻ theo khu vực"),
                new MenuItemReport("Dịch vụ canteen - Học bổng"),
                new MenuItemReport("Dịch vụ canteen - Chi tiết"),
                new MenuItemReport("HS không ăn tại canteen"),
                new MenuItemReport("Số lượng bữa ăn nợ"),
                new MenuItemReport("Chi tiết hóa đơn canteen"),

            };

            ItemsOther = new ObservableCollection<MenuItemReport>
            {
                new MenuItemReport("Báo cáo doanh thu - lợi nhuận theo SP & DV"),
                new MenuItemReport("Báo cáo khấu hao NVL theo đơn hàng"),
                new MenuItemReport("Báo cáo hạng mục chi phí"),
            };

            Other = new ObservableCollection<MenuItemReport>
            {
                new MenuItemReport("Danh sách tác vụ"),
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
            if (e.ClickedItem is MenuItemReport menuItem)
            {
                if (menuItem.Title == "Tổng quan")
                {
                    this.Frame.Navigate(typeof(OverviewReport));
                }
                else if (menuItem.Title == "Mặt hàng bán chạy")
                {
                    this.Frame.Navigate(typeof(BestSellingProductsPage));
                }
            }
        }


    }

    public class MenuItemReport
    {
        public string Title { get; set; }

        public MenuItemReport(string title)
        {
            Title = title;
        }
    }
}
