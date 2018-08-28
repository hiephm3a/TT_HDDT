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
    /// Interaction logic for FrmKHInfo.xaml
    /// </summary>
    public partial class FrmKHInfo : Sm.Windows.Controls.Form
    {
        public FrmKHInfo()
        {
            InitializeComponent();
            DisplayLanguage = StartUp.M_LAN;
            BindingSysObj = StartUp.SysObj;
            SmLib.SysFunc.LoadIcon(this);
        }

        public bool isError = true;
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            txtTen_kh.Text = StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"].ToString();
            txtDia_chi.Text = StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow]["dia_chi"].ToString();
            txtMa_so_thue.Text = StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow]["ma_so_thue"].ToString();
            txtTen_kh.Focus();
        }

        #region ConfirmGridView_OnOk
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"] = txtTen_kh.Text;
                StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow]["dia_chi"] = txtDia_chi.Text;
                StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow]["ma_so_thue"] = txtMa_so_thue.Text;
                isError = false;
                this.Close();
            }
        }
        #endregion

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            #region ma_so_thue
            if (!StartUp.M_MST_CHECK.Equals("0") && txtMa_so_thue.Text.Trim() != "")
            {
                //if (string.IsNullOrEmpty(txtMa_so_thue.Text.Trim()))
                //{
                //    ExMessageBox.Show( 370,StartUp.SysObj, "Chưa vào mã số thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                //    if (StartUp.M_MST_CHECK.Equals("2"))
                //    {
                //        result = false;
                //        txtMa_so_thue.Focus();
                //    }
                //}
                //else if (!SmLib.SysFunc.CheckSumMaSoThue(txtMa_so_thue.Text.Trim()))

                if (!SmLib.SysFunc.CheckSumMaSoThue(txtMa_so_thue.Text.Trim()))
                {
                    if (StartUp.M_MST_CHECK.Equals("1"))
                    {
                        ExMessageBox.Show( 375,StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    else// if (StartUp.M_MST_CHECK.Equals("2"))
                    {
                        ExMessageBox.Show( 380,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_so_thue.Focus();
                    }
                }
            }
            #endregion
            return result;
        }
        #endregion

        #region ConfirmGridView_OnCancel
        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isError = true;
            this.Close();
        } 
        #endregion

        //private static bool VerifyTaxCode(string code)
        //{
        //    int length = code.Length;
        //    if (length == 10)
        //    {
        //        for (int i = 0; i < length; i++)
        //        {
        //            int n = (int)code[i];
        //            if (!(n >= 48 && n <= 58))
        //                return false;
        //        }
        //        return true;
        //    }
        //    else if (length == 14)
        //    {
        //        if ((int)code[10] != 45)
        //            return false;

        //        string code1 = code.Substring(0, 10);
        //        string code2 = code.Substring(11, 3);
        //        for (int i = 0; i < code1.Length; i++)
        //        {
        //            int n = (int)code1[i];
        //            if (!(n >= 48 && n <= 58))
        //                return false;
        //        }
        //        for (int j = 0; j < code2.Length; j++)
        //        {
        //            int n = (int)code1[j];
        //            if (!(n >= 48 && n <= 58))
        //                return false;
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
            
           
        //}
       
    }
}
