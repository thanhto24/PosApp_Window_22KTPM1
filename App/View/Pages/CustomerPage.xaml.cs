using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.View.ViewModel;
using App.Model;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Documents;
using System.Collections.Generic;
using App.Utils;

namespace App.View.Pages
{
    public sealed partial class CustomerPage : Page
    {
        public CustomerViewModel CustomerViewModel { get; set; }

        public CustomerPage()
        {
            this.InitializeComponent();
            CustomerViewModel = new CustomerViewModel();
        }

        private async void OnSearchClick(object sender, RoutedEventArgs e)
        {
            string phone = SearchBox.Text;
            CustomerViewModel.findByPhone(phone);


            //await FakeAPI()
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CustomerViewModel.resetClick();
        }

    }
}
