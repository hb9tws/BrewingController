using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BrewingController.View
{
    class ActuatorStateToColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string parm = parameter.ToString();
                int lastDot = parm.LastIndexOf(".", StringComparison.Ordinal);
                string enumName = parm.Substring(0, lastDot);
                string enumValue = parm.Substring(lastDot + 1);

                Type enumtype = Type.GetType(enumName);
                object obj = Enum.Parse(enumtype, enumValue);

                if (obj.Equals(value))
                {
                    return new SolidColorBrush(Colors.ForestGreen);
                }
                else
                {
                    return new SolidColorBrush(Colors.OrangeRed);
                }
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.DarkGray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
