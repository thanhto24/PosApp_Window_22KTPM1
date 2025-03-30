using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace App.Converter
{
    public class IntegerToVisibilityListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}