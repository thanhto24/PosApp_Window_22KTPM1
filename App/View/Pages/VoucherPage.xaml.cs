using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using App.Model;
using App.View.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace App.View.Pages
{
    public sealed partial class VoucherPage : Page
    {
        public VoucherViewModel VoucherViewModel { get; set; }
        private Voucher? _selectedVoucher;

        public VoucherPage()
        {
            this.InitializeComponent();
            VoucherViewModel = new VoucherViewModel();
        }

        private async void GenerateVoucher(object sender, RoutedEventArgs e)
        {
            // Kiểm tra ngày hợp lệ
            if (StartDatePicker.Date == null || EndDatePicker.Date == null)
            {
                await ShowMessage("Vui lòng chọn ngày hiệu lực.");
                return;
            }

            if (StartDatePicker.Date.Value > EndDatePicker.Date.Value)
            {
                await ShowMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
                return;
            }

            // Kiểm tra số lượng hợp lệ
            if (!int.TryParse(VoucherQuantityBox.Text, out int quantity) || quantity < 1 || quantity > 20)
            {
                await ShowMessage("Số lượng phải là số nguyên từ 1 đến 20.");
                return;
            }

            // Kiểm tra đơn hàng tối thiểu
            if (!decimal.TryParse(MinOrderBox.Text, out decimal minOrder) || minOrder <= 0)
            {
                await ShowMessage("Vui lòng nhập số tiền tối thiểu hợp lệ.");
                return;
            }

            // Kiểm tra giá trị giảm
            bool isPercentage = DiscountValueBox.Text.Contains("%");
            string discountText = DiscountValueBox.Text.Replace("%", "").Trim();
            if (!decimal.TryParse(discountText, out decimal discountValue) || discountValue <= 0 || (isPercentage && discountValue > 100))
            {
                await ShowMessage("Vui lòng nhập giá trị giảm hợp lệ (nếu là phần trăm, tối đa 100%).");
                return;
            }

            // Kiểm tra mức giảm tối đa
            if (!decimal.TryParse(MaxDiscountBox.Text, out decimal maxDiscount) || maxDiscount <= 0)
            {
                await ShowMessage("Vui lòng nhập mức giảm tối đa hợp lệ.");
                return;
            }

            // Nếu tất cả kiểm tra hợp lệ, tiến hành tạo mã giảm giá
            // CreateVoucher(quantity, minOrder, discountValue, maxDiscount);
        }

        private void EditVoucher_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton?.Tag is Voucher selectedVoucher)
            {
                // Populate the form fields with the voucher's details
                CodeBox.Text = selectedVoucher.Code;
                StartDatePicker.Date = selectedVoucher.StartDate;
                EndDatePicker.Date = selectedVoucher.EndDate;
                VoucherQuantityBox.Text = selectedVoucher.Quantity.ToString();
                NoteBox.Text = selectedVoucher.Note;
                MinOrderBox.Text = selectedVoucher.MinOrderValue.ToString();
                DiscountValueBox.Text = selectedVoucher.DiscountValue.ToString();
                MaxDiscountBox.Text = selectedVoucher.MaxDiscount.ToString();
            }
            VoucherDialog.Hide();
        }


        private void DeleteVoucher_Click(object sender, RoutedEventArgs e)
        {
            VoucherDialog.Hide();
        }


        private async void ShowVoucherDialog(object sender, RoutedEventArgs e)
        {
            await VoucherDialog.ShowAsync();
        }

        private async Task ShowMessage(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Thông báo",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot // Set XamlRoot
            };

            await dialog.ShowAsync();
        }

    }
}