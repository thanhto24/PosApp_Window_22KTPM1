using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace App.Converter
{
    public class IntegerToVisibilityMsgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}