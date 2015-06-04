using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BrewingController.View
{
    class EnumToBooleanConverter : IValueConverter
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
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string parm = parameter.ToString();
                int lastDot = parm.LastIndexOf(".", StringComparison.Ordinal);
                string enumName = parm.Substring(0, lastDot);
                string enumValue = parm.Substring(lastDot + 1);

                Type enumtype = Type.GetType(enumName);
                return Enum.Parse(enumtype, enumValue);
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
