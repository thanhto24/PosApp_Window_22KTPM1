using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using App.Service;
using App.View.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.View.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        public Dashboard()
        {
            this.InitializeComponent();
            container.Navigate(Type.GetType($"{GetType().Namespace}.HomeScreen"));
        }

        private void navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked)
            {
                // = "Settings clicked";
                return;
            }
            var item = (NavigationViewItem)sender.SelectedItem;

            if (item.Tag != null)
            {
                string tag = (string)item.Tag;
                container.Navigate(Type.GetType($"{GetType().Namespace}.{tag}"));
            }
        }

        private void navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //if (args.SelectedItemContainer != null)
            //{
            //    var tag = args.SelectedItemContainer.Tag.ToString();
            //    NavigateToPage(tag);
            //}
        }

        //private void NavigateToPage(string tag)
        //{
        //    switch (tag)
        //    {
        //        case "CategoriesPage":
        //            container.Navigate(typeof(CategoriesPage));
        //            break;
        //        case "ProductsPage":
        //            container.Navigate(typeof(ProductsPage));
        //            break;
        //        case "OrdersPage":
        //            container.Navigate(typeof(OrdersPage));
        //            break;
        //        // Add more cases as needed
        //        default:
        //            break;
        //    }
        //}
    }
}
