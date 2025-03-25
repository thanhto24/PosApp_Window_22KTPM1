using System;
using Microsoft.UI.Xaml.Data;

namespace App.Converters
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int price)
            {
                return $"Giá: {price:N0} VND";
            }
            return "Giá: 0 VND";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
