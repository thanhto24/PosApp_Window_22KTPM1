using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace App.Converter
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return value;  // Return null if value is null

            DateTime date;

            // If value is a string and can be parsed to DateTimeOffset (includes 'Z' for UTC)
            if (value is string str && DateTimeOffset.TryParse(str, out DateTimeOffset dto))
            {
                date = dto.DateTime.ToLocalTime();  // Convert DateTimeOffset to DateTime and adjust to local time
            }
            else if (value is DateTime dt)
            {
                date = dt.ToLocalTime();  // Convert DateTime to local time
            }
            else if (value is DateTimeOffset dateOffset)
            {
                date = dateOffset.DateTime.ToLocalTime();  // Convert DateTimeOffset to DateTime and adjust to local time
            }
            else
            {
                return value;  // Return the original value if it's neither a string, DateTime, nor DateTimeOffset
            }

            string format = parameter as string ?? "dd/MM/yyyy HH:mm";
            return date.ToString(format);  // Format the DateTime as per the parameter
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


}
