    using System;
    using Microsoft.UI.Xaml.Data;

    namespace App.Converter
    {
        public class CurrencyConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (value is decimal amount)
                {
                    return string.Format("{0:#,##0} đ", amount);
                }

                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                throw new NotImplementedException();
            }
        }
    }