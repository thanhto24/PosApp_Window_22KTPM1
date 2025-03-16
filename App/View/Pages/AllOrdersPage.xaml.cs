using App.View.ViewModel; // Import ViewModel
using App.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace App.View.Pages
{
    public sealed partial class AllOrdersPage : Page
    {
        public OrderViewModel OrderViewModel { get; set; }

        public AllOrdersPage()
        {
            this.InitializeComponent();
            OrderViewModel = new OrderViewModel(); // Kh?i t?o ViewModel
        }
    }
}
