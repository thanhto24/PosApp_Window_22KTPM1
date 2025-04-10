using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.View.ViewModel;

namespace App.View.Pages
{
    public sealed partial class BestSellingProductsPage : Page
    {
        public PieChartViewModel ViewModel { get; set; }

        public BestSellingProductsPage()
        {
            this.InitializeComponent();
            ViewModel = new PieChartViewModel();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}