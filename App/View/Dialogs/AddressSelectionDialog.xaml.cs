using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using static App.Service.GHN;
using App.Service;

namespace App.View.Dialogs
{
    public sealed partial class AddressSelectionDialog : ContentDialog
    {
        private readonly AddressService _addressService;

        public AddressInfo ToAddress { get; private set; }

        public AddressSelectionDialog()
        {
            this.InitializeComponent();
            _addressService = new AddressService();
            LoadProvinces();
        }

        private async void LoadProvinces()
        {
            var provinces = await _addressService.GetProvincesAsync();

            ToProvinceComboBox.ItemsSource = provinces;
        }

        private async void ToProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToProvinceComboBox.SelectedItem is Province selected)
            {
                ToDistrictComboBox.ItemsSource = await _addressService.GetDistrictsAsync(selected.id);
                ToDistrictComboBox.SelectedItem = null;
                ToWardComboBox.ItemsSource = null;
            }
        }

        private async void ToDistrictComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToDistrictComboBox.SelectedItem is District selected)
            {
                ToWardComboBox.ItemsSource = await _addressService.GetWardsAsync(selected.id);
                ToWardComboBox.SelectedItem = null;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Lưu thông tin địa chỉ nhận
            ToAddress = new AddressInfo
            {
                Province = ToProvinceComboBox.SelectedItem as Province,
                District = ToDistrictComboBox.SelectedItem as District,
                Ward = ToWardComboBox.SelectedItem as Ward,
                Detailed = ToDetailedAddressTextBox.Text
            };
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ToAddress = null;
        }

        public class AddressInfo
        {
            public Province Province { get; set; }
            public District District { get; set; }
            public Ward Ward { get; set; }
            public string Detailed { get; set; }
        }
    }
}
