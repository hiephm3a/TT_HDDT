using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Socthda
{
    class CheckBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int suatien = 0;
            int.TryParse(value.ToString(), out suatien);
            return suatien == 1 ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ischecked = bool.Parse(value.ToString());
            return ischecked == true ? 1 : 0;
        }
    }
}
