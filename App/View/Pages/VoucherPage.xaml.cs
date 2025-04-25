using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using App.Model;
using App.View.ViewModel;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace App.View.Pages
{
    public sealed partial class VoucherPage : Page
    {
        public VoucherViewModel VoucherViewModel { get; set; }
        private Voucher? _selectedVoucher;
        //private string DatabasePath = Path.Combine(AppContext.BaseDirectory, "database.db");

        public VoucherPage()
        {
            this.InitializeComponent();
            VoucherViewModel = new VoucherViewModel();
        }

        private void GenerateVoucher(object sender, RoutedEventArgs e)
        {
            if (CodeBox.Text == null)
            {
                ShowMessage("Vui lòng nhập mã Code.");
                return;
            }

            if (VoucherViewModel.isDup(CodeBox.Text))
            {
                ShowMessage("Mã Code bị trùng với dữ liệu trong CSDL, vui lòng chọn mã khác");
                return;
            }

            // Kiểm tra ngày hợp lệ
            if (StartDatePicker.Date == null || EndDatePicker.Date == null)
            {
                ShowMessage("Vui lòng chọn ngày hiệu lực.");
                return;
            }

            if (StartDatePicker.Date.Value > EndDatePicker.Date.Value)
            {
                ShowMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
                return;
            }

            // Kiểm tra số lượng hợp lệ
            if (!int.TryParse(VoucherQuantityBox.Text, out int quantity) || quantity < 1 || quantity > 20)
            {
                ShowMessage("Số lượng phải là số nguyên từ 1 đến 20.");
                return;
            }

            // Kiểm tra đơn hàng tối thiểu
            if (!decimal.TryParse(MinOrderBox.Text, out decimal minOrder) || minOrder < 0)
            {
                ShowMessage("Vui lòng nhập số tiền tối thiểu hợp lệ.");
                return;
            }

            // Kiểm tra giá trị giảm
            string discountText = DiscountValueBox.Text.Replace("%", "").Trim();
            if (!decimal.TryParse(discountText, out decimal discountValue) || discountValue <= 0 || discountValue > 100)
            {
                ShowMessage("Vui lòng nhập % hợp lệ, từ 0% đến 100%).");
                return;
            }

            // Nếu tất cả kiểm tra hợp lệ, tiến hành tạo mã giảm giá
            if (_selectedVoucher != null)
            {
                VoucherViewModel.UpdateVoucher(_selectedVoucher, CodeBox.Text, (DateTimeOffset)StartDatePicker.Date, (DateTimeOffset)EndDatePicker.Date, quantity, minOrder, discountValue, NoteBox.Text);
                ShowMessage("Đã cập nhật voucher");
                reser_all();
            }
            else
            {
                VoucherViewModel.CreateVoucher(CodeBox.Text, (DateTimeOffset)StartDatePicker.Date, (DateTimeOffset)EndDatePicker.Date, quantity, minOrder, discountValue, NoteBox.Text);
                ShowMessage("Đã tạo voucher");
                reser_all();

            }
        }

        private void reser_all()
        {
            AddVoucherButton.Content = "Tạo mã giảm giá";
            _selectedVoucher = null;
            CodeBox.Text = "Nhập mã code";
            StartDatePicker.Date = null;
            EndDatePicker.Date = null;
            VoucherQuantityBox.Text = "Nhập số lượng";
            NoteBox.Text = "Nhập mô tả voucher";
            MinOrderBox.Text = "Nhập số tiền tối thiểu";
            DiscountValueBox.Text = "Nhập phần trăm giảm";
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
                MinOrderBox.Text = selectedVoucher.MinOrder.ToString();
                DiscountValueBox.Text = selectedVoucher.DiscountValue.ToString();
                _selectedVoucher = selectedVoucher;
            }
            VoucherDialog.Hide();
            AddVoucherButton.Content = "Cập nhật thông tin mới";
        }


        private void DeleteVoucher_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton?.Tag is Voucher selectedVoucher)
                VoucherViewModel.RemoveVoucher(selectedVoucher.Code);
            VoucherDialog.Hide();
        }


        private async void ShowVoucherDialog(object sender, RoutedEventArgs e)
        {
            await VoucherDialog.ShowAsync();
        }

        private async void ShowMessage(string message)
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
