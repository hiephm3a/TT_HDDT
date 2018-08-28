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
using System.Data;
using Sm.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using System.Windows.Interop;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for FrmPrintGlctpk1.xaml
    /// </summary>
    public partial class FrmPrintTT_SOCTHDA_HDDT : Form
    {
        public DataSet DsPrint = new DataSet();
        DataSet DsTmpPrint = null;
        int so_dong_in = 1;
        bool IsND51;
        public FrmPrintTT_SOCTHDA_HDDT(bool isND51)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this); 
            GridSearch.LocalSysObj = StartUp.SysObj;
            IsND51 = isND51;
            if (isND51)
            {
                GridSearch.ReportGroupName = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_file"].ToString();
            }
            else
            {
                GridSearch.ReportGroupName = StartUp.CommandInfo["rep_file"].ToString();
            }
            GridSearch.XGReport.RecordActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs>(XGReport_RecordActivated);

            if (BindingSysObj.GetOption("M_LAN").ToString().Equals("V"))
            {
                btnExport.Content = BindingSysObj.GetSysVar("M_EXPORT_SIGN").ToString();
            }
            else
            {
                btnExport.Content = BindingSysObj.GetSysVar2("M_EXPORT_SIGN").ToString();
            }
        }

        #region Form_Loaded
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            GridSearch.DSource = DsPrint;
            GridSearch.ReportPreviewMouseDoubleClick += new SmReport.ControlFilterReport.MouseClick(GridSearch_ReportPreviewMouseDoubleClick);
            //txtctu0.Value = 0;
            //txtlien.Value = StartUp.DmctInfo["so_lien"];
            DataTable tbIn = StartUp.GetPhIn();
            if (tbIn.Rows.Count == 0)
            {
                DataRow dr = tbIn.NewRow();
                dr["ma_ct"] = StartUp.Ma_ct;
                dr["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                dr["so01"] = 0;//StartUp.so_ct0;
                dr["so02"] = StartUp.DmctInfo["so_lien"];
                tbIn.Rows.Add(dr);

            }
            if (tbIn.Rows.Count == 1)
            {
                DataRow dr = tbIn.Rows[0];
                if (IsND51)
                {
                    dr["so02"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"].ToString().Trim();
                }
                else
                {

                }
            }
            //copy table Ct ra table temp
            //DsTmpPrint = DsPrint.Tables["TableCT"].Copy();
            DsTmpPrint = DsPrint.Copy();

            so_dong_in = Convert.ToInt16(StartUp.DmctInfo["so_dong_in"]);
            this.DataContext = tbIn;
            
        }

        void XGReport_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {
            if (GridSearch.XGReport.ActiveRecord == null || GridSearch.XGReport.ActiveRecord.RecordType != RecordType.DataRecord)
                return;
            DataRowView drvXReport = (GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView;
            string nd51 = drvXReport["nd51"].ToString();
            if (nd51 == "1")
            {
                txtlien.IsReadOnly = true;
                txtlien.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"].ToString().Trim();
            }
            else
            {
                txtlien.IsReadOnly = false;
                txtlien.Text = StartUp.DmctInfo["so_lien"].ToString().Trim();
            }
        }
        #endregion

        #region GridSearch_ReportPreviewMouseDoubleClick
        void GridSearch_ReportPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            View();
        } 
        #endregion
        
        #region InsertRowCT
        void InsertRowCT(string nd51)
        {
            DataRow[] _row = DsPrint.Tables["TableCT"].DefaultView.ToTable().Select("tag = 1");
            if (_row.Count() > 0)
                return;
            string stt_rec = DsPrint.Tables["TablePH"].DefaultView[0]["stt_rec"].ToString();
            
            int _index = 1;
            foreach (DataRowView dr in DsPrint.Tables["TableCT"].DefaultView)
            {
                dr["stt"] = _index;
                _index++;
            }

            /*Chị VANTT bảo không lên đoạn này 130734187
            if (nd51 == "1")
            {
                //Thêm dòng ghi chú
                string gc_thue = DsPrint.Tables["TablePH"].DefaultView[0]["gc_thue"].ToString().Trim();
                if (gc_thue != "")
                {
                    DataRow row = DsPrint.Tables["TableCT"].NewRow();
                    row["stt_rec"] = stt_rec;
                    row["ten_vt"] = "(" + gc_thue + ")";
                    row["tag"] = 1;
                    DsPrint.Tables["TableCT"].Rows.Add(row);
                }
            }
            */
            //thêm dòng chiết khấu
            decimal t_ck = Convert.ToDecimal(DsPrint.Tables["TablePH"].DefaultView[0]["t_ck"]);
            decimal t_ck_nt = Convert.ToDecimal(DsPrint.Tables["TablePH"].DefaultView[0]["t_ck_nt"]);
            if (t_ck != 0 || t_ck_nt != 0)
            {
                DataRow newrow = DsPrint.Tables["TableCT"].NewRow();
                newrow["stt_rec"] = stt_rec;
                newrow["stt_rec0"] = -1;
                newrow["ten_vt"] = "Chiết khấu";
                newrow["ten_vt2"] = "Discount";
                newrow["tien2"] = t_ck;
                newrow["tien_nt2"] = t_ck_nt;
                newrow["tag"] = 1;
                DsPrint.Tables["TableCT"].Rows.Add(newrow);
            }

            int rowCountCT = DsPrint.Tables["TableCT"].DefaultView.Count;
            GridSearch.InsertSubRow("HDA", "TableCT");
            ////Thêm số dòng cho đủ ngầm định
            //if (rowCountCT < so_dong_in)
            //{
            //    for (int k = rowCountCT; k < so_dong_in; k++)
            //    {
            //        DataRow row = DsPrint.Tables["TableCT"].NewRow();
            //        row["stt_rec"] = stt_rec;
            //        row["stt_rec0"] = "999";
            //        row["tag"] = 1;
            //        DsPrint.Tables["TableCT"].Rows.Add(row);
            //    }

            //}
            DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";
            GridSearch.DSource = DsPrint;
        } 
        #endregion

        #region ResetTableCt
        void ResetTableCt()
        {
            string stt_rec = DsPrint.Tables["TablePH"].DefaultView[0]["stt_rec"].ToString();
            if (DsTmpPrint != null)
            {
                DsPrint = DsTmpPrint.Copy();
                DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";
                GridSearch.DSource = DsPrint;
            }
            //if (DsTmpPrint != null && DsPrint.Tables["TableCT"].Rows.Count > DsTmpPrint.Rows.Count)
            //{
                
            //    //lay ra cac dong trang va dong ghi chu 
            //    DataRow[] _row = DsPrint.Tables["TableCT"].Select("tag = 1");
            //    foreach (DataRow dr in _row)
            //    {
            //        //delete các row có trong grdcp
            //        DsPrint.Tables["TableCT"].Rows.Remove(dr);
            //    }
                
            //    DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                
                
            //}
            ////thêm dòng chiết khấu
            //decimal t_ck = Convert.ToDecimal(DsPrint.Tables["TablePH"].DefaultView[0]["t_ck"]);
            //decimal t_ck_nt = Convert.ToDecimal(DsPrint.Tables["TablePH"].DefaultView[0]["t_ck_nt"]);
            //if (t_ck != 0 || t_ck_nt != 0)
            //{
            //    DataRow newrow = DsPrint.Tables["TableCT"].NewRow();
            //    newrow["stt_rec"] = stt_rec;
            //    newrow["ten_vt"] = "Chiết khấu";
            //    newrow["tien2"] = t_ck;
            //    newrow["tien_nt2"] = t_ck_nt;
            //    newrow["tag"] = 1;
            //    DsPrint.Tables["TableCT"].Rows.Add(newrow);
            //}
            //int rowCountCT = DsPrint.Tables["TableCT"].DefaultView.Count;
            ////Thêm số dòng cho đủ ngầm định
            //if (rowCountCT < so_dong_in)
            //{
            //    for (int k = rowCountCT; k < so_dong_in; k++)
            //    {
            //        DataRow row = DsPrint.Tables["TableCT"].NewRow();
            //        row["stt_rec"] = stt_rec;
            //        row["tag"] = 1;
            //        DsPrint.Tables["TableCT"].Rows.Add(row);
            //    }

            //}
            
            //DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            //GridSearch.DSource = DsPrint;
        }

        #endregion

        #region btnin_Click
        private void btnin_Click(object sender, RoutedEventArgs e)
        {
            if (GridSearch.XGReport.ActiveRecord == null)
                return;
            if (txtlien.Value != null)
            {
                int sl_in = Convert.ToInt16(StartUp.DsTrans.Tables[0].DefaultView[0]["sl_in"]);
                DataRowView drvXReport = (GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView;

                string nd51 = drvXReport["nd51"].ToString();
                if (nd51 == "1" && sl_in > 0)
                {
                    if (ExMessageBox.Show( 390,StartUp.SysObj, "Hóa đơn đã được in, có muốn in lại hay không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        return;

                    FrmLogin login = new FrmLogin();
                    login.ShowDialog();
                    if (!login.IsLogined)
                        return;
                }
                int so_lien = 1, so_lien_hd = 0, so_lien_xac_minh = 0;
                int so_lien_hien_thoi = Convert.ToInt32(StartUp.GetSo_lien((DataRecord)GridSearch.XGReport.ActiveRecord, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()));
                int.TryParse(DsPrint.Tables["TablePH"].DefaultView[0]["so_lien_hd"].ToString(), out so_lien_hd);
                int.TryParse(StartUp.DmctInfo["so_lien_xac_minh"].ToString(), out so_lien_xac_minh);

                if (so_lien_hien_thoi > so_lien_hd)
                    DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "BẢN SAO";
                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                while (so_lien <= so_lan_in)
                {
                    
                    DsPrint.Tables["TablePH"].DefaultView[0]["so_lien"] = so_lien;

                    if (so_lien_hien_thoi <= so_lien_hd)
                    {
                        if (so_lien <= so_lien_hien_thoi || so_lien > so_lien_hd)
                            DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "BẢN SAO";
                        else
                            DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "";
                    }
                    
                    InsertRowCT(nd51);
                    GridSearch.V_In(1, (so_lien_xac_minh >= so_lien && string.IsNullOrEmpty(DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"].ToString())));
                    so_lien++;
                }
               
                //update sl_in và so_lien in
                if (nd51 == "1" && GridSearch.PrintSuccess)
                {
                    string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                    StartUp.UpdateSl_in(stt_rec, drvXReport["id"].ToString(), txtlien.Text);
                    StartUp.DsTrans.Tables[0].DefaultView[0]["sl_in"] = StartUp.GetSl_in(stt_rec);
                }
                ResetTableCt();
                StartUp.SetPhIn(this.DataContext as DataTable);
            }
        	this.Close();
		}
        #endregion

        #region btnin_lt_Click
        private void btnin_lt_Click(object sender, RoutedEventArgs e)
        {
            if (GridSearch.XGReport.ActiveRecord == null)
                return;

            DataRowView drvXReport = (GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView;
            string mau_tu_in = drvXReport["mau_tu_in"].ToString();

            if (StartUp.IsQLHD && mau_tu_in == "1")
            {
                ExMessageBox.Show( 395,StartUp.SysObj, "Có chứng từ thuộc mẫu hóa đơn tự in, không in liên tục được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (StartUp.SysObj.GetOption("M_IN_HOI_CK").ToString() == "1")
            {
                if (ExMessageBox.Show( 400,StartUp.SysObj, "Có chắc chắn in tất cả các chứng từ đã được lọc?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    int so_lien;
                    List<int> lstSo_lien = new List<int>();
                    
                    if (txtlien.Value != null)
                    {
                        int iRowTmp = FrmTT_SOCTHDA_HDDT.iRow;
                        so_lien = 1;
                        bool isPrint = false;
                        int so_lien_xac_minh = 0;
                        int.TryParse(StartUp.DmctInfo["so_lien_xac_minh"].ToString(), out so_lien_xac_minh);

                        int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                        while (so_lien <= so_lan_in)
                        {
                            for (int i = 1; i < DsPrint.Tables[0].Rows.Count; i++)
                            {
                                string stt_rec = DsPrint.Tables[0].Rows[i]["stt_rec"].ToString();
                                DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                                DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                                DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";

                                if (so_lien == 1)
                                {
                                    lstSo_lien.Add(Convert.ToInt32(StartUp.GetSo_lien((DataRecord)GridSearch.XGReport.ActiveRecord, stt_rec)));
                                }

                                if (DsPrint.Tables[0].Rows[i]["status"].ToString() != "3")
                                {

                                    int so_lien_hien_thoi = lstSo_lien[i - 1], so_lien_hd = 0;
                                    int.TryParse(DsPrint.Tables["TablePH"].DefaultView[0]["so_lien_hd"].ToString(), out so_lien_hd);

                                    if (so_lien_hien_thoi > so_lien_hd)
                                        DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "BẢN SAO";
                                    else
                                    {
                                        if (so_lien <= so_lien_hien_thoi || so_lien > so_lien_hd)
                                            DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "BẢN SAO";
                                        else
                                            DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "";
                                    }

                                    DsPrint.Tables["TablePH"].DefaultView[0]["so_lien"] = so_lien;

                                    int sl_in = Convert.ToInt16(StartUp.DsTrans.Tables[0].Rows[i]["sl_in"]);
                                   

                                    if (mau_tu_in == "1" && sl_in > 0 && !isPrint)
                                    {
                                        if (ExMessageBox.Show( 405,StartUp.SysObj, "Hóa đơn đã được in, có muốn in lại hay không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                            return;

                                        FrmLogin login = new FrmLogin();
                                        login.ShowDialog();
                                        if (!login.IsLogined)
                                            return;

                                        // da thong bao roi, lan sau ko thong bao nua
                                        isPrint = true;
                                    }


                                    InsertRowCT(mau_tu_in);
                                    GridSearch.V_In(1, (so_lien_xac_minh >= so_lien&& string.IsNullOrEmpty(DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"].ToString())));
                                    if (mau_tu_in == "1" && so_lien == 1 && GridSearch.PrintSuccess)
                                    {
                                        StartUp.UpdateSl_in(stt_rec, drvXReport["id"].ToString(), txtlien.Text);
                                        StartUp.DsTrans.Tables[0].Rows[i]["sl_in"] = StartUp.GetSl_in(stt_rec);
                                    }

                                }
                            }
                            so_lien++;
                        }
                        ResetTableCt();
                        DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + DsPrint.Tables["TablePH"].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                        DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + DsPrint.Tables["TablePH"].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                        DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";
                        StartUp.SetPhIn(this.DataContext as DataTable);
                       
                    }
                    this.Close();
                }
            }
        }
        #endregion

        #region btnxem_Click
        private void btnxem_Click(object sender, RoutedEventArgs e)
        {
            View();
        }
        void View()
        {
            if (GridSearch.XGReport.ActiveRecord == null)
                return;
            string nd51 = ((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString();
            int so_lien_hien_thoi = Convert.ToInt32(StartUp.GetSo_lien((DataRecord)GridSearch.XGReport.ActiveRecord, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()));

            if (so_lien_hien_thoi > 0)
                DsPrint.Tables["TablePH"].DefaultView[0]["ban_sao"] = "BẢN SAO";
            //string _str_sl_in = StartUp.GetSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()).ToString();
            //if (!_str_sl_in.Equals("0"))
            //{
            //    DsPrint.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";
            //    StartUp.DsTrans.Tables[0].DefaultView[0]["sl_in"] = _str_sl_in;
            //}
            DsPrint.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;

            InsertRowCT(nd51);

            StartUp.SetPhIn(this.DataContext as DataTable);
            GridSearch.V_Xem();
            ResetTableCt();
        }
        #endregion

        #region btnthoat_Click
        private void btnthoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region txtlien_LostFocus
        private void txtlien_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtlien.IsFocusWithin)
            {
                if (txtlien.Value.ToString() == "")
                    txtlien.Value = 0;
            }
        }
        #endregion

        #region txtctu0_LostFocus
        private void txtctu0_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtctu0.IsFocusWithin)
            {
                if (txtctu0.Value.ToString() == "")
                    txtctu0.Value = 0;
                DsPrint.Tables["TablePH"].DefaultView[0]["so_ct_goc"] = txtctu0.Value;
            }
        }
        #endregion

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (DsPrint.Tables[0].DefaultView.Count == 1)
            {
                string file_name = StartUp.GetFileNameExportWithSignature(DsPrint.Tables[0].DefaultView[0]);
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                GridSearch.V_XuatPdf(file_name, windowHandle);
            }
        }
    }
}
