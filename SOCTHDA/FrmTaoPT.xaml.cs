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
using SmVoucherLib;
using System.Data;
using System.Data.SqlClient;
using SysLib;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for FrmTaoPT.xaml
    /// </summary>
    public partial class FrmTaoPT : Form
    {
        public bool isOk = false;
        public int kind = 1;
        public string Ma_nt_ht = "";
        public string so_hd = "";
        public string ngay_hd = "";
        public string filterma_qs = "";
        public DataTable tbInfoPT = null;

        public FrmTaoPT()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            BindingSysObj = StartUp.SysObj;
            GrdOkCancel.pnlButton.btnCancel.Visibility = Visibility.Collapsed;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            txtso_ct_pt.MaxLength = BindingSysObj.GetDatabaseFieldLength("so_ct");
            txtnguoi_nop.MaxLength = BindingSysObj.GetDatabaseFieldLength("ong_ba");
            txtlydo_nop.MaxLength = BindingSysObj.GetDatabaseFieldLength("dien_giai");

            if (tbInfoPT == null || (tbInfoPT != null && tbInfoPT.Rows.Count == 0))
            {
                txtKind.Value = kind;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "if(select count(1) from dmqs where ma_cts LIKE '%{0}%') = 1";
                cmd.CommandText += " select ma_qs from dmqs where ma_cts LIKE '%{1}%'";

                if (txtKind.Value.ToString().Equals("1"))
                {
                    txtMa_qs_pt.Filter = filterma_qs.Replace("HDA", "PT1");
                    txtMa_gd.Filter = "ma_ct = 'PT1' and status = 1 and ma_gd IN ('2','9')";
                    cmd.CommandText = string.Format(cmd.CommandText, "PT1", "PT1");
                }
                else
                {
                    txtMa_qs_pt.Filter = filterma_qs.Replace("HDA", "BC1");
                    txtMa_gd.Filter = "ma_ct = 'BC1' and status = 1 and ma_gd IN ('2','9')";
                    cmd.CommandText = string.Format(cmd.CommandText, "BC1", "BC1");
                }
                txtMa_gd.Text = StartUp.CommandInfo["parameter"].ToString().Split(';')[0];
                txtMa_gd.SearchInit();
                txtMa_gd_PreviewLostFocus(txtMa_gd, null);

                DataSet ds = BindingSysObj.ExcuteReader(cmd);
                if (ds.Tables.Count == 1)
                    txtMa_qs_pt.Text = ds.Tables[0].Rows[0]["ma_qs"].ToString();
                if (StartUp.M_LAN.Equals("V"))
                {
                    txtlydo_nop.Text = string.Format("Thu tiền hóa đơn số {0}, ngày {1}", so_hd, ngay_hd);
                }
                else
                {
                    txtlydo_nop.Text = string.Format("Invoice no. {0}, invoice date {1}", so_hd, ngay_hd);
                }
            }
            else
            {
                txtKind.Value = tbInfoPT.Rows[0]["ma_ct"].ToString().Equals("PT1") ? 1 : 2;
                if (txtKind.Value.ToString().Equals("1"))
                {
                    txtMa_qs_pt.Filter = filterma_qs.Replace("HD1", "PT1");
                    txtMa_gd.Filter = "ma_ct = 'PT1' and status = 1 and ma_gd IN ('2','9')";
                }
                else
                {
                    txtMa_qs_pt.Filter = filterma_qs.Replace("HD1", "BC1");
                    txtMa_gd.Filter = "ma_ct = 'BC1' and status = 1 and ma_gd IN ('2','9')";
                }
                txtMa_gd.Text = tbInfoPT.Rows[0]["ma_gd"].ToString();
                txtMa_gd.SearchInit();
                txtMa_gd_PreviewLostFocus(txtMa_gd, null);
                txtMa_qs_pt.Text = tbInfoPT.Rows[0]["ma_qs"].ToString();
                txtso_ct_pt.Text = tbInfoPT.Rows[0]["so_ct"].ToString().Trim();
                txtnguoi_nop.Text = tbInfoPT.Rows[0]["ong_ba"].ToString();
                txtlydo_nop.Text = tbInfoPT.Rows[0]["dien_giai"].ToString();
            }

            txtMa_nt.Text = StartUp.M_MA_NT0;
            if (Ma_nt_ht != StartUp.M_MA_NT0)
            {
                txtMa_nt.Filter = "ma_nt IN ('" + StartUp.M_MA_NT0 + "','" + Ma_nt_ht + "')";
            }
            else
            {
                txtMa_nt.IsReadOnly = true;
                txtMa_nt.IsTabStop = false;
            }
            
            txtKind.Focus();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            FormTrans _formparent = this.Owner as FormTrans;
            if (string.IsNullOrEmpty(txtMa_qs_pt.Text.Trim()) || !txtMa_qs_pt.CheckLostFocus())
            {
                ExMessageBox.Show(697, StartUp.SysObj, "Chưa vào mã quyển sổ phiếu thu", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtMa_qs_pt.IsFocus = true;
                return;
            }
            if (string.IsNullOrEmpty(txtso_ct_pt.Text.Trim()))
            {
                ExMessageBox.Show(698, StartUp.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtso_ct_pt.Focus();
                return;
            }

            string stt_rec_pt = "";
            if (tbInfoPT != null && tbInfoPT.Rows.Count == 1)
            {
                stt_rec_pt = tbInfoPT.Rows[0]["stt_rec"].ToString();
            }
            if (_formparent.CheckValidSoct(StartUp.SysObj, txtMa_qs_pt.Text, txtso_ct_pt.Text.PadLeft(txtso_ct_pt.MaxLength, ' '), stt_rec_pt))
            {
                if (txtMa_qs_pt.RowResult["chkso_ct"].ToString().Equals("1"))
                {
                    if (ExMessageBox.Show(699, StartUp.SysObj, "Số chứng từ đã tồn tại. Số cuối cùng là: " + "[" + _formparent.GetLastSoct(StartUp.SysObj, txtMa_qs_pt.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        txtso_ct_pt.SelectAll();
                        txtso_ct_pt.Focus();
                        return;
                    }
                }
                else if (txtMa_qs_pt.RowResult["chkso_ct"].ToString().Equals("2"))
                {
                    ExMessageBox.Show(694, StartUp.SysObj, "Số chứng từ đã tồn tại. Số cuối cùng là: " + "[" + _formparent.GetLastSoct(StartUp.SysObj, txtMa_qs_pt.Text).Trim() + "]", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtso_ct_pt.SelectAll();
                    txtso_ct_pt.Focus();
                    return;
                }
            }
            isOk=true;
            this.Close();
        }

        private void txtMa_qs_pt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus != GrdOkCancel.pnlButton.btnOk)
                txtso_ct_pt.Text = GetNewSoct(txtMa_qs_pt.Text);
        }

        public string GetNewSoct(string ma_qs)
        {
            string cmd;
            int M_AUTO_SOCT = Convert.ToInt16(BindingSysObj.GetOption("M_AUTO_SOCT").ToString());
            switch (M_AUTO_SOCT)
            {
                case 1:
                    cmd = "SELECT transform, so_ct + 1 as so_ct FROM dmqs WHERE ma_qs = '" + ma_qs.Trim() + "'";
                    break;
                default:
                    cmd = cmd = "EXEC  [GetNewSoct] '" + ma_qs.Trim() + "'";
                    break;
            }
            DataTable tbTmp = BindingSysObj.ExcuteReader(new SqlCommand(cmd)).Tables[0];
            if (tbTmp.Rows.Count > 0)
            {
                DataRow row = tbTmp.Rows[0];
                if (row[1] != null && row[1] != DBNull.Value)
                {
                    string so_ct_new = row[1].ToString();
                    return string.Format(row[0].ToString(), Convert.ToDouble(so_ct_new));
                }
            }
            return "";
        }

        private void txtKind_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtKind.Text))
                txtKind.Value = kind;
            if (txtKind.Value.ToString().Equals("1"))
                txtMa_qs_pt.Filter = "ma_cts like '%PT1%' and status=1";
            else
                txtMa_qs_pt.Filter = "ma_cts like '%BC1%' and status=1";

        }

        private void txtMa_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_gd.RowResult != null)
            {
                txtTen_gd.Text = StartUp.M_LAN.Equals("V") ? txtMa_gd.RowResult["ten_gd"].ToString() : txtMa_gd.RowResult["ten_gd2"].ToString();
            }
        }

    }
}
