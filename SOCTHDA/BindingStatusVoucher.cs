using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Data.SqlClient;
using SmErrorLib;

namespace Socthda
{
    class BindingStatusVoucher : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString().Trim()))
            {
                DataRow[] rows = StartUp.tbStatus.Select("Ma_post =" + value);
                if (rows.Count() > 0)
                {
                    DataRow row = rows[0] as DataRow;
                    return StartUp.tbStatus.Rows.IndexOf(row);
                }
            }
            if (StartUp.tbStatus.Select("Default = 1").Count() > 0)
                return StartUp.tbStatus.Rows.IndexOf(StartUp.tbStatus.Select("Default = 1")[0] as DataRow);
            return StartUp.tbStatus.Rows.IndexOf(StartUp.tbStatus.Rows[0] as DataRow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return StartUp.DmctInfo["ma_post"];
            else
            {
                int i = 1;
                int.TryParse(value.ToString(), out i);
                return StartUp.tbStatus.DefaultView[i]["Ma_post"];
            }
        }

        #endregion
    }

  
}
