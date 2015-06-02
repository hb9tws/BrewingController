using System;
using Windows.UI.Xaml.Data;

namespace BrewingController.View
{
    public class DoubleToCelsiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String temperature = "Unknown";

            if (value is double )
            {
                temperature = $"{(double) value:F1} °C";
            }
            return temperature;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
