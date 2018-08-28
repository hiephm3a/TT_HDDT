using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sm.Windows.Controls;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for FrmTotalInfo.xaml
    /// </summary>
    public partial class FrmTotalInfo : Form
    {
        CodeValueBindingObject Voucher_Ma_nt0;
        public FrmTotalInfo()
        {
            InitializeComponent();
            DisplayLanguage = StartUp.M_LAN;
            BindingSysObj = StartUp.SysObj;
            SmLib.SysFunc.LoadIcon(this);
            this.ConfirmGV.ButtonType = 1;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Voucher_Ma_nt0 = (CodeValueBindingObject)TT_SOCTHDA_HDDTTotalInfo.FindResource("Voucher_Ma_nt0");
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            this.ConfirmGV.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            
        }

        private void ConfirmGV_OnOk(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
