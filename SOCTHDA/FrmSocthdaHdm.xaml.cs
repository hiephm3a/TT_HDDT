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
using System.Data;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for FrmPoctpnaCopy.xaml
    /// </summary>
    public partial class FrmTT_SOCTHDA_HDDTHdm : Form
    {
        public FrmTT_SOCTHDA_HDDTHdm()
        {
            InitializeComponent();
            this.BindingSysObj = StartUp.SysObj;
            SmLib.SysFunc.LoadIcon(this);
        }

        public bool isOk = false;
        public FrmView frm;
        public DataSet dsHdm;

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct_old.Focus();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            //txtNgay_ct_new.Text
            if (!string.IsNullOrEmpty(txtNgay_ct_old.Text.Trim()))
            {
                try
                {
                    Convert.ToDateTime(txtNgay_ct_old.Text);
                }
                catch
                {
                    ExMessageBox.Show( 825,StartUp.SysObj, "Ngày bắt đầu không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_old.Focus();
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtNgay_ct_new.Text.Trim()))
            {
                try
                {
                    Convert.ToDateTime(txtNgay_ct_new.Text);
                   
                }
                catch
                {
                    ExMessageBox.Show( 830,StartUp.SysObj, "Ngày kết thúc không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_new.Focus();
                    return;
                }
            }
            frm = new FrmView(GetFilter());
            frm.ShowDialog();
           
            isOk = frm.isOk;
            dsHdm = frm.dsHdm;
            this.Close();
        }

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                isOk = false;
                this.Close();
            }
        }

        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            tblTen_kh.Text = "";
            if (txtMa_kh.RowResult != null)
            {
                if (StartUp.M_LAN.ToUpper().Equals("V"))
                    tblTen_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                else
                    tblTen_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
            }
        }

        private string GetFilter()
        {
            string filter = "1=1";
            if (txtNgay_ct_old.dValue != new DateTime())
            {
                filter += " AND ngay_ct >= '" + String.Format("{0:yyyyMMdd}", txtNgay_ct_old.dValue) + "'";
            }

            if (txtNgay_ct_new.dValue != new DateTime())
            {
                filter += " AND ngay_ct <= '" + String.Format("{0:yyyyMMdd}", txtNgay_ct_new.dValue) + "'";
            }

            if (!string.IsNullOrEmpty(txtMa_kh.Text.Trim()))
            {
                filter += " AND ma_kh LIKE '%" + txtMa_kh.Text + "%'";
            }

            if (!string.IsNullOrEmpty(txtma_hdm.Text.Trim()))
            {
                filter += " AND ma_hd LIKE '%" + txtma_hdm.Text + "%'";
            }
            filter += " AND status = 2";
            return filter;
        }
    }
}
