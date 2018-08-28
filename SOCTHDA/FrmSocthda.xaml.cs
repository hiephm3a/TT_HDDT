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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmDefine;
using Infragistics.Windows.DataPresenter;
using SmVoucherLib;
using System.Data;
using Sm.Windows.Controls;
using System.Data.SqlClient;
using SmDataLib;
using Sm.Windows.Controls.ControlLib;
using SmLib;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using HddtLib.Interface;
using HddtLib;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmTT_SOCTHDA_HDDT : SmVoucherLib.FormTrans
    {


        public static int iRow = 0;
        int iRow_old = 0;
        //THAMTT
        private string M_SD_HDDT = "";
        private string M_HDDT_DUYET = "";
        private string M_tt_hddt_tcn = "";
        private string flag_button = "";//3-DC tang, 4-DC Giam, 5-Thay The
        private string _so_ct_tt = "";
        private string _stt_rec_tt = "";
        private string _ma_kh_tt = "";
        //ENDTHAMTT
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Lan0;
        CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject IsCheckedPx_gia_dd;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject M_Ngay_lct;
        //THAMTT
        CodeValueBindingObject IsCheckedPH_HDDT;
        //ENDTHAMTT
        // public static ActionTask currActionTask = ActionTask.None;


        string ma_kh_old = "";

        public DataSet DsVitual;
        private DataSet dsCheckData;
        public FrmTT_SOCTHDA_HDDT()
        {
            InitializeComponent();
            LanguageProvider.Language = StartUp.M_LAN;
            this.BindingSysObj = StartUp.SysObj;
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_So_ct = txtSo_ct;
            C_Ma_nt = txtMa_nt;

            if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FK") || (StartUp.dtRegInfo != null && StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
            {
                btnChonHDB.Visibility = Visibility.Collapsed;
            }
        }

        #region FrmTT_SOCTHDA_HDDT_Loaded
        void FrmTT_SOCTHDA_HDDT_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                StartUp.M_KM_CK = Convert.ToInt16(BindingSysObj.GetOption(stt_mau_temlate.ToString(), "M_KM_CK"));
                StartUp.M_AR_CK = Convert.ToInt16(BindingSysObj.GetOption(stt_mau_temlate.ToString(), "M_AR_CK"));

                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsCheckedPx_gia_dd = (CodeValueBindingObject)FormMain.FindResource("IsCheckedPx_gia_dd");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");

                M_Ngay_lct = (CodeValueBindingObject)FormMain.FindResource("M_Ngay_lct");
                M_Ngay_lct.Value = StartUp.M_ngay_lct.Equals("1");


                string M_CDKH13 = SysO.GetOption("M_CDKH13").ToString().Trim();
                if (M_CDKH13 != "1")
                    txtSoDuKH.Visibility = tblSoDuKH.Visibility = Visibility.Hidden;
                //THAMTT
                IsCheckedPH_HDDT = (CodeValueBindingObject)FormMain.FindResource("IsCheckedPH_HDDT");
                this.M_SD_HDDT = StartupBase.SysObj.GetOption("M_SD_HDDT").ToString();
                if (this.M_SD_HDDT != "1")
                {
                    this.tabHDDT.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.tabHDDT.Visibility = Visibility.Visible;
                }

                //this.M_HDDT_DUYET = StartupBase.SysObj.GetOption("M_HDDT_DUYET").ToString();
                //if (this.M_HDDT_DUYET == "0")
                //{
                //    btnPH.IsEnabled = false;
                //}
                //end THAMTT

                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.TwoWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                GrdCt.Lan = StartUp.M_LAN;
                M_LAN = StartUp.M_LAN;

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);

                //load form theo stt_rec
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());

                    LoadData();

                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                    IsCheckedPx_gia_dd.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["px_gia_dd"].ToString() == "1");
                    Ty_Gia_ValueChange.Value = false;


                    Voucher_Lan0.Value = M_LAN.Trim().Equals("V");
                    //THAMTT
                    IsCheckedPH_HDDT.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "0");
                    M_tt_hddt_tcn = StartUp.DsTrans.Tables[0].Rows[iRow]["tt_hddt_tcn"].ToString();
                    //IsEnableButton(M_tt_hddt_tcn, M_HDDT_DUYET);
                    //ENDTHAMTT

                }

                //TabInfo.SelectedIndex = 0;
                SetFocusToolbar();

                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ma_kh_old = txtMa_kh.Text.Trim();
                    }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        #region LoadData
        void LoadData()
        {
            GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

            //grid Hạch toán
            this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;

            //trạng thái
            txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;
            if (StartUp.tbStatus.DefaultView.Count == 1)
            {
                txtStatus.IsEnabled = false;
            }

            UpdateTienKM();
            UpdateTienKM_NT();
        }
        #endregion

        #endregion

        #region V_Sau
        void V_Sau()
        {
            if (iRow < StartUp.DsTrans.Tables[0].Rows.Count - 1)
                iRow++;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

            //THAMTT
            IsCheckedPH_HDDT.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "0");
            //M_tt_hddt_tcn = StartUp.DsTrans.Tables[0].Rows[iRow]["tt_hddt_tcn"].ToString();
            //IsEnableButton(M_tt_hddt_tcn, M_HDDT_DUYET);

        }
        #endregion

        #region V_Truoc
        void V_Truoc()
        {
            if (iRow > 1)
                iRow--;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

            //THAMTT
            IsCheckedPH_HDDT.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "0");
            //M_tt_hddt_tcn = StartUp.DsTrans.Tables[0].Rows[iRow]["tt_hddt_tcn"].ToString();
            //IsEnableButton(M_tt_hddt_tcn, M_HDDT_DUYET);
            //ENDTHAMTT

        }
        #endregion

        #region V_Dau
        void V_Dau()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count > 1 ? 1 : 0;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            //THAMTT
            IsCheckedPH_HDDT.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "0");

            //M_tt_hddt_tcn = StartUp.DsTrans.Tables[0].Rows[iRow]["tt_hddt_tcn"].ToString();
            //IsEnableButton(M_tt_hddt_tcn, M_HDDT_DUYET);
            //ENDTHAMTT

        }
        #endregion

        #region V_Cuoi
        void V_Cuoi()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            //THAMTT
            IsCheckedPH_HDDT.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "0");
            M_tt_hddt_tcn = StartUp.DsTrans.Tables[0].Rows[iRow]["tt_hddt_tcn"].ToString();
            //IsEnableButton(M_tt_hddt_tcn, M_HDDT_DUYET);
            //ENDTHAMTT

        }
        #endregion

        #region V_Moi
        private void V_Moi()
        {
            try
            {
                currActionTask = ActionTask.Add;
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                //khi thêm mới, gán lại giá trị 
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    DsVitual = StartUp.DsTrans.Copy();

                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ma_ct"] = StartUp.Ma_ct;
                    NewRecord["row_id"] = 0;

                    if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                    {
                        NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;
                    }
                    else
                    {
                        NewRecord["ngay_ct"] = DateTime.Now.Date;
                    }


                    if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                    {
                        NewRecord["ma_nt"] = StartUp.DmctInfo["ma_nt"];
                        NewRecord["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                        NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]), StartUp.M_User_Id);
                    }
                    else
                    {
                        NewRecord["ma_gd"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_gd"];
                        NewRecord["ma_nt"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"];
                        NewRecord["so_seri"] = StartUp.DsTrans.Tables[0].Rows[iRow]["so_seri"];
                        NewRecord["ma_thue"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_thue"];
                        NewRecord["thue_suat"] = StartUp.DsTrans.Tables[0].Rows[iRow]["thue_suat"];
                        NewRecord["tk_thue_co"] = StartUp.DsTrans.Tables[0].Rows[iRow]["tk_thue_co"];
                        NewRecord["loai_tk_co"] = StartUp.DsTrans.Tables[0].Rows[iRow]["loai_tk_co"].ToString();


                        NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, StartUp.DsTrans.Tables[0].Rows[iRow]["ma_qs"].ToString().Trim());
                    }

                    if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                        NewRecord["ty_giaf"] = 1;
                    else
                        NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);

                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["sua_tien"] = 0;
                    NewRecord["px_gia_dd"] = 0;
                    NewRecord["sua_tkthue"] = 0;
                    NewRecord["sua_thue"] = 0;
                    NewRecord["tinh_ck"] = 0;
                    NewRecord["t_tien"] = 0;
                    NewRecord["t_tien_nt"] = 0;
                    NewRecord["t_tien2"] = 0;
                    NewRecord["t_tien_nt2"] = 0;
                    NewRecord["t_tien_sau_ck"] = 0;
                    NewRecord["t_tien_sau_ck_nt"] = 0;
                    NewRecord["t_thue"] = 0;
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_ck"] = 0;
                    NewRecord["t_ck_nt"] = 0;
                    NewRecord["han_tt"] = 0;
                    NewRecord["sl_in"] = 0;
                    NewRecord["flag_btn"] = "0";
                    //THAMTT
                    NewRecord["user_id_ph"] = StartUp.M_User_Id;
                    NewRecord["tt_hddt_tcn"] = "0";
                    NewRecord["ngay_ct_luu"] = DateTime.Now.Date;
                    NewRecord["so_ct_luu"] = "";
                    if (flag_button == "3" || flag_button == "4" || flag_button == "5")
                    {
                        NewRecord["so_ct_tt"] = _so_ct_tt;
                        NewRecord["stt_rec_tt"] = _stt_rec_tt;
                        NewRecord["ma_kh"] = _ma_kh_tt;
                        NewRecord["flag_btn"] = flag_button;
                    }
                    //

                    //Them moi dong trong Ct
                    DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                    NewCtRecord["stt_rec"] = newSttRec;
                    NewCtRecord["stt_rec0"] = "001";
                    NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                    NewCtRecord["ngay_ct"] = NewRecord["ngay_ct"];
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        NewCtRecord["ma_kho_i"] = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"];
                    NewCtRecord["so_luong"] = 0;
                    NewCtRecord["gia_nt"] = 0;
                    NewCtRecord["tien_nt"] = 0;
                    NewCtRecord["gia"] = 0;
                    NewCtRecord["tien"] = 0;
                    NewCtRecord["tl_ck"] = 0;
                    NewCtRecord["ck"] = 0;
                    NewCtRecord["ck_nt"] = 0;
                    NewCtRecord["gia_nt2"] = 0;
                    NewCtRecord["tien_nt2"] = 0;
                    NewCtRecord["gia2"] = 0;
                    NewCtRecord["tien2"] = 0;
                    NewCtRecord["km_ck"] = 0;
                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);

                    iRow_old = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;

                    //set lai so du kh
                    txtSoDuKH.Value = 0;

                    //filter lại Table[0], Table[1]
                    StartUp.DataFilter(newSttRec);
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"].ToString());

                    DsVitual = null;
                    IsInEditMode.Value = true;
                    //THAMTT
                    if (M_SD_HDDT == "1")
                    {
                        txtMa_qs.Filter = "ma_cts like '%HDA%' and status=1 and loai_qs = '0'";
                    }
                    //ENDTHAMTT
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        txtma_gd.SelectAllOnFocus = true;
                        txtma_gd.IsFocus = true;

                    }));
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region V_Sua
        private void V_Sua()
        {
            if (StartUp.DsTrans.Tables[0].Rows.Count == 0)
            {
                ExMessageBox.Show(445, StartUp.SysObj, "Không có dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //THAMTT
            if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "8")
            {
                ExMessageBox.Show(1451, StartUp.SysObj, "Hóa đơn đã hủy. Không được sửa!!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //THAMTT
            if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                return;

            //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
            //{
            //    ExMessageBox.Show( 450,StartUp.SysObj, "Dữ liệu đã khóa sổ, không sửa được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            currActionTask = ActionTask.Edit;
            DsVitual = new DataSet();
            //copy Table[0], Table[1]
            DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());
            DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());
            IsInEditMode.Value = true;
            //THAMTT
            if (IsCheckedPH_HDDT.Value == false)
            {
                txtNgay_ct.IsEnabled = false;
                txtNgay_lct.IsEnabled = false;
                txtMa_qs.IsEnabled = false;
                txtSo_ct.IsEnabled = false;
                txtSo_seri.IsEnabled = false;
                txtMa_nt.IsEnabled = false;
                txtTy_gia.IsEnabled = false;
                txtMa_thue.IsEnabled = false;
                txtThue_suat.IsEnabled = false;
                btnChonHDB.IsEnabled = false;
                txtstatus.Text = "2";
            }
            //ENDTHAMTT
            //UpdateTonKho();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                txtma_gd.IsFocus = true;
            }));
            txtMa_kh.SearchInit();
            txtMa_kh_PreviewLostFocus(null, null);
        }
        #endregion

        #region V_Huy
        private void V_Huy()
        {
            IsInEditMode.Value = false;
            if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
            {
                //Refresh lại khi chọn edit
                switch (currActionTask)
                {
                    #region Edit
                    case ActionTask.Edit:
                        {
                            if (DsVitual != null)
                            {
                                //xóa các row trong table[1], table[2]
                                string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                                // Nên dịch chuyển iRow lùi dòng 0
                                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

                                //Refresh lại grid hoạch toán
                                if (StartUp.DsTrans.Tables[1].Rows.Count > 0)
                                {
                                    //lấy các rowfilter trong grdcp
                                    DataRow[] _row = StartUp.DsTrans.Tables[1].Select("stt_rec='" + stt_rec + "'");
                                    foreach (DataRow dr in _row)
                                    {
                                        //delete các row có trong grdcp
                                        StartUp.DsTrans.Tables[1].Rows.Remove(dr);
                                    }
                                }

                                //Refresh lại table[0]
                                StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;

                                StartUp.DsTrans.Tables[1].Merge(DsVitual.Tables[1]);

                                //RowFilter lại Table[0], Table[1] với iRow trước khi edit
                                StartUp.DataFilter(stt_rec);
                            }

                        }
                        break;
                    #endregion
                    //Refresh lại khi chọn new
                    case ActionTask.Add:
                    case ActionTask.Copy:
                        {
                            Xoa();
                            iRow = iRow_old;
                            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
                        }
                        break;
                }
                IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            }
            TabInfo.SelectedIndex = 0;
            currActionTask = ActionTask.None;

        }
        #endregion

        #region V_Xoa
        void Xoa()
        {
            if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                return;
            if (currActionTask == ActionTask.None || currActionTask == ActionTask.View)
                currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                //Delete tksd13
                StartUpTrans.UpdateTkSd13(1, 0);

                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                StartUp.DeleteVoucher(_stt_rec, txtMa_qs.Text, currActionTask, IsNd51);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi 1 dòng
                // Sau đó RowFilter lại Table[0], Table[1]
                // Rồi mới xóa Table[0]
                //iRow = iRow > 0 ? iRow - 1 : 0;
                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

                //Xóa row table[0]
                StartUp.DsTrans.Tables[0].Rows.RemoveAt(iRow);

                //xóa các row trong Table[1]
                if (StartUp.DsTrans.Tables[1].Rows.Count > 0)
                {
                    DataRow[] rows = StartUp.DsTrans.Tables[1].Select("stt_rec='" + _stt_rec + "'");
                    foreach (DataRow dr in rows)
                    {
                        StartUp.DsTrans.Tables[1].Rows.Remove(dr);
                    }
                }

                //Refresh lại Table[0], Table[1]
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    txtNgay_ct.Text = "";
                    iRow = iRow > StartUp.DsTrans.Tables[0].Rows.Count - 1 ? iRow - 1 : iRow;
                    //load lại form theo stt_rec
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());

                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            currActionTask = ActionTask.None;
        }
        private void V_Xoa()
        {
            //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
            //{
            //    ExMessageBox.Show( 455,StartUp.SysObj, "Không thể xóa chứng từ đã khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            if (IsCheckedPH_HDDT.Value == false)
            {
                ExMessageBox.Show(1450, StartUp.SysObj, "Hóa đơn đã phát hành. Không được xóa!!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            if (!string.IsNullOrEmpty(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim()))
                if (ExMessageBox.Show(391, StartUp.SysObj, "Hóa đơn đã được thanh toán, có muốn xóa phiếu thanh toán hay không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    StartUp.DeletePT(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim(), StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Trim());
                }

            Xoa();

            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

        #region V_HuyHD
        private void V_HuyHD()
        {
            try
            {
                bool isError = false;

                //Kiem tra Qs
                if (C_QS != null && C_NgayHT != null)
                {
                    int _resultCheck = StartUpTrans.CheckQS(C_QS.Text, C_NgayHT.dValue.ToString("yyyyMMdd"), Convert.ToInt16(SysO.UserInfo.Rows[0]["user_id"].ToString()));
                    if (_resultCheck == 1)
                    {
                        ExMessageBox.Show(460, SysO, "Ngày bắt đầu sử dụng quyển sổ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        isError = true;
                    }
                    else if (_resultCheck == 2)
                    {
                        ExMessageBox.Show(465, SysO, "Quyền sử dụng quyển sổ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        isError = true;
                    }
                }
                if (!isError)
                {
                    FrmLogin _frmIn = new FrmLogin();
                    _frmIn.ShowDialog();
                    if (_frmIn.IsLogined)
                    {
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["status"] = 3;
                        //Lưu
                        DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                        tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                        DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                        DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();
                        foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                        {
                            // update thông tin cho các record Table1 (Ct) 
                            drv["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                            drv["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                            drv["ma_ct"] = StartUp.Ma_ct;
                            tbCtToSave.Rows.Add(drv.Row.ItemArray);
                        }

                        if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                        {
                            ExMessageBox.Show(470, StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        ThreadStart _thread = delegate()
                        {
                            Post(0);
                        };
                        new Thread(_thread).Start();
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region V_Nhan
        private void V_Nhan()
        {
            string ma_qs_tam = "";
            DateTime tick = DateTime.Now;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                if (!IsSequenceSave)
                {
                    GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    //Nếu dữ liệu đang sửa bị sai là autocompletetextbox thì ko chi lưu.
                    if (Keyboard.FocusedElement.GetType().Equals(typeof(TextBoxAutoComplete)))
                    {
                        TextBoxAutoComplete txt = Keyboard.FocusedElement as TextBoxAutoComplete;
                        if (txt.ParentControl != null)
                        {
                            if (!txt.ParentControl.CheckLostFocus())
                            {
                                return;
                            }
                        }
                    }
                }
                if (CheckValid())
                {
                    if (!IsSequenceSave)
                    {
                        //Điền thông tin vào 1 số trường khác cho Ph.

                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"] = StartUp.SysObj.GetOption("M_MA_DVCS").ToString();
                        //THAMTT
                        if (currActionTask == ActionTask.Add)
                        {
                            StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct_luu"] = txtSo_ct.Text.ToString().Trim();
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct_luu"] = txtNgay_ct.dValue.Date;
                        }
                        ma_qs_tam = txtMa_qs.Text.ToString().Trim();
                        //ENDTHAMTT

                        // update userId, date, time cho Table0 (Ph)
                        //if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Copy)
                        //{
                        //    StartUp.DsTrans.Tables[0].DefaultView[0]["user_id0"] = StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString();
                        //    StartUp.DsTrans.Tables[0].DefaultView[0]["date0"] = DateTime.Now.Date;
                        //    StartUp.DsTrans.Tables[0].DefaultView[0]["time0"] = DateTime.Now.ToString("HH:mm:ss");
                        //}
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["user_id"] = StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString();
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["user_name"] = StartUp.SysObj.UserInfo.Rows[0]["user_name"].ToString();
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["date"] = DateTime.Now.Date;
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["time"] = DateTime.Now.ToString("HH:mm:ss");
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"];
                        //NewRecord["ten_post"] = StartUp.tbStatus.Select("ma_post =" + StartUp.DmctInfo["ma_post"].ToString())[0]["ten_post"];
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_act"] = StartUp.tbStatus.Select("ma_post =" + txtStatus.SelectedIndex.ToString())[0]["ten_act"];

                        //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");
                    }

                    //Áp giá phiếu xuất

                    DateTime ngay_ct = (DateTime)StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                    object px_gia_dd = StartUp.DsTrans.Tables[0].DefaultView[0]["px_gia_dd"];
                    string ma_nt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                    string ma_kho;
                    string ma_vt;
                    decimal so_luong;


                    if (ngay_ct >= StartUp.ngay_gia_px && px_gia_dd.ToString() != "1")
                    {
                        DataSet dsApgia;
                        SqlCommand cmdApgia = new SqlCommand("Ingia_px");
                        cmdApgia.CommandType = CommandType.StoredProcedure;
                        cmdApgia.Parameters.Add("@Ngay_ct", SqlDbType.SmallDateTime).Value = ngay_ct;
                        cmdApgia.Parameters.Add("@Ma_kho", SqlDbType.VarChar).Value = "";
                        cmdApgia.Parameters.Add("@Ma_vt", SqlDbType.VarChar).Value = "";
                        cmdApgia.Parameters.Add("@So_luong", SqlDbType.Decimal).Value = 0;

                        for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                        {
                            string gia_ton = StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString().Trim();
                            if (gia_ton.Equals("1") || gia_ton.Equals("4"))
                            {
                                ma_kho = StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString();
                                ma_vt = StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString();
                                so_luong = (decimal)StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"];

                                cmdApgia.Parameters["@Ma_kho"].Value = ma_kho;
                                cmdApgia.Parameters["@Ma_vt"].Value = ma_vt;
                                cmdApgia.Parameters["@So_luong"].Value = so_luong;
                                dsApgia = StartUp.SysObj.ExcuteReader(cmdApgia);
                                if (dsApgia == null || dsApgia.Tables.Count == 0 || dsApgia.Tables[0].Rows.Count == 0)
                                    continue;
                                StartUp.DsTrans.Tables[1].DefaultView[i]["gia"] = dsApgia.Tables[0].Rows[0]["gia"];
                                StartUp.DsTrans.Tables[1].DefaultView[i]["tien"] = dsApgia.Tables[0].Rows[0]["tien"];

                                if (StartUp.M_ma_nt0 == ma_nt)
                                {
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"] = dsApgia.Tables[0].Rows[0]["gia"];
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = dsApgia.Tables[0].Rows[0]["tien"];
                                }
                                else
                                {
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"] = dsApgia.Tables[0].Rows[0]["gia_nt"];
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = dsApgia.Tables[0].Rows[0]["tien_nt"];
                                }
                            }
                        }
                    }
                    DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();

                    if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                    {
                        if (!IsSequenceSave)
                        {
                            #region can ban tien
                            //    decimal t_tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"], 0);
                            //    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["ty_gia"], 0);
                            //    decimal t_tien2 = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"], 0);

                            //    decimal t_tien_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km_nt"], 0);
                            //    decimal t_tien_km = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"], 0);

                            //    decimal t_tien2_temp = SysFunc.Round(t_tien_nt2 * ty_gia, StartUp.M_ROUND);
                            //    decimal t_tien_km_temp = SysFunc.Round(t_tien_km_nt * ty_gia, StartUp.M_ROUND);
                            //    bool CanBang = false, CanBangKM = false;
                            //    if (ty_gia == 0 || ChkSua_tien.IsChecked == true)
                            //    {
                            //        CanBang = true;
                            //        CanBangKM = true;
                            //    }
                            //    for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && (!CanBang || !CanBangKM); i++)
                            //    {
                            //        decimal tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"], 0);
                            //        decimal tien2 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"], 0);
                            //        int km_ck = ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"], 0);
                            //        if (tien_nt2 != 0 && tien2 != 0)
                            //        {
                            //            if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"], 0) == 0)
                            //            {
                            //                StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"] = tien2 + (t_tien2_temp - t_tien2);
                            //                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = t_tien2_temp;
                            //                CanBang = true;
                            //            }
                            //            else
                            //            {
                            //                StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"] = tien2 + (t_tien_km_temp - t_tien_km);
                            //                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"] = t_tien_km_temp;
                            //                CanBangKM = true;
                            //            }
                            //        }
                            //    } 
                            #endregion

                            // phan bo tien thue vao ct
                            PhanBoThueInCT_CBTien();
                        }
                        tbCtToSave = StartUp.DsTrans.Tables[1].DefaultView.ToTable().Copy();
                    }

                    DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                    tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                    if (!IsSequenceSave)
                    {
                        tbPhToSave.Rows[0]["status"] = 0;
                    }
                    DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");
                    for (int i = 0; i < tbCtToSave.Rows.Count; i++)
                    {
                        tbCtToSave.Rows[i]["dvt1"] = tbCtToSave.Rows[i]["dvt"];
                    }
                    if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        ExMessageBox.Show(475, StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    //StartUp.UpdateRates(tbPhToSave.Rows[0]["ma_nt"].ToString(), Convert.ToDateTime(txtNgay_ct.Value).Date, Convert.ToDecimal(txtTy_gia.Value));
                    bool isError = false;
                    #region
                    if (!IsSequenceSave)
                    {
                        if (!isError)
                        {
                            //if (dsCheckData == null || dsCheckData.Tables[0].Rows.Count == 0)
                            dsCheckData = StartUp.CheckData(currActionTask == ActionTask.Edit ? 0 : 1);

                            dsCheckData.Tables[0].AcceptChanges();
                            if (dsCheckData.Tables.Count > 0)
                            {
                                bool ismessage = false;
                                string ma_vt_xuat_am = "";
                                foreach (DataRowView dv in dsCheckData.Tables[0].DefaultView)
                                {
                                    if (isError)
                                        break;
                                    string result = dv[0].ToString();
                                    //Số ct trùng
                                    switch (result)
                                    {
                                        case "PH01":
                                            {
                                                if (StartUp.M_trung_so.Equals("1"))
                                                {
                                                    if (ExMessageBox.Show(480, StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                    {
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                else if (StartUp.M_trung_so.Equals("2"))
                                                {
                                                    ExMessageBox.Show(485, StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
                                                }
                                            }
                                            break;
                                        case "PH02":
                                            {
                                                if (currActionTask != ActionTask.Edit)
                                                {
                                                    if (dsCheckData.Tables.Count > 1)
                                                    {
                                                        int isresult = Convert.ToInt32(dsCheckData.Tables[1].Rows[0][0]);
                                                        string ma_qs = dsCheckData.Tables[2].Rows[0][0].ToString().Trim();
                                                        int index = Convert.ToInt32(dsCheckData.Tables[3].Rows[0][0]);

                                                        if (isresult == 1)
                                                        {
                                                            ExMessageBox.Show(490, SysO, "Ngày bắt đầu sử dụng ký hiệu hóa đơn không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                            txtNgay_ct.Focus();
                                                            isError = true;
                                                        }
                                                        else if (isresult == 2)
                                                        {
                                                            ExMessageBox.Show(495, SysO, "Quyền sử dụng ký hiệu hóa đơn không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                            txtMa_qs.IsFocus = true;
                                                            isError = true;
                                                        }
                                                        else if (isresult == 3)
                                                        {
                                                            string ten_tthd = dsCheckData.Tables[4].Rows[0]["ten_tthd"].ToString().Trim().ToLower();
                                                            ExMessageBox.Show(500, SysO, "Số hóa đơn của ký hiệu " + "[" + txtMa_qs.Text.Trim() + "]" + " đã " + "[" + ten_tthd + "]" + "!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                                                            isError = true;
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        case "PH03":
                                            {
                                                ExMessageBox.Show(505, StartUp.SysObj, "Số hóa đơn không liên tục!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                                            }
                                            break;
                                        case "PH04":
                                            {
                                                ExMessageBox.Show(510, StartUp.SysObj, "Mã nx là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                txtMa_nx.IsFocus = true;
                                            }
                                            break;
                                        case "CT01":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show(515, StartUp.SysObj, "Tk dt là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_dt"];
                                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                {
                                                    GrdCt.Focus();
                                                }));
                                                break;
                                            }
                                        case "CT02":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show(520, StartUp.SysObj, "Tk kho là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                {
                                                    GrdCt.Focus();
                                                }));
                                                return;
                                            }
                                        case "CT03":
                                            {
                                                StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ton13"] = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_kho_i"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vt"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vv_i"].ToString());
                                                //int index = Convert.ToInt16(dv[1]);
                                                if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["vt_ton_kho"], 0) == 1)
                                                {
                                                    //144089096 akhai yêu cầu sửa chỉ hiện ma_vt 
                                                    if (!ma_vt_xuat_am.Contains(StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vt"].ToString().Trim()))
                                                        ma_vt_xuat_am = ma_vt_xuat_am + StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vt"].ToString().Trim() + ", ";
                                                }
                                            }
                                            break;
                                        case "CT04":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show(535, StartUp.SysObj, "Tk gv là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_gv"];
                                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                {
                                                    GrdCt.Focus();
                                                }));
                                                break;
                                            }
                                        case "CT05":
                                            {
                                                if (StartUp.M_AR_CK == 1)
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show(540, StartUp.SysObj, "Tk c.khấu là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_ck"];
                                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                    {
                                                        GrdCt.Focus();
                                                    }));
                                                }
                                                break;
                                            }
                                        case "CT06":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show(545, StartUp.SysObj, "Tk cp km là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_km_i"];
                                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                {
                                                    GrdCt.Focus();
                                                }));
                                                break;
                                            }
                                    }
                                    dsCheckData.Tables[0].Rows.Remove(dv.Row);
                                }
                                if (!string.IsNullOrEmpty(ma_vt_xuat_am))
                                {
                                    if (StartUp.M_CHK_TON_VT.Equals("2"))
                                    {
                                        ExMessageBox.Show(525, StartUp.SysObj, "Có vật tư [" + ma_vt_xuat_am.Substring(0, ma_vt_xuat_am.Length - 2) + "] xuất âm hoặc tồn kho nhỏ hơn tồn tối thiếu, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                        isError = true;
                                    }
                                    else if (StartUp.M_CHK_TON_VT.Equals("1"))
                                    {
                                        ExMessageBox.Show(530, StartUp.SysObj, "Có vật tư [" + ma_vt_xuat_am.Substring(0, ma_vt_xuat_am.Length - 2) + "] xuất âm hoặc tồn kho nhỏ hơn tồn tối thiếu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }
                            }
                        }
                        #region
                        if (!isError && IsNd51)
                        {
                            if (txtMa_qs.RowResult == null)
                                txtMa_qs.SearchInit();
                            UpdateNewSoCt(SysO, txtMa_qs.Text);

                            if (currActionTask == ActionTask.Edit && txtMa_qs.RowResult != null)
                            {
                                UpdateNewNgayCt(SysO, txtMa_qs.Text, GetCurrentSo_ct(txtMa_qs.RowResult["transform"].ToString(), txtSo_ct.Text.Trim()));
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region "Tao phieu thu tu dong"
                    if (!isError)
                    {
                        bool _createPT1 = false;
                        string newstt_recPt1 = "";
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ma_ct", typeof(string));
                        dt.Columns.Add("stt_rec", typeof(string));
                        dt.Columns.Add("stt_recPT", typeof(string));
                        dt.Columns.Add("ma_qs", typeof(string));
                        dt.Columns.Add("so_ct", typeof(string));
                        dt.Columns.Add("ma_nt", typeof(string));
                        dt.Columns.Add("ty_gia", typeof(decimal));
                        dt.Columns.Add("ty_giaf", typeof(decimal));
                        dt.Columns.Add("nguoinop", typeof(string));
                        dt.Columns.Add("lydonop", typeof(string));
                        dt.Columns.Add("ma_gd", typeof(string));

                        string menu_idPT1 = BindingSysObj.ExcuteScalar(new SqlCommand("Select top 1 menu_id From command Where ma_ct like 'PT1'")).ToString().Trim();
                        if (SmLib.SysFunc.CheckPermission(BindingSysObj, ActionTask.Add, menu_idPT1))
                        {
                            if (StartUp.DsTrans.Tables[0].DefaultView[0]["status"].ToString().Equals("2"))
                            {
                                bool _isTienMat = false;
                                string[] _ds_tkkt = BindingSysObj.GetOption("M_TK_TK_VT").ToString().Split(',');
                                foreach (string tk in _ds_tkkt)
                                {
                                    if (txtMa_nx.Text.Trim().StartsWith(tk.ToString().Trim()))
                                        _isTienMat = true;
                                }
                                if (_isTienMat)
                                {
                                    switch (BindingSysObj.GetOption("M_TAO_PT_TM").ToString())
                                    {
                                        case "1":
                                            if (ExMessageBox.Show(696, StartUp.SysObj, "Có tạo phiếu thu tiền ngay cho hóa đơn bán hàng?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                                            {
                                                _createPT1 = true;
                                            }
                                            break;
                                        case "2":
                                            _createPT1 = true;
                                            break;
                                    }
                                    if (_createPT1)
                                    {
                                        DataTable tbInfoPT = null;
                                        if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString()))
                                        {
                                            SqlCommand _cmd = new SqlCommand();
                                            _cmd.CommandText = string.Format("SELECT stt_rec,ma_ct,ma_gd,ma_qs,so_ct,ma_nt,ong_ba,dien_giai FROM {0} WHERE stt_rec LIKE '{1}'", StartUp.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Equals("PT1") ? "ph41" : "ph51", StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString());
                                            tbInfoPT = BindingSysObj.ExcuteReader(_cmd).Tables[0];
                                            if (tbInfoPT.Rows.Count == 1)
                                                StartUp.DeletePT(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim(), StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Trim());
                                        }

                                        int _isTk = 0;
                                        //Nếu tiền thanh toán > M_MUC_TIEN_PT1 thì tự động tạo bc1
                                        if (txtT_tt.nValue > Convert.ToDecimal(BindingSysObj.GetSysVar("M_MUC_TIEN_PT1")))
                                        {
                                            _isTk = 1;
                                        }
                                        else
                                        {
                                            //nếu là tk ngân hàng mặc định tạo phiếu báo có
                                            SqlCommand _cmd = new SqlCommand();
                                            _cmd.CommandText = "SELECT COUNT(1) FROM dmtknh WHERE tk LIKE @tk";
                                            _cmd.Parameters.Add(new SqlParameter("@tk", SqlDbType.VarChar)).Value = txtMa_nx.Text.Trim();
                                            _isTk = (int)BindingSysObj.ExcuteScalar(_cmd);
                                        }

                                        DataRow _dr = dt.NewRow();
                                        dt.Rows.Add(_dr);

                                        FrmTaoPT _form = new FrmTaoPT();
                                        _form.tbInfoPT = tbInfoPT;
                                        _form.DataContext = dt.DefaultView;
                                        _form.txtMa_qs_pt.Text = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_qs_pt"].ToString();
                                        _form.txtso_ct_pt.Text = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"].ToString();
                                        _form.txtnguoi_nop.Text = txtOng_ba.Text;
                                        _form.kind = (_isTk > 0 ? 2 : 1);
                                        _form.Ma_nt_ht = txtMa_nt.Text;
                                        _form.so_hd = txtSo_ct.Text.Trim();
                                        _form.ngay_hd = txtNgay_ct.dValue.ToShortDateString();
                                        _form.filterma_qs = txtMa_qs.Filter;
                                        _form.ShowDialog();
                                        if (!_form.isOk)
                                            _createPT1 = false;
                                        else
                                        {
                                            dt = dt.Copy();

                                            if (_form.txtKind.Text.Equals("1"))
                                                newstt_recPt1 = DataProvider.NewTrans(StartUp.SysObj, "PT1", StartUp.Ws_Id);
                                            else
                                                newstt_recPt1 = DataProvider.NewTrans(StartUp.SysObj, "BC1", StartUp.Ws_Id);
                                            dt.Rows[0]["ma_ct"] = (_form.txtKind.Text.Equals("1") ? "PT1" : "BC1");
                                            dt.Rows[0]["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
                                            dt.Rows[0]["stt_recPT"] = newstt_recPt1;
                                            dt.Rows[0]["ma_qs"] = _form.txtMa_qs_pt.Text;
                                            dt.Rows[0]["so_ct"] = _form.txtso_ct_pt.Text.PadLeft(_form.txtso_ct_pt.MaxLength, ' ');
                                            dt.Rows[0]["ma_nt"] = _form.txtMa_nt.Text;
                                            dt.Rows[0]["ty_gia"] = _form.txtMa_nt.Text.Equals(StartUp.M_MA_NT0) ? 1 : txtTy_gia.Rate;
                                            dt.Rows[0]["ty_giaf"] = _form.txtMa_nt.Text.Equals(StartUp.M_MA_NT0) ? 1 : txtTy_gia.RateF;
                                            dt.Rows[0]["nguoinop"] = _form.txtnguoi_nop.Text;
                                            dt.Rows[0]["lydonop"] = _form.txtlydo_nop.Text;
                                            dt.Rows[0]["ma_gd"] = _form.txtMa_gd.Text;

                                            //Cap nhat lai dstrans
                                            StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_qs_pt"] = _form.txtMa_qs_pt.Text;
                                            StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"] = _form.txtso_ct_pt.Text;
                                            StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"] = (_form.txtKind.Text.Equals("1") ? "PT1" : "BC1");
                                            StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"] = newstt_recPt1;
                                        }
                                    }
                                }
                            }
                        }

                        string _stt_rec1 = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                        ThreadStart _thread = delegate()
                        {
                            //Delete tksd13

                            //post
                            Post(_createPT1 ? 1 : 0);
                            if (_createPT1)
                            {
                                CreatePT1(dt);
                            }

                            if (!IsSequenceSave)
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                        new Action(() =>
                                        {
                                            if (StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString().Equals(_stt_rec1))
                                            {
                                                //update ton kho 
                                                UpdateTonKho();

                                                //update so du kh
                                                LoadDataDu13();
                                            }
                                            if (_createPT1)
                                                if (!string.IsNullOrEmpty(newstt_recPt1))
                                                {
                                                    DataRow[] drs = StartUp.DsTrans.Tables[0].Select("stt_rec = '" + _stt_rec1 + "'");
                                                    if (drs.Length == 1)
                                                    {
                                                        drs[0]["stt_rec_pt"] = newstt_recPt1;
                                                        drs[0]["so_ct_pt"] = dt.Rows[0]["so_ct"];
                                                        drs[0]["ma_ct_pt"] = dt.Rows[0]["ma_ct"];
                                                    }
                                                }
                                        }));
                            }
                        };
                        new Thread(_thread).Start();
                        if (!IsSequenceSave)
                        {
                            //Update lại thứ tự các chứng từ
                            int iRowNew = GetiRow(StartUp.DsTrans.Tables[0], StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString());
                            if (iRow != iRowNew)
                            {
                                DataRow oldRow = StartUp.DsTrans.Tables[0].DefaultView[0].Row;
                                DataRow newRow = StartUp.DsTrans.Tables[0].NewRow();
                                newRow.ItemArray = oldRow.ItemArray;
                                if (iRow > iRowNew)
                                    StartUp.DsTrans.Tables[0].Rows.InsertAt(newRow, iRowNew);
                                else
                                    StartUp.DsTrans.Tables[0].Rows.InsertAt(newRow, iRowNew + 1);
                                StartUp.DsTrans.Tables[0].AcceptChanges();
                                StartUp.DsTrans.Tables[0].Rows.Remove(oldRow);
                                StartUp.DsTrans.Tables[0].AcceptChanges();
                                iRow = iRowNew;
                            }


                            currActionTask = ActionTask.None;
                            IsInEditMode.Value = false;
                        }
                    }
                    #endregion
                    //THAMTT
                    #region 'Phat hanh hoa don dien tu'
                    if (!isError)
                    {
                        if (M_HDDT_DUYET == "0")
                        {
                            //Gọi Phát hành hóa đơn điện tử => Trả lại kết quả:
                            //Update Ph81: 
                            //Truong hop 1: Lập mới phát hành
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            if (flag_button == "3")
                            {
                                //Goi Dieu chinh tang hoa don
                                ////Update Ph81: 
                                // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                                // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                                // + Update tt_hddt_tcn của hóa đơn điều chỉnh/thay thế về trạng thái = 3 lọc theo stt_rec_tt 
                            }
                            if (flag_button == "4")
                            {
                                //Goi Dieu chinh giảm hoa don
                                ////Update Ph81: 
                                // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                                // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                                // + Update tt_hddt_tcn của hóa đơn điều chỉnh/thay thế về trạng thái = 4 lọc theo stt_rec_tt 
                            }
                            if (flag_button == "5")
                            {
                                //goi Thay the hoa don
                                // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                                // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                                // + Update tt_hddt_tcn của hóa đơn thay thế về trạng thái = 8 - Hủy hóa đơn tương ứng lọc theo stt_rec_tt

                            }

                        }
                        if (flag_button == "2" || M_tt_hddt_tcn != "0")
                        {
                            //Goi su kien dieu chinh thong tin hoa don 
                            //Truong hop 3: Dieu chinh thong tin hoa don
                            //+ update tt_hddt_tcn = "2" - Dieu chinh thong tin
                        }
                    }
                    #endregion
                    //ENDTHAMTT
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

            sw.Stop();
            Debug.WriteLine(string.Format("==================================================================", tick.ToString("hh:mm:ss"), DateTime.Now.ToString("hh:mm:ss"), sw.ElapsedMilliseconds));
            Debug.WriteLine(string.Format("================= // {0} - {1}: {2} // ===========================", tick.ToString("hh:mm:ss"), DateTime.Now.ToString("hh:mm:ss"), sw.ElapsedMilliseconds));
            Debug.WriteLine(string.Format("==================================================================", tick.ToString("hh:mm:ss"), DateTime.Now.ToString("hh:mm:ss"), sw.ElapsedMilliseconds));
        }

        void CreatePT1(DataTable dt)
        {
            try
            {

                SqlCommand cmd = new SqlCommand("exec [dbo].[SOCTHDA-CREATEPT1] @Stt_rec, @Stt_recPT, @ma_qs, @so_ct, @ma_nt, @ty_gia, @ty_giaf, @nguoinop, @lydonop, @ma_gd, @ma_ct");
                cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = dt.Rows[0]["stt_rec"];
                cmd.Parameters.Add("@Stt_recPT", SqlDbType.VarChar).Value = dt.Rows[0]["stt_recPT"];
                cmd.Parameters.Add("@ma_qs", SqlDbType.VarChar).Value = dt.Rows[0]["ma_qs"];
                cmd.Parameters.Add("@so_ct", SqlDbType.VarChar).Value = dt.Rows[0]["so_ct"];
                cmd.Parameters.Add("@ma_nt", SqlDbType.VarChar).Value = dt.Rows[0]["ma_nt"];
                cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = dt.Rows[0]["ty_gia"];
                cmd.Parameters.Add("@ty_giaf", SqlDbType.Decimal).Value = dt.Rows[0]["ty_giaf"];
                cmd.Parameters.Add("@nguoinop", SqlDbType.NVarChar).Value = dt.Rows[0]["nguoinop"];
                cmd.Parameters.Add("@lydonop", SqlDbType.NVarChar).Value = dt.Rows[0]["lydonop"];
                cmd.Parameters.Add("@ma_gd", SqlDbType.VarChar).Value = dt.Rows[0]["ma_gd"];
                cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = dt.Rows[0]["ma_ct"];
                StartUp.SysObj.ExcuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void Post(int ispostck)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[SOCTHDA-Post] @stt_rec,0,@IsHasPT1");
            cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            cmd.Parameters.Add("@IsHasPT1", SqlDbType.Int).Value = ispostck;
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }

        #endregion

        #region CheckValid
        bool CheckValid()
        {

            bool result = true;
            if (!IsSequenceSave)
            {
                string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                int sua_tien = 0;
                int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);

                if (IsInEditMode.Value == true)
                {
                    #region ma_gd
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()) && result == true)
                    {
                        ExMessageBox.Show(550, StartUp.SysObj, "Chưa vào loại hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtma_gd.IsFocus = true;
                    }
                    #endregion

                    #region ma_kh
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString()) && result == true)
                    {
                        ExMessageBox.Show(555, StartUp.SysObj, "Chưa vào mã khách hàng!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_kh.IsFocus = true;
                    }
                    #endregion

                    #region ma_nx
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString()) && result == true)
                    {
                        ExMessageBox.Show(560, StartUp.SysObj, "Chưa vào mã nx!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_nx.IsFocus = true;
                    }
                    //if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString()) && result == true)
                    //{
                    //    SqlCommand cmd = new SqlCommand("select count(ma_nx) from dmnx where ma_nx=@ma_nx");
                    //    cmd.Parameters.Add("@ma_nx", SqlDbType.Char).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString().Trim();
                    //    if ((int)StartUp.SysObj.ExcuteScalar(cmd) == 0)
                    //    {
                    //        ExMessageBox.Show( 565,StartUp.SysObj, "Mã nx là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    //        result = false;
                    //        txtMa_nx.IsFocus = true;
                    //    }
                    //}
                    #endregion

                    #region ngay_ct
                    if ((txtNgay_ct.Value == null || txtNgay_ct.Value.ToString() == "") && result == true)
                    {
                        ExMessageBox.Show(570, StartUp.SysObj, "Chưa vào ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtNgay_ct.Focus();
                    }
                    if (txtNgay_ct.Value.ToString() != "" && result == true)
                    {
                        if (!txtNgay_ct.IsValueValid && result == true)
                        {
                            ExMessageBox.Show(575, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                        if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct.dValue)) && result == true)
                        {
                            ExMessageBox.Show(580, StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                        if (result && Convert.ToDateTime(txtNgay_ct.dValue) < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                        {
                            ExMessageBox.Show(585, StartUp.SysObj, "Ngày hạch toán phải sau ngày mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }

                        if (result && StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                        {
                            ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                    }
                    #endregion

                    #region ngay_lct
                    if (result && StartUp.M_ngay_lct.Equals("1"))
                    {
                        if (txtNgay_lct.Value == null || txtNgay_lct.Value.ToString() == "")
                        {
                            ExMessageBox.Show(590, StartUp.SysObj, "Chưa vào ngày lập px!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_lct.Focus();
                            return false;
                        }
                        if (!txtNgay_lct.IsValueValid)
                        {
                            ExMessageBox.Show(595, StartUp.SysObj, "Ngày lập px không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_lct.Focus();
                            return false;
                        }
                        //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_lct.dValue)))
                        //{
                        //    ExMessageBox.Show( 600,StartUp.SysObj, "Ngày lập px phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    txtNgay_lct.Focus();
                        //    return false;
                        //}
                    }
                    #endregion

                    #region ma_qs
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString()) && result == true)
                    {
                        ExMessageBox.Show(610, StartUp.SysObj, "Chưa vào ký hiệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_qs.IsFocus = true;
                    }
                    #endregion

                    #region Cục thuế
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_co_cn"].ToString().Equals("1") && string.IsNullOrEmpty(txtMa_kh2.Text.Trim()))
                    {
                        ExMessageBox.Show(615, StartUp.SysObj, "Chưa vào cục thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_kh2.IsFocus = true;
                    }
                    #endregion

                    #region chi tiet HT
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0 && result == true)
                    {
                        ExMessageBox.Show(620, StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D1);
                        GrdCt_AddNewRecord(null, null);
                        GrdCt.ActiveCell = (GrdCt.Records[0] as DataRecord).Cells["ma_vt"];
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            GrdCt.Focus();
                        }));
                    }
                    #endregion


                    #region kiểm tra ma_vt, ma_kho_i, tk_dt, tk_vt, tk_gv, tk_ck, tk_km
                    for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && result == true; i++)
                    {
                        StartUp.DsTrans.Tables[1].DefaultView[i]["ma_ct"] = StartUp.Ma_ct;
                        StartUp.DsTrans.Tables[1].DefaultView[i]["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                        StartUp.DsTrans.Tables[1].DefaultView[i]["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];

                        #region kiểm tra ma_vt, ma_kho_i
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim()))
                        {
                            ExMessageBox.Show(625, StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_vt"];


                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }
                        if (result == true && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim()))
                        {
                            ExMessageBox.Show(630, StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_kho_i"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }



                        #endregion

                        #region kiểm tra tk_dt
                        if (result && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_dt"].ToString().Trim()))
                        {
                            ExMessageBox.Show(645, StartUp.SysObj, "Chưa vào tk doanh thu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_dt"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }
                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_dt"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_dt"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 650,StartUp.SysObj, "Tk dt là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_dt"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion



                        #region kiểm tra tk_gv
                        if (result && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_gv"].ToString().Trim()))
                        {
                            ExMessageBox.Show(665, StartUp.SysObj, "Chưa vào tk giá vốn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_gv"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }
                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_gv"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_gv"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 670,StartUp.SysObj, "Tk gv là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_gv"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion

                        #region tk_ck
                        if (result && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                        {
                            //146279846
                            //if (StartUp.M_AR_CK == 1 && ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tl_ck"], 0) != 0)
                            //{
                            //    ExMessageBox.Show( 675,StartUp.SysObj, "Chưa vào tk c.khấu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            //    result = false;
                            //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_ck"];
                            //    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            //    {
                            //        GrdCt.Focus();
                            //    }));
                            //}
                        }

                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 680,StartUp.SysObj, "Tk c.khấu là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_ck"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion

                        #region tk_km
                        if (result && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                        {
                            if (StartUp.M_KM_CK == 1 && StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"].ToString().Trim() == "1")
                            {
                                ExMessageBox.Show(685, StartUp.SysObj, "Chưa vào tk cp km!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                result = false;
                                GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_km_i"];
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    GrdCt.Focus();
                                }));
                            }
                        }
                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 690,StartUp.SysObj, "Tk cp km là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_km_i"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion
                        #region so_luong
                        if (result)
                        {
                            if (int.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString()) == 3)
                            {
                                if (decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"].ToString()) == 0)
                                {
                                    ExMessageBox.Show(695, StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    result = false;
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["so_luong"];
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        GrdCt.Focus();
                                    }));
                                }
                            }
                        }
                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 700,StartUp.SysObj, "Tk cp km là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_km_i"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion
                    }
                    #endregion

                    txtMa_kh.SearchInit();


                    #region ma_so_thue, ten_kh, dia_chi để trống
                    if (result)
                    {
                        FrmKHInfo formKh = null;
                        if (txtMa_kh.RowResult == null)
                            txtMa_kh.SearchInit();
                        if (txtMa_kh.RowResult != null && (string.IsNullOrEmpty(txtMa_kh.RowResult["ma_so_thue"].ToString().Trim()) ||
                            string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()) ||
                            string.IsNullOrEmpty(txtMa_kh.RowResult["ten_kh"].ToString().Trim())) && result == true)
                        {
                            formKh = new FrmKHInfo();
                            formKh.ShowDialog();
                            string ma_so_thue = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"].ToString().Trim();
                            if (formKh.isError && ma_so_thue != "")
                            {
                                if (!SmLib.SysFunc.CheckSumMaSoThue(ma_so_thue))
                                {
                                    if (StartUp.M_MST_CHECK.Equals("1"))
                                        ExMessageBox.Show(705, StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                                    else
                                    {
                                        ExMessageBox.Show(710, StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        result = false;
                                        txtma_so_thue.Focus();
                                    }
                                }
                            }

                            string ten_kh = StartUp.DsTrans.Tables[0].DefaultView[0][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"].ToString().Trim();
                            if (ten_kh == "")
                            {
                                txtMa_kh.SearchInit();
                                txtMa_kh_PreviewLostFocus(txtMa_kh, null);
                            }
                        }
                    }
                    #endregion

                    /* #region ma_so_thue
                    if (result && formKh == null &&
                        !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue_dmkh"].ToString().Trim()))
                    {
                        if (!StartUp.M_MST_CHECK.Equals("0"))
                        {
                            if (!SmLib.SysFunc.CheckSumMaSoThue(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue_dmkh"].ToString().Trim()))
                            {

                                formKh = new FrmKHInfo();
                                formKh.ShowDialog();
                                string ma_so_thue = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"].ToString().Trim();
                                if (formKh.isError && ma_so_thue != "")
                                {
                                    if (!SmLib.SysFunc.CheckSumMaSoThue(ma_so_thue))
                                    {
                                        if (StartUp.M_MST_CHECK.Equals("1"))
                                            ExMessageBox.Show( 715,StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                                        else//if (StartUp.M_MST_CHECK.Equals("2"))
                                        {
                                            ExMessageBox.Show( 720,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            result = false;
                                            txtma_so_thue.Focus();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion*/

                    #region tk_thue_no
                    if (result && txtTk_thue_no.Text == "")
                    {
                        ExMessageBox.Show(725, StartUp.SysObj, "Chưa vào tk thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtTk_thue_no.IsFocus = true;
                    }
                    if (result && !txtTk_thue_no.CheckLostFocus())
                    {
                        ExMessageBox.Show(730, StartUp.SysObj, "Tk thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtTk_thue_no.IsFocus = true;
                    }
                    #endregion

                    #region tk_thue_co
                    if (result && txtTk_thue_co.Text == "")
                    {
                        ExMessageBox.Show(735, StartUp.SysObj, "Chưa vào tk thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_thue.IsFocus = true;
                    }
                    if (result && !txtTk_thue_co.CheckLostFocus())
                    {
                        ExMessageBox.Show(740, StartUp.SysObj, "Tk thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtTk_thue_co.IsFocus = true;
                    }
                    #endregion

                    #region so_ct
                    if (!IsNd51)
                    {
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()) && result == true)
                        {
                            ExMessageBox.Show(745, StartUp.SysObj, "Chưa vào số chứng từ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtSo_ct.Focus();
                        }
                        //Kiểm tra so_ct có thuộc quyển ct không
                        if (result && !CheckSo_ct(StartUp.DsTrans.Tables[0].Rows[iRow]["transform"].ToString(), ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["so_ct1"], 0), ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["so_ct2"], 0), txtSo_ct.Text.Trim()))
                        {
                            ExMessageBox.Show(750, StartUp.SysObj, "Hóa đơn không thuộc quyển chứng từ hiện hành!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtSo_ct.SelectAll();
                            txtSo_ct.Focus();
                            result = false;
                        }
                        //if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, stt_rec) && result == true)
                        //{
                        //    if (StartUp.DmctInfo["m_trung_so"].ToString().Equals("1"))
                        //    {
                        //        if (ExMessageBox.Show( 755,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        //        {
                        //            result = false;
                        //            txtSo_ct.SelectAll();
                        //            txtSo_ct.Focus();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        ExMessageBox.Show( 760,StartUp.SysObj, "Số chứng từ đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        txtSo_ct.SelectAll();
                        //        txtSo_ct.Focus();
                        //    }
                        //}
                    }
                    else
                    {
                        if (result)
                            txtMa_qs.SearchInit();
                        ////http://forum.fast.com.vn/showthread.php?t=11511&p=123168#post123168
                        //if (result && StartUp.DsTrans.Tables[1].DefaultView.Count > Convert.ToDecimal(StartUp.DmctInfo["so_dong_in"]))
                        //{
                        //    ExMessageBox.Show( 765,StartUp.SysObj, "Số dòng chi tiết không được nhiều hơn số dòng in ngầm định!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    result = false;
                        //}

                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()) && result == true)
                        {
                            ExMessageBox.Show(770, StartUp.SysObj, "Chưa vào số hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        }
                        //Kiểm tra so_ct có thuộc quyển ct không
                        if (result == true && !CheckSo_ct(StartUp.DsTrans.Tables[0].Rows[iRow]["transform"].ToString(), ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["so_ct1"], 0), ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["so_ct2"], 0), txtSo_ct.Text.Trim()))
                        {
                            ExMessageBox.Show(775, StartUp.SysObj, "Số hóa đơn không thuộc ký hiệu hiện hành!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                            result = false;
                        }
                        //if (result == true && CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, stt_rec))
                        //{
                        //    ExMessageBox.Show( 780,StartUp.SysObj, "Số hóa đơn đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    result = false;
                        //    txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        //}
                        //else if (result == true && currActionTask != ActionTask.Edit && !CheckValidSoctLT(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text))
                        //{
                        //    ExMessageBox.Show( 785,StartUp.SysObj, "Số hóa đơn không liên tục!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    result = false;
                        //    txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        //}
                    }
                    #endregion
                }
            }
            return result;
        }
        #endregion

        #region V_Copy
        FrmCopy _formcopy = null;
        private void V_Copy()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Copy;
            _formcopy = new FrmCopy();
            SmLib.SysFunc.LoadIcon(_formcopy);

            DsVitual = null;

            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();

        }

        void _formcopy_Closed(object sender, EventArgs e)
        {
            if (_formcopy.isCopy == true)
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["stt_rec_pt"] = "";
                    NewRecord["so_ct_pt"] = "";
                    NewRecord["ma_ct_pt"] = "";
                    NewRecord["ngay_ct"] = _formcopy.ngay_ct;
                    //NewRecord["ngay_lct"] = _formcopy.ngay_ct;
                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["ten_post"] = StartUp.tbStatus.Select("ma_post =" + StartUp.DmctInfo["ma_post"].ToString())[0]["ten_post"];
                    if (StartUp.M_ngay_lct.Trim().Equals("0"))
                    {
                        NewRecord["ngay_lct"] = _formcopy.ngay_ct;
                    }
                    //THAMTT
                    NewRecord["ma_qs"] = GetDMQS_HD(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                             StartUp.M_User_Id, "0", NewRecord["ma_qs"].ToString().Trim());
                    NewRecord["tt_hddt_tcn"] = "0";
                    NewRecord["user_id_ph"] = StartUp.M_User_Id;
                    NewRecord["ngay_ct_luu"] = _formcopy.ngay_ct;
                    NewRecord["flag_btn"] = "0";
                    //ENDTHAMTT
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";
                    NewRecord["so_cttmp"] = NewRecord["so_ct"];
                    NewRecord["sl_in"] = 0;

                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    //add các row trong GrdCp
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                    {
                        //lấy các rowfilter trong grdcp
                        DataRow[] _row = StartUp.DsTrans.Tables[1].Select("stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        DataRow NewCtRecord;
                        foreach (DataRow dr in _row)
                        {
                            //add 
                            NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                            NewCtRecord.ItemArray = dr.ItemArray;
                            NewCtRecord["stt_rec"] = newSttRec;
                            StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);
                        }
                    }

                    iRow_old = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    //load lại form
                    StartUp.DataFilter(newSttRec);

                    //IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

                    IsInEditMode.Value = true;
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        txtma_gd.IsFocus = true;
                    }));
                }
            }
        }

        private object GetDMQS_HD1(SysLib.SysObject BindingSysObj, string p, DateTime dateTime, int p_4, string p_5)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region V_Xem
        private void V_Xem()
        {
            currActionTask = ActionTask.View;
            //  set lai stringbrowse 
            //string stringBrowse1 = "ngay_ct:100:h=Ngày c.từ:FL:D;so_ct:70:h=Số c.từ:FL;so_seri:70:h=Số seri;"+
            //    "ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;dien_giai:225:h=Diễn giải;"+
            //    "ma_bp:100:h=NVBH;t_tien_nt2:130:h=Tiền hàng nt:N1:S;t_thue_nt:130:h=Tiền thuế nt:N1:S;"+
            //    "t_tt_nt:130:h=Tổng tt nt:N1:S;ma_nx:80:h=Mã nx;thue_suat:80:h=Thuế suất:F=2;tk_thue_co:80:h=Tk thuế;"+
            //    "t_tien2:130:h=Tiền hàng:N0:S;t_thue:130:h=Tiền thuế:N0:S;t_tt:130:h=Tổng tt:N0:S;ma_nt:100:h=Mã nt;"+
            //    "ty_gia:130:h=Tỷ giá:R;date:105:h=Ngày cập nhật:D;time:100:h=Giờ cập nhật;"+
            //    "user_id:100:h=Số hiệu NSD:N;user_name:100:h=Tên NSD";

            //string stringBrowse2 = "ma_vt:100:h=Mã vật tư:FL;ten_vt:270:h=Tên vật tư:FL;dvt1:50:h=Ðvt;"+
            //    "ma_kho_i:70:h=Mã kho;so_luong:130:h=Số lượng:Q:S;gia_nt2:130:h=Giá bán nt:P1;tien_nt2:130:h=Thành tiền nt:N1:S;"+
            //    "tk_dt:80:h=Tk dt;gia_nt:130:h=Giá vốn nt:P1;tien_nt:130:h=Tiền vốn nt:N1:S;tk_vt:80:h=Tk kho;"+
            //    "tk_gv:80:h=Tk gv;gia2:130:h=Giá bán:P0;tien2:130:h=Thành tiền:N0:S;"+
            //    "gia:130:h=Giá vốn:N1;tien:130:h=Tiền vốn:N0:S";

            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
            _frmView.ListFieldSum = "t_tt_nt;t_tt";
            _frmView.TongCongLabel = "Tổng thanh toán";
            _frmView.frmBrw.Title = StartUp.M_Tilte;
            //Them cac truong tu do
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);
            _frmView.frmBrw.LanguageID = "TT_SOCTHDA_HDDT_9";
            _frmView.ShowDialog();
            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {

                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmTT_SOCTHDA_HDDT.iRow = select_irow + 1;
                    StartUp.DataFilter(selected_stt_rec);
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

                }
            }
        }

        #endregion

        #region V_Tim
        private void V_Tim()
        {
            try
            {
                currActionTask = ActionTask.View;
                //FrmSearchApctpn9 _FrmTim = new FrmSearchApctpn9(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), StartUp.Ma_ct);
                FrmSearchTT_SOCTHDA_HDDT _FrmTim = new FrmSearchTT_SOCTHDA_HDDT(StartUp.SysObj, StartUp.filterId, StartUp.filterView);
                SmLib.SysFunc.LoadIcon(_FrmTim);
                _FrmTim.Closed += new EventHandler(_FrmTim_Closed);
                _FrmTim.ShowDialog();
                _FrmTim = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void _FrmTim_Closed(object sender, EventArgs e)
        {
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

        #region V_In
        private void V_In()
        {
            try
            {
                FrmPrintTT_SOCTHDA_HDDT oReport = new FrmPrintTT_SOCTHDA_HDDT(IsNd51);
                oReport.DsPrint = StartUp.DsTrans.Copy();
                oReport.DsPrint.Tables[0].TableName = "TablePH";
                oReport.DsPrint.Tables[1].TableName = "TableCT";


                foreach (DataRow r in oReport.DsPrint.Tables[1].AsEnumerable())
                {
                    r["dvt_qd"] = r["dvt_qd"];
                    r["sl_qd"] = r["so_luong"];
                    r["gia_qd_nt"] = r["gia_nt"];
                    r["gia_qd"] = r["gia"];
                    r["gia_qd_nt2"] = r["gia_nt2"];
                    r["gia_qd2"] = r["gia2"];
                }


                string stt_rec = oReport.DsPrint.Tables["TablePH"].Rows[iRow]["stt_rec"].ToString();
                SqlCommand cmd = new SqlCommand(string.Format("dbo.[SOCTHDA-Getslqd] '{0}'", stt_rec));
                DataSet dsqd = SysO.ExcuteReader(cmd);
                if (dsqd != null && dsqd.Tables.Count > 0 && dsqd.Tables[0].Rows.Count > 0)
                {
                    decimal sl = 0, t_sl = 0;
                    foreach (DataRow r in dsqd.Tables[0].Rows)
                    {
                        var ct = from c in oReport.DsPrint.Tables[1].AsEnumerable()
                                 where c.Field<string>("stt_rec0") == r["stt_rec0"].ToString()
                                 select c;

                        foreach (DataRow row in ct)
                        {
                            row["dvt_qd"] = r["dvt_qd"];
                            row["sl_qd"] = sl = (decimal)r["sl_qd"];
                            row["gia_qd_nt"] = r["gia_qd_nt"];
                            row["gia_qd"] = r["gia_qd"];
                            row["gia_qd_nt2"] = r["gia_qd_nt2"];
                            row["gia_qd2"] = r["gia_qd2"];
                        }
                    }
                }
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_sl_qd"] = SumFunction(oReport.DsPrint.Tables[1], "sl_qd", 0);


                DataColumn newcolumn = new DataColumn("so_lien", typeof(int));
                newcolumn.DefaultValue = 1;
                oReport.DsPrint.Tables["TablePH"].Columns.Add(newcolumn);

                newcolumn = new DataColumn("so_ct_goc", typeof(int));
                newcolumn.DefaultValue = 0;
                oReport.DsPrint.Tables["TablePH"].Columns.Add(newcolumn);

                newcolumn = new DataColumn("ban_sao", typeof(string));
                newcolumn.DefaultValue = "";
                oReport.DsPrint.Tables["TablePH"].Columns.Add(newcolumn);

                //them 2 cot tag, stt cho table ct
                DataColumn col = new DataColumn("tag", typeof(int));
                col.DefaultValue = 0;
                oReport.DsPrint.Tables["TableCT"].Columns.Add(col);

                col = new DataColumn("stt", typeof(string));
                col.DefaultValue = "";
                oReport.DsPrint.Tables["TableCT"].Columns.Add(col);

                oReport.DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                oReport.DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                oReport.DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";

                oReport.DsPrint.Tables.Add(StartUp.GetDmnt().Copy());
                oReport.DsPrint.Tables.Add(CreateTableInfo().Copy());
                oReport.DsPrint.Tables.Add(CreateTableMST(StartUp.M_MA_THUE, "TableMST_NB").Copy());
                oReport.DsPrint.Tables.Add(CreateTableMST(oReport.DsPrint.Tables["TablePH"].Rows[iRow]["ma_so_thue"].ToString().TrimEnd(), "TableMST_NM").Copy());

                oReport.ShowDialog();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        #region CreateTableInfo
        DataTable CreateTableInfo()
        {
            DataTable table = new DataTable();
            table.TableName = "TableInfo";

            DataColumn column = new DataColumn("M_PHONE", typeof(string));
            column.DefaultValue = StartUp.M_PHONE;
            table.Columns.Add(column);

            column = new DataColumn("M_MST", typeof(string));
            column.DefaultValue = StartUp.M_MA_THUE;
            table.Columns.Add(column);

            column = new DataColumn("M_Ten_CTY", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetSysVar("M_Ten_CTY").ToString().ToUpper();
            table.Columns.Add(column);

            column = new DataColumn("M_DIA_CHI", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetSysVar("M_DIA_CHI").ToString();
            table.Columns.Add(column);

            column = new DataColumn("M_TK_NH", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetOption("M_TK_NH").ToString();
            table.Columns.Add(column);

            column = new DataColumn("M_CUC_THUE", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetOption("M_CUC_THUE").ToString().ToUpper();
            table.Columns.Add(column);

            //column = new DataColumn("so_dong_in", typeof(string));
            //column.DefaultValue = StartUp.DmctInfo["so_dong_in"].ToString();
            //table.Columns.Add(column);

            DataRow dr = table.NewRow();
            table.Rows.Add(dr);
            return table;
        }
        #endregion

        #region CreateTableMST
        DataTable CreateTableMST(string ma_so_thue, string tablename)
        {
            DataTable table = new DataTable();
            table.TableName = tablename;
            int length = ma_so_thue.Length;
            int i = -1;
            DataColumn column = null;
            for (i = 0; i < length; i++)
            {
                column = new DataColumn("m" + (i + 1).ToString(), typeof(string));
                column.DefaultValue = ma_so_thue.Substring(i, 1);
                table.Columns.Add(column);
            }
            while (i < 14)
            {
                column = new DataColumn("m" + (i + 1).ToString(), typeof(string));
                column.DefaultValue = "";
                table.Columns.Add(column);
                i++;
            }

            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return table;
        }
        #endregion
        #endregion

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
            if (currActionTask != ActionTask.Add)
            {
                LoadDataDu13();
                UpdateTonKho();

                //update so_luong km
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_sl_km"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 1);
                //update tien km
                UpdateTienKM_NT();
                UpdateTienKM();
            }
            IsVisibilityFieldsXamDataGridByMa_NT(ma_nt);
            IsVisibilityFieldsXamDataGridBySua_Tien();
            IsVisibilityFieldsXamDataGridByPx_gia_dd();
            if (StartUp.DsTrans.Tables[0].Columns.Contains("tt_hddt_tcn"))
                IsCheckedPH_HDDT.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString() == "0");
            //hien cot km_ck
            if (StartUp.M_KM_CK == 0)
            {
                //GrdCt.FieldLayouts[0].Fields["km_ck"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["km_ck"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["tk_km_i"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["tk_km_i"].Settings.CellMaxWidth = 0;

            }
            if (StartUp.M_AR_CK == 0)
            {
                //GrdCt.FieldLayouts[0].Fields["tl_ck"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["tl_ck"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["ck_nt"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["ck_nt"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["tk_ck"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["tk_ck"].Settings.CellMaxWidth = 0;
                ChkTinh_ck.IsEnabled = false;
            }

        }
        #region IsVisibilityFieldsXamDataGridByMa_NT
        void IsVisibilityFieldsXamDataGridByMa_NT(string ma_nt)
        {
            //Nếu ngoại tệ = tiền hoạch toán
            if (ma_nt == StartUp.M_ma_nt0)
            {
                //GrdCt không hiển thị 
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["gia2"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["tien2"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["gia2"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["tien2"].Settings.CellMaxWidth = 0;

            }
            else
            {
                //GrdCt hiển thị 
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["gia2"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["tien2"].Visibility = Visibility.Visible;

                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["gia2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia2"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["tien2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien2"].Width.Value.Value;

                if (StartUp.M_AR_CK != 0)
                {
                    GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Visible;
                    GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ck"].Width.Value.Value;
                }

            }
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            // Debug.WriteLine("============= changlanguage ==============");
            //ChangeLanguage(StartUp.M_LAN, GrdCt, true);

            //Dispatcher.Invoke(new Action(delegate()
            //{
            //    try
            //    {
            //        LanguageProvider.ChangeLanguage(GrdCt as Visual, LanguageID.Trim() + ".TabInfo.tabItemHT", StartUp.M_LAN, false);
            //    }
            //    catch (Exception)
            //    {
            //    }
            ChangeLanguage();

            //}), DispatcherPriority.Background, new object[] { });

        }
        #endregion

        #region IsVisibilityFieldsXamDataGridBySua_Tien
        void IsVisibilityFieldsXamDataGridBySua_Tien()
        {
            IsCheckedSua_tien.Value = ChkSua_tien.IsChecked.Value;
        }
        #endregion

        #region IsVisibilityFieldsXamDataGridByPx_gia_dd
        void IsVisibilityFieldsXamDataGridByPx_gia_dd()
        {
            IsCheckedPx_gia_dd.Value = ChkPx_gia_dd.IsChecked.Value;
        }
        #endregion
        #endregion

        #region txtma_gd_PreviewLostFocus
        private void txtma_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtma_gd.RowResult == null || string.IsNullOrEmpty(txtma_gd.Text.Trim()))
                    return;
                if (M_LAN.ToUpper().Equals("V"))
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_gd"] = txtma_gd.RowResult["ten_gd"].ToString().Trim();
                else
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_gd"] = txtma_gd.RowResult["ten_gd2"].ToString().Trim();
            }
        }
        #endregion

        #region LoadDataDu13
        private void LoadDataDu13()
        {
            txtSoDuKH.Value = ArapLib.ArFuncLib.GetSdkh13(StartUp.SysObj, StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString(), StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString());
        }
        #endregion

        #region txtMa_kh_PreviewLostFocus
        private bool txtDiaChiFocusable = true;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtMa_kh.RowResult == null || string.IsNullOrEmpty(txtMa_kh.Text.Trim()))
                    return;

                if (txtMa_kh.Text.Trim() != ma_kh_old) // txtMa_kh.IsDataChanged)
                {
                    //if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Copy)
                    {
                        if (M_LAN.ToUpper().Equals("V"))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                        else
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh2"] = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();
                        if (e != null)
                        {
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue_dmkh"] = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"] = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();

                        }
                    }
                    if (!string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim()))
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString().Trim() == "")
                    {
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = "";
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = "";

                        txtMa_nx.Text = txtMa_kh.RowResult["tk"].ToString().Trim();
                        txtMa_nx.SearchInit();
                        if (txtMa_nx.RowResult != null)
                        {
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = txtMa_nx.RowResult["ten_nx"].ToString();
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = txtMa_nx.RowResult["ten_nx2"].ToString();
                        }
                    }
                }

                if (string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                {
                    txtDiaChiFocusable = true;
                }
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = txtMa_kh.RowResult["dia_chi"].ToString().Trim();
                    txtDiaChiFocusable = false;
                }
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_thck"].ToString().Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ma_thck"] = txtMa_kh.RowResult["ma_thck"];
                }
                if (ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"].ToString(), 0) == 0)
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] = ParseInt(txtMa_kh.RowResult["han_tt"], 0);
                }
                LoadDataDu13();
                StartUp.DsTrans.Tables[0].DefaultView[0]["tk_nh"] = txtMa_kh.RowResult["tk_nh"].ToString();
                StartUp.DsTrans.Tables[0].DefaultView[0]["nh_kh3"] = txtMa_kh.RowResult["nh_kh3"].ToString();
            }
        }
        #endregion

        #region txtDia_chi_GotFocus
        private void txtDia_chi_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtDiaChiFocusable)
            {
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
        #endregion

        #region txtMa_nx_PreviewLostFocus
        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = "";
            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = "";
            if (!string.IsNullOrEmpty(txtMa_nx.Text.Trim()))
            {
                if (StartUp.DsTrans.Tables[0].Rows[iRow]["sua_tkthue"].ToString() == "0")
                    StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_no"] = txtMa_nx.RowResult["ma_nx"].ToString().Trim();
                if (M_LAN.ToUpper().Equals("V"))
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = txtMa_nx.RowResult["ten_nx"].ToString();
                else
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = txtMa_nx.RowResult["ten_nx2"].ToString();
            }
            LoadDataDu13();
        }
        #endregion

        #region txtNgay_ct_LostFocus
        private void txtNgay_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct.Value == DBNull.Value)
                txtNgay_ct.Value = DateTime.Now;
            if (!txtNgay_ct.IsFocusWithin && IsInEditMode.Value)
            {
                //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct.dValue)))
                //{
                //    ExMessageBox.Show( 790,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //    txtNgay_ct.Value = DateTime.Now.Date;
                //}
                if (StartUp.M_ngay_lct.Equals("0") || string.IsNullOrEmpty(txtNgay_lct.Text))
                    txtNgay_lct.Value = txtNgay_ct.Value;

            }
        }
        #endregion

        #region txtngay_lct_LostFocus
        private void txtNgay_lct_LostFocus(object sender, RoutedEventArgs e)
        {

            if (!txtNgay_lct.IsFocusWithin && IsInEditMode.Value && txtNgay_lct.IsValueValid)
            {
                if (txtNgay_lct.Value != null && txtNgay_ct.Value.ToString() != txtNgay_lct.Value.ToString())
                {
                    ExMessageBox.Show(795, StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        #endregion

        #region txtMa_qs_PreviewLostFocus
        private void txtMa_qs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!IsInEditMode.Value)
                return;
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_seri"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri"] = txtMa_qs.RowResult["so_seri"].ToString().Trim();
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_ct1"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct1"] = txtMa_qs.RowResult["so_ct1"].ToString();
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_ct2"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct2"] = txtMa_qs.RowResult["so_ct2"].ToString();
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["transform"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["transform"] = txtMa_qs.RowResult["transform"].ToString();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (IsInEditMode.Value == true && !e.NewFocus.GetType().Equals(typeof(SmVoucherLib.ToolBarButton)))
                    if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString()))
                    {
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()) || (IsNd51 && txtMa_qs.IsDataChanged))
                        {
                            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"].ToString().Trim()) || !StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString().Trim().Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"].ToString().Trim()) || IsNd51)
                            {
                                txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"] = txtSo_ct.Text;
                                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"] = txtMa_qs.Text;
                            }
                            else
                                txtSo_ct.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"].ToString().Trim();
                        }
                        if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                        {
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                            StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"] = txtSo_ct.Text;
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"] = txtMa_qs.Text;
                        }

                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_lien_hd"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"] = txtMa_qs.RowResult["so_lien_hd"];
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien1"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien1"] = txtMa_qs.RowResult["ten_lien1"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien2"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien2"] = txtMa_qs.RowResult["ten_lien2"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien3"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien3"] = txtMa_qs.RowResult["ten_lien3"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien4"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien4"] = txtMa_qs.RowResult["ten_lien4"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien5"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien5"] = txtMa_qs.RowResult["ten_lien5"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien6"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien6"] = txtMa_qs.RowResult["ten_lien6"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien7"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien7"] = txtMa_qs.RowResult["ten_lien7"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien8"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien8"] = txtMa_qs.RowResult["ten_lien8"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien9"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien9"] = txtMa_qs.RowResult["ten_lien9"].ToString();

                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_dn_in"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ten_dn_in"] = txtMa_qs.RowResult["ten_dn_in"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["mst_dn_in"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["mst_dn_in"] = txtMa_qs.RowResult["mst_dn_in"].ToString();

                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["mau_hd"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["mau_hd"] = txtMa_qs.RowResult["mau_hd"].ToString();
                        if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ma_file"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_file"] = txtMa_qs.RowResult["ma_file"].ToString();
                    }
            }));
        }
        #endregion

        #region Mã ngoại tệ lostfocus
        private void txtMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;
            //if (txtMa_nt.IsDataChanged)
            //{

            IsVisibilityFieldsXamDataGridByMa_NT(txtMa_nt.Text.Trim());
            if (txtMa_nt.RowResult != null)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = txtMa_nt.RowResult["loai_tg"];
                if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"] = 1;
                }
                else
                {
                    // if (currActionTask != ActionTask.Edit)
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"] = StartUp.GetRates(txtMa_nt.Text.Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                }
            }
            Ty_gia_ValueChanged(true);
            //}
        }

        #endregion

        #region Tỷ giá
        private void txtTy_gia_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTy_gia.Value == DBNull.Value)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"] = 0;
            }
            if (txtTy_gia.OldValue == Convert.ToDecimal(txtTy_gia.Value))
                return;
            Ty_gia_ValueChanged(false);

        }

        void Ty_gia_ValueChanged(bool IsMa_ntChanged)
        {
            if (currActionTask == ActionTask.Delete || currActionTask == ActionTask.View)
                //|| txtTy_gia.OldValue == Convert.ToDecimal(txtTy_gia.Value)
                return;
            Ty_Gia_ValueChange.Value = (txtTy_gia.OldValue != Convert.ToDecimal(txtTy_gia.Value));

            decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);

            //Hạch toán thay đổi
            // if (sua_tien == 0 && _ty_gia != 0)
            if (_ty_gia != 0)
            {
                //ma_nt thay doi thi updatetotal or ty gia thay doi và sua_tien = 0 thì updatetotal
                if (sua_tien == 0 || IsMa_ntChanged)
                {
                    UpdateTotal("ck", "ck_nt", false);
                    UpdateTotal("gia2", "gia_nt2", true);
                    UpdateTotal("tien2", "tien_nt2", false);
                    UpdateTotal("gia", "gia_nt", true);
                    UpdateTotal("tien", "tien_nt", false);

                    decimal t_tien_km = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 1);
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"] = t_tien_km;
                    if (StartUp.M_THUE_KM_CK == 1)
                    {
                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"] = SysFunc.Round((t_tien_km * thue_suat) / 100, StartUp.M_ROUND);
                    }
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
                }
            }
        }
        #endregion

        #region UpdateTotal
        void UpdateTotal(string columnname, string columnname_nt, bool isPrice)
        {
            decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                decimal tien_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i][columnname_nt], 0);
                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    StartUp.DsTrans.Tables[1].DefaultView[i][columnname] = tien_nt;
                else
                {
                    decimal tien = SysFunc.Round(tien_nt * ty_gia, isPrice == false ? StartUp.M_ROUND : StartUp.M_ROUND_GIA);
                    if (tien != 0)
                        StartUp.DsTrans.Tables[1].DefaultView[i][columnname] = tien;
                }
            }
        }
        #endregion

        #region txtTy_gia_GotFocus

        private void txtTy_gia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Voucher_Ma_nt0.Value)
            {
                KeyboardNavigation.SetTabNavigation(GrNT, KeyboardNavigationMode.Continue);
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
        #endregion

        #region txtTk_thue_co_PreviewLostFocus
        private void txtTk_thue_co_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtTk_thue_co.RowResult == null || string.IsNullOrEmpty(txtTk_thue_co.Text.Trim()))
                    return;
                StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_co_cn"] = txtTk_thue_co.RowResult["tk_cn"];
            }
        }
        #endregion

        #region Check sửa tiền
        private void ChkSua_tien_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSua_tien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                UpdateTotalChkSua_tien();
                //txtTy_gia.Focus();
                Ty_gia_ValueChanged(false);
            }
            IsVisibilityFieldsXamDataGridBySua_Tien();
        }
        #endregion

        #region UpdateTotalChkSua_tien
        void UpdateTotalChkSua_tien()
        {
            int countCT = StartUp.DsTrans.Tables[1].DefaultView.Count;
            if (countCT > 0)
            {
                decimal so_luong, gia_nt2, gia_nt;
                decimal tien_nt2;
                decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                for (int i = 0; i < countCT; i++)
                {
                    so_luong = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"], 0);
                    gia_nt2 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt2"], 0);
                    gia_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"], 0);
                    if (so_luong != 0)
                    {
                        if (gia_nt2 != 0)
                        {
                            tien_nt2 = SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"] = tien_nt2;

                            //tinh lai ck_nt
                            decimal tl_ck = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tl_ck"], 0);
                            if (tl_ck != 0)
                            {
                                decimal ck_nt = SysFunc.Round((tien_nt2 * tl_ck) / 100, StartUp.M_ROUND_NT);
                                StartUp.DsTrans.Tables[1].DefaultView[i]["ck_nt"] = ck_nt;

                                decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                if (ck != 0)
                                {
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["ck"] = ck;
                                }
                            }
                        }
                        if (gia_nt != 0)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                        }
                    }
                }
                //tinh lai tong tien hang
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);

                //tinh tong ck
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "ck_nt", 0);
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);

                //tinh lai tong tien von
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);

                //tinh lai tien_km_nt
                UpdateTienKM_NT();

            }
        }
        #endregion

        #region GrdCt_PreviewEditModeEnded
        private void GrdCt_PreviewEditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value == false)
                    return;

                if (GrdCt.ActiveCell != null && StartUp.DsTrans.Tables[1].GetChanges(DataRowState.Deleted) == null)
                {
                    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                    decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);
                    int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
                    int tinh_ck = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["tinh_ck"], 0);
                    string nh_kh3 = StartUp.DsTrans.Tables[0].DefaultView[0]["nh_kh3"].ToString();

                    switch (e.Cell.Field.Name)
                    {
                        #region ma_vt
                        case "ma_vt":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                if (e.Cell.IsDataChanged)
                                {
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    if (txt.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["ten_vt"].Value = txt.RowResult["ten_vt"];
                                        e.Cell.Record.Cells["ten_vt2"].Value = txt.RowResult["ten_vt2"];
                                        e.Cell.Record.Cells["dvt"].Value = txt.RowResult["dvt"];

                                        //var dtCT = (from ct in StartUp.DsTrans.Tables[1].AsEnumerable()
                                        //            where ct["stt_rec"].ToString().Trim() == StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString().Trim()
                                        //            && ct["ma_vt"].ToString().Trim()==txt.RowResult["ma_vt"].ToString().Trim()
                                        //            select new
                                        //            {
                                        //                gia_nt2 = ParseDecimal(ct["gia_nt2"], 0),
                                        //                gia2 = ParseDecimal(ct["gia2"], 0),

                                        //            });
                                        //if (dtCT.Count() > 0)
                                        //{
                                        //    e.Cell.Record.Cells["gia_nt2"].Value = dtCT.First().gia_nt2;
                                        //    e.Cell.Record.Cells["gia2"].Value = dtCT.First().gia2;
                                        //}

                                        //cac tk temp de binding
                                        e.Cell.Record.Cells["tk_dt_dmvt"].Value = txt.RowResult["tk_dt"].ToString();
                                        e.Cell.Record.Cells["tk_ck_dmvt"].Value = txt.RowResult["tk_ck"].ToString();
                                        e.Cell.Record.Cells["tk_gv_dmvt"].Value = txt.RowResult["tk_gv"].ToString();
                                        e.Cell.Record.Cells["tk_km_dmvt"].Value = txt.RowResult["tk_km"].ToString();

                                        //132458936
                                        if (txt.RowResult["tk_dt"].ToString().Trim() != "")
                                            if (txtma_gd.Text.Equals("2"))
                                                e.Cell.Record.Cells["tk_dt"].Value = txt.RowResult["tk_dtnb"].ToString();
                                            else
                                                e.Cell.Record.Cells["tk_dt"].Value = txt.RowResult["tk_dt"].ToString();
                                        if (txt.RowResult["tk_ck"].ToString().Trim() != "")
                                            e.Cell.Record.Cells["tk_ck"].Value = txt.RowResult["tk_ck"].ToString();

                                        if (txt.RowResult["tk_gv"].ToString().Trim() != "")
                                            e.Cell.Record.Cells["tk_gv"].Value = txt.RowResult["tk_gv"].ToString();
                                        if (txt.RowResult["tk_km"].ToString().Trim() != "")
                                            e.Cell.Record.Cells["tk_km_i"].Value = txt.RowResult["tk_km"].ToString();
                                        if (e.Cell.Record.Cells["km_ck"].Value.ToString() == "0")
                                            e.Cell.Record.Cells["tk_km_i"].Value = "";

                                        e.Cell.Record.Cells["gia_ton"].Value = txt.RowResult["gia_ton"];
                                        e.Cell.Record.Cells["vt_ton_kho"].Value = txt.RowResult["vt_ton_kho"];
                                        //neu theo doi ton kho = 0 thi
                                        // so_luong = 0, gia = 0 (anhbtn, khaibl - 08-10-2010)
                                        //if (isIsDataChanged)
                                        //{
                                        //DataRow tbGia = StartUp.Getdmgia2(e.Cell.Value.ToString(), string.Format("{0:yyyyMMdd}", txtNgay_ct.dValue));
                                        //if (tbGia != null)
                                        //{
                                        //    if (Voucher_Ma_nt0.Value)
                                        //    {
                                        //        e.Cell.Record.Cells["gia_nt2"].Value = tbGia["gia2"];
                                        //    }
                                        //    else
                                        //    {
                                        //        e.Cell.Record.Cells["gia_nt2"].Value = tbGia["gia_nt2"];
                                        //    }
                                        //    e.Cell.Record.Cells["gia2"].Value = tbGia["gia2"];
                                        //}
                                        //else
                                        //{
                                        //    e.Cell.Record.Cells["gia_nt2"].Value = 0;
                                        //    e.Cell.Record.Cells["gia2"].Value = 0;
                                        //}
                                        if (ParseInt(txt.RowResult["vt_ton_kho"], 0) == 0)
                                        {
                                            e.Cell.Record.Cells["so_luong"].Value = 0;
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);

                                            e.Cell.Record.Cells["gia_nt2"].Value = 0;
                                            e.Cell.Record.Cells["gia2"].Value = 0;
                                            e.Cell.Record.Cells["gia_nt"].Value = 0;
                                            e.Cell.Record.Cells["gia"].Value = 0;
                                            e.Cell.Record.Cells["tien"].Value = 0;
                                            e.Cell.Record.Cells["tien_nt"].Value = 0;
                                        }
                                        //}
                                        DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                        drVCT["sua_tk_vt"] = txt.RowResult["sua_tk_vt"];
                                        drVCT["sl_min"] = txt.RowResult["sl_min"];
                                        drVCT["tk_vt_dmvt"] = txt.RowResult["tk_vt"].ToString();

                                        //Lấy tk vật tư
                                        if (txt.RowResult["tk_vt"].ToString() != "")
                                        {
                                            e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"].ToString();
                                        }
                                        //if (isIsDataChanged)
                                        //{
                                        if (ParseDecimal(e.Cell.Record.Cells["gia_nt2"].Value, 0) == 0 && ParseDecimal(e.Cell.Record.Cells["gia2"].Value, 0) == 0)
                                        {
                                            if (txtNgay_ct.dValue != new DateTime())
                                            {
                                                DataRow dr = StartUp.Getdmgia2(e.Editor.Value.ToString(), String.Format("{0:yyyyMMdd}", txtNgay_ct.dValue), nh_kh3);
                                                if (dr != null)
                                                {
                                                    if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                                        e.Cell.Record.Cells["gia_nt2"].Value = dr["gia2"];
                                                    else
                                                        e.Cell.Record.Cells["gia_nt2"].Value = dr["gia_nt2"];

                                                    e.Cell.Record.Cells["gia2"].Value = dr["gia2"];
                                                }
                                            }
                                        }
                                        //}
                                        CellValuePresenter cell_Kho = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kho_i"]);
                                        AutoCompleteTextBox autoCompleteKho = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Kho.Editor as ControlHostEditor);
                                        if (autoCompleteKho != null)
                                        {
                                            autoCompleteKho.SearchInit();
                                            if (autoCompleteKho.RowResult != null)
                                            {
                                                if (autoCompleteKho.RowResult["tk_dl"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteKho.RowResult["tk_dl"].ToString().Trim()))
                                                {
                                                    e.Cell.Record.Cells["tk_vt"].Value = autoCompleteKho.RowResult["tk_dl"].ToString();
                                                    drVCT["tk_vt_dmvt"] = autoCompleteKho.RowResult["tk_dl"].ToString();
                                                }
                                            }
                                        }



                                        //Update Binding
                                        CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_vt"]);
                                        ControlFunction.RefreshSingleBinding(cellV, AutoCompleteTextBox.IsReadOnlyProperty);

                                        if (ParseInt(txt.RowResult["vt_ton_kho"], 0) == 1)
                                        {

                                            if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()))
                                            {
                                                e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString(), (e.Cell.Record.DataItem as DataRowView)["ma_vv_i"].ToString());
                                            }
                                        }
                                        else
                                        {
                                            e.Cell.Record.Cells["ton13"].Value = DBNull.Value;
                                        }
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region km_ck
                        case "km_ck":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["km_ck"].Value = 0;
                                    if (e.Cell.Record.Cells["km_ck"].Value.ToString() == "1")
                                        e.Cell.Record.Cells["tk_km_i"].Value = e.Cell.Record.Cells["tk_km_dmvt"].Value;
                                    else
                                        e.Cell.Record.Cells["tk_km_i"].Value = "";

                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_sl_km"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 1);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    UpdateTienKM_NT();
                                    UpdateTienKM();

                                }
                            }
                            object count = GrdCt.Records.Count;
                            break;
                        #endregion

                        #region ma_kho_i
                        case "ma_kho_i":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                AutoCompleteTextBox autoCompleteKho = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);

                                if (autoCompleteKho.IsDataChanged)
                                {
                                    DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;

                                    CellValuePresenter cell_VT = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_VT.Editor as ControlHostEditor);
                                    if (txt.RowResult == null)
                                        txt.SearchInit();
                                    if (txt.RowResult != null)
                                    {
                                        if (txt.RowResult["sua_tk_vt"] != DBNull.Value && Convert.ToDecimal(txt.RowResult["sua_tk_vt"]) == 0)
                                        {
                                            e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"].ToString();
                                            drVCT["tk_vt_dmvt"] = txt.RowResult["tk_vt"].ToString();

                                        }
                                    }

                                    if (autoCompleteKho != null)
                                    {
                                        autoCompleteKho.SearchInit();
                                        if (autoCompleteKho.RowResult != null)
                                        {
                                            if (autoCompleteKho.RowResult["tk_dl"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteKho.RowResult["tk_dl"].ToString().Trim()))
                                            {
                                                if (autoCompleteKho.RowResult["tk_dl"].ToString() != "")
                                                    e.Cell.Record.Cells["tk_vt"].Value = autoCompleteKho.RowResult["tk_dl"].ToString();
                                                drVCT["tk_vt_dmvt"] = autoCompleteKho.RowResult["tk_dl"].ToString();

                                            }
                                        }
                                    }

                                    if (ParseInt(txt.RowResult["vt_ton_kho"], 0) == 1)
                                    {

                                        if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()))
                                        {
                                            e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString(), (e.Cell.Record.DataItem as DataRowView)["ma_vv_i"].ToString());
                                        }
                                    }
                                    else
                                    {
                                        e.Cell.Record.Cells["ton13"].Value = DBNull.Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region so luong
                        case "so_luong":
                            {
                                if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                    e.Cell.Record.Cells["so_luong"].Value = 0;

                                decimal so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                //AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]).Editor as ControlHostEditor);
                                //if (txt.RowResult != null && txt.RowResult["gia_ton"] != DBNull.Value)
                                if (int.Parse(e.Cell.Record.Cells["gia_ton"].Value.ToString()) == 3)
                                    if (so_luong == 0)
                                    {
                                        ExMessageBox.Show(800, StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                GrdCt.ActiveCell = e.Cell.Record.Cells["so_luong"];
                                            }));
                                        return;
                                    }
                                if (e.Cell.IsDataChanged)
                                {
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_sl_km"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 1);
                                    //Xuat ban khong co so luong thi nhat dinh khong co gia von
                                    if (so_luong == 0)
                                    {
                                        e.Cell.Record.Cells["gia_nt"].Value = 0;
                                        e.Cell.Record.Cells["gia"].Value = 0;
                                        e.Cell.Record.Cells["gia_nt2"].Value = 0;
                                        e.Cell.Record.Cells["gia2"].Value = 0;
                                    }

                                    decimal gia_nt2 = ParseDecimal(e.Cell.Record.Cells["gia_nt2"].Value, 0);
                                    decimal gia_nt = ParseDecimal(e.Cell.Record.Cells["gia_nt"].Value, 0);

                                    // Neu ko check sua tien, gia_nt*ty_gia != 0 va so_luong != 0
                                    // thì gia = gia_nt*ty_gia
                                    decimal gia2 = SysFunc.Round(gia_nt2 * ty_gia, StartUp.M_ROUND_GIA);
                                    decimal gia = SysFunc.Round(gia_nt * ty_gia, StartUp.M_ROUND_GIA);
                                    //if (sua_tien == 0 && so_luong != 0 )
                                    if (so_luong != 0)
                                    {
                                        if (gia2 != 0)
                                            e.Cell.Record.Cells["gia2"].Value = gia2;
                                        if (gia != 0)
                                            e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    //Co the nhap Tien =Gia*So_luong
                                    decimal tien_nt2 = SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                    decimal tien_nt = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                    //if (sua_tien == 0 )
                                    //{
                                    if (tien_nt2 != 0)
                                    {
                                        e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                        //tinh lai tong tien hang, tong thue, tong tien sau ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);
                                    }
                                    if (tien_nt != 0)
                                    {
                                        e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        //tinh lai tong tien hang, tong thue, tong tien sau ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                    }
                                    //}

                                    // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                    // thì tien = tien_nt*ty_gia
                                    decimal tien2 = SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                    decimal tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    //if (sua_tien == 0) 
                                    //{
                                    if (tien2 != 0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = tien2;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                                    }
                                    if (tien != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    }
                                    //tinh lai ck_nt
                                    decimal tl_ck = ParseDecimal(e.Cell.Record.Cells["tl_ck"].Value, 0);
                                    if (tl_ck != 0)
                                    {
                                        decimal ck_nt = SysFunc.Round((tien_nt2 * tl_ck) / 100, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                        //tinh tong ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "ck_nt", 0);
                                        decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                        //if (sua_tien == 0 && ck != 0)
                                        if (ck != 0)
                                        {
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                            //tinh tong ck
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
                                        }
                                    }
                                    //}

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {

                                        e.Cell.Record.Cells["gia"].Value = e.Cell.Record.Cells["gia_nt"].Value;
                                        e.Cell.Record.Cells["gia2"].Value = e.Cell.Record.Cells["gia_nt2"].Value;
                                        e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        e.Cell.Record.Cells["tien2"].Value = e.Cell.Record.Cells["tien_nt2"].Value;
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;

                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"];
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"];
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"];
                                    }

                                    if (ParseInt(e.Cell.Record.Cells["km_ck"].Value, 0) == 1)
                                    {
                                        UpdateTienKM_NT();
                                        UpdateTienKM();

                                    }
                                }
                            }
                            break;
                        #endregion

                        #region gia nt2
                        case "gia_nt2":
                            {

                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["gia_nt2"].Value = 0;

                                    decimal so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                    decimal gia_nt2 = ParseDecimal(e.Cell.Record.Cells["gia_nt2"].Value, 0);

                                    // Neu ko check sua tien, gia_nt*ty_gia != 0 va so_luong != 0
                                    // thì gia = gia_nt*ty_gia
                                    decimal gia2 = SysFunc.Round(gia_nt2 * ty_gia, StartUp.M_ROUND_GIA);
                                    //if (sua_tien == 0 && so_luong != 0 && gia2 != 0)
                                    if (so_luong != 0 && gia2 != 0)
                                    {
                                        e.Cell.Record.Cells["gia2"].Value = gia2;
                                    }

                                    //Co the nhap Tien =Gia*So_luong
                                    decimal tien_nt2 = SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                    //if (sua_tien == 0 && tien_nt2 != 0)
                                    if (tien_nt2 != 0)
                                    {
                                        e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);
                                    }

                                    // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                    // thì tien = tien_nt*ty_gia
                                    decimal tien2 = SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                    //if (sua_tien == 0 && tien2 != 0)
                                    if (tien2 != 0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = tien2;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["gia2"].Value = e.Cell.Record.Cells["gia_nt2"].Value;
                                        e.Cell.Record.Cells["tien2"].Value = e.Cell.Record.Cells["tien_nt2"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"];
                                    }

                                    if (ParseInt(e.Cell.Record.Cells["km_ck"].Value, 0) == 1)
                                    {
                                        UpdateTienKM_NT();
                                        UpdateTienKM();

                                    }
                                }
                            }
                            break;
                        #endregion

                        #region tien nt2
                        case "tien_nt2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tien_nt2"].Value = 0;

                                    decimal tien_nt2 = ParseDecimal(e.Cell.Record.Cells["tien_nt2"].Value, 0);
                                    //if (sua_tien == 0)
                                    //{
                                    //tinh lai ck_nt
                                    decimal tl_ck = ParseDecimal(e.Cell.Record.Cells["tl_ck"].Value, 0);
                                    if (tl_ck != 0)
                                    {
                                        decimal ck_nt = SysFunc.Round((tien_nt2 * tl_ck) / 100, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                        //tinh tong ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "ck_nt", 0);
                                        decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                        //if (sua_tien == 0 && ck != 0)
                                        if (ck != 0)
                                        {
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                            //tinh tong ck
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
                                        }
                                    }
                                    //}

                                    //tinh lai tong tien hang
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);

                                    decimal tien2 = SmLib.SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                    //if (tien2 != 0 && sua_tien == 0)
                                    if (tien2 != 0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = tien2;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = e.Cell.Record.Cells["tien_nt2"].Value;
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"];
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"];
                                    }

                                    if (ParseInt(e.Cell.Record.Cells["km_ck"].Value, 0) == 1)
                                    {
                                        UpdateTienKM_NT();
                                        UpdateTienKM();

                                    }
                                }
                            }
                            break;
                        #endregion

                        #region gia nt
                        case "gia_nt":
                            {

                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["gia_nt"].Value = 0;

                                    decimal so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                    decimal gia_nt = ParseDecimal(e.Cell.Record.Cells["gia_nt"].Value, 0);

                                    // Neu ko check sua tien, gia_nt*ty_gia != 0 va so_luong != 0
                                    // thì gia = gia_nt*ty_gia
                                    decimal gia = SysFunc.Round(gia_nt * ty_gia, StartUp.M_ROUND_GIA);
                                    //if (sua_tien == 0 && so_luong != 0 && gia != 0)
                                    if (so_luong != 0 && gia != 0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    //Co the nhap Tien =Gia*So_luong
                                    decimal tien_nt = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                    //if (sua_tien == 0 && tien_nt != 0)
                                    if (tien_nt != 0)
                                    {
                                        e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                    }

                                    // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                    // thì tien = tien_nt*ty_gia
                                    decimal tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    //if (sua_tien == 0 && tien != 0)
                                    if (tien != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = e.Cell.Record.Cells["gia_nt"].Value;
                                        e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region tien nt
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tien_nt"].Value = 0;

                                    decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["tien_nt"].Value, 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                    decimal tien = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    //if (tien != 0 && sua_tien == 0)
                                    if (tien != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region tl_ck
                        case "tl_ck":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tl_ck"].Value = 0;

                                    decimal tl_ck = ParseDecimal(e.Cell.Record.Cells["tl_ck"].Value, 0);
                                    if (tl_ck != 0)
                                    {
                                        decimal tien_nt2 = ParseDecimal(e.Cell.Record.Cells["tien_nt2"].Value, 0);
                                        decimal ck_nt = SysFunc.Round((tien_nt2 * tl_ck) / 100, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                        //tinh tong ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "ck_nt", 0);
                                        decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                        //if (sua_tien == 0 && ck != 0)
                                        if (ck != 0)
                                        {
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                            //tinh tong ck
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
                                        }
                                    }
                                    else
                                    {
                                        e.Cell.Record.Cells["ck_nt"].Value = 0;
                                        e.Cell.Record.Cells["ck"].Value = 0;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = 0;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = 0;
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region tien2
                        case "tien2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tien2"].Value = 0;
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);

                                    if (ParseInt(e.Cell.Record.Cells["km_ck"].Value, 0) == 1)
                                    {
                                        UpdateTienKM();
                                    }
                                }

                            }
                            break;
                        #endregion

                        #region tien
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tien"].Value = 0;
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                }

                            }
                            break;
                        #endregion

                        #region ck_nt
                        case "ck_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ck_nt"].Value = 0;
                                    //tinh tong ck
                                    decimal ck_nt = ParseDecimal(e.Cell.Record.Cells["ck_nt"].Value, 0);
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "ck_nt", 0);
                                    decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                    //if (sua_tien == 0 && ck != 0)
                                    if (ck != 0)
                                    {
                                        e.Cell.Record.Cells["ck"].Value = ck;
                                        //tinh tong ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"];
                                    }

                                }
                            } break;
                        #endregion

                        #region ck
                        case "ck":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ck"].Value = 0;
                                    //tinh tong ck
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
                                }
                            } break;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region GrdCt_AddNewRecord
        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            return NewRowCt();
        }

        bool NewRowCt()
        {
            try
            {
                DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                NewCtRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
                int Stt_rec0 = 1;
                var vtien_nt0_hd = StartUp.DsTrans.Tables[1].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                .Max(x => x.Field<string>("stt_rec0"));
                if (vtien_nt0_hd != null)
                {
                    int.TryParse(vtien_nt0_hd.ToString(), out Stt_rec0);
                    Stt_rec0++;
                }
                NewCtRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                NewCtRecord["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                NewCtRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                NewCtRecord["so_luong"] = 0;
                NewCtRecord["gia_nt"] = 0;
                NewCtRecord["tien_nt"] = 0;
                NewCtRecord["gia"] = 0;
                NewCtRecord["tien"] = 0;
                NewCtRecord["tl_ck"] = 0;
                NewCtRecord["ck"] = 0;
                NewCtRecord["ck_nt"] = 0;
                NewCtRecord["gia_nt2"] = 0;
                NewCtRecord["tien_nt2"] = 0;
                NewCtRecord["gia2"] = 0;
                NewCtRecord["tien2"] = 0;
                NewCtRecord["ton13"] = DBNull.Value;
                NewCtRecord["km_ck"] = 0;
                int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                if (count > 0)
                {
                    NewCtRecord["ma_kho_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_kho_i"];
                    SqlCommand cmd = new SqlCommand("select ma_dm from dmctct where ma_ct=@ma_ct and ma_dm=@ma_dm");
                    cmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = StartUp.Ma_ct;
                    cmd.Parameters.Add("@ma_dm", SqlDbType.Char).Value = "dmvv";

                    //if (StartUp.SysObj.ExcuteReader(cmd).Tables[0].Rows.Count > 0)
                    //{
                    //    NewCtRecord["ma_vv_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_vv_i"];
                    //}
                }
                FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, StartUp.DsTrans.Tables[1].DefaultView, NewCtRecord, 1);
                StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);

                return true;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
                return false;
            }
        }

        #endregion

        #region GrdCt_RecordDelete
        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            txtMa_thue.SelectAllOnFocus = true;
            txtMa_thue.IsFocus = true;
        }
        #endregion

        #region GrdCt_KeyDown
        private void GrdCt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                NewRowCt();
                GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
            }
            if (Keyboard.IsKeyDown(Key.Tab) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                txtMa_thue.IsFocus = true;
            }
        }
        #endregion

        #region GrdCt_KeyUp
        private void GrdCt_KeyUp(object sender, KeyEventArgs e)
        {
            StartUpTrans.CheckPhanBo(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString());
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    {
                        DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                        if (record == null || record.Cells["ma_vt"].Value == null || record.Cells["ma_vt"].Value.ToString() == "")
                            return;
                        NewRowCt();
                        GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                        GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"];
                    }
                    break;

                #region F5
                case Key.F5:
                    {
                        if (GrdCt.ActiveRecord != null && GrdCt.ActiveRecord.RecordType == RecordType.DataRecord)
                        {
                            CellValuePresenter cellV = CellValuePresenter.FromCell((GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"]);
                            if (cellV != null)
                            {
                                ControlHostEditor controlHost = cellV.Editor as ControlHostEditor;
                                if (controlHost != null)
                                {
                                    AutoCompleteTextBox txt = ControlFunction.GetAutoCompleteControl(controlHost);
                                    if (string.IsNullOrEmpty(txt.Text.Trim()))
                                    {
                                        ExMessageBox.Show(805, StartUp.SysObj, "Chưa nhập mã vật tư!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }
                                    if (txt != null)
                                    {
                                        if (txt.CheckLostFocus())
                                        {
                                            string ma_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"].Value.ToString();
                                            string ten_vt;
                                            if (StartUp.M_LAN.Equals("V"))
                                            {
                                                ten_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ten_vt"].Value.ToString();
                                            }
                                            else
                                            {
                                                ten_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ten_vt2"].Value.ToString();
                                            }
                                            string ma_kho = (GrdCt.ActiveRecord as DataRecord).Cells["ma_kho_i"].Value.ToString();
                                            object ngay_ct = StartUp.DsTrans.Tables[0].Rows[iRow]["ngay_ct"];

                                            DataTable tb = StartUp.GetPN(ma_vt, ma_kho, ngay_ct);
                                            if (tb.Rows.Count > 0)
                                            {
                                                FrmTT_SOCTHDA_HDDT_Pn _frmPn = new FrmTT_SOCTHDA_HDDT_Pn(tb, ten_vt);
                                                _frmPn.ShowDialog();

                                                int currRow = 0;
                                                currRow = GrdCt.ActiveRecord.Index;
                                                DataRowView drvFrmPn;
                                                if (currRow >= 0 && currRow < GrdCt.Records.Count)
                                                {
                                                    drvFrmPn = _frmPn.drvFrmSOCTPNF_PN;
                                                    if (drvFrmPn != null)
                                                    {
                                                        if (StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                                                            (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvFrmPn["gia"];
                                                        else
                                                            (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvFrmPn["gia_nt"];

                                                        (GrdCt.DataSource as DataView)[currRow]["gia"] = drvFrmPn["gia"];

                                                        if (ParseInt((GrdCt.DataSource as DataView)[currRow]["gia_ton"], 0) == 1 || ParseInt((GrdCt.DataSource as DataView)[currRow]["gia_ton"], 0) == 4)
                                                            StartUp.DsTrans.Tables[0].Rows[iRow]["px_gia_dd"] = 1;

                                                        decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);

                                                        int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
                                                        decimal so_luong = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["so_luong"], 0);
                                                        decimal gia_nt = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia_nt"], 0);
                                                        decimal gia = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia"], 0);
                                                        //Co the nhap Tien =Gia*So_luong
                                                        decimal tien_nt = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                                        //if (sua_tien == 0 && tien_nt != 0)
                                                        if (tien_nt != 0)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien_nt"] = tien_nt;
                                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);

                                                        }

                                                        // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                                        // thì tien = tien_nt*ty_gia
                                                        decimal tien = SysFunc.Round(so_luong * gia, StartUp.M_ROUND);
                                                        //if (sua_tien == 0 && tien != 0)
                                                        if (tien != 0)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien"] = tien;
                                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ExMessageBox.Show(810, StartUp.SysObj, "Không có phiếu nhập cho vật tư này!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            ExMessageBox.Show(815, StartUp.SysObj, "Không có phiếu nhập cho vật tư này!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                #endregion

                #region F8
                case Key.F8:
                    {
                        if (ExMessageBox.Show(820, StartUp.SysObj, "Có xoá dòng ghi hiện thời?", "Xoá dòng", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                        if (record != null)
                        {
                            int indexCell = GrdCt.ActiveCell == null ? 0 : GrdCt.ActiveCell.Field.Index;
                            int indexRecord = record.Index;
                            GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                StartUp.DsTrans.Tables[1].Rows.Remove(StartUp.DsTrans.Tables[1].DefaultView[indexRecord].Row);
                                StartUp.DsTrans.Tables[1].AcceptChanges();
                                //tinh lai tong tien
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_sl_km"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 1);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                UpdateTienKM_NT();
                                UpdateTienKM();

                                if (indexRecord == 0 && GrdCt.Records.Count == 0)
                                    GrdCt_AddNewRecord(null, null);
                                if (GrdCt.Records.Count > 0)
                                    GrdCt.ActiveRecord = GrdCt.Records[indexRecord > GrdCt.Records.Count - 1 ? GrdCt.Records.Count - 1 : indexRecord];



                            }
                        }
                    }
                    break;
                #endregion

                default:
                    break;
            }
        }
        #endregion

        #region SumFunction
        decimal SumFunction(DataTable datatable, string columnname, int km)
        {
            decimal result = 0;
            //decimal.TryParse(datatable.Compute("sum(" + columnname + ")", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out result);
            var SumTotal = datatable.AsEnumerable()
                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() && b.Field<string>("km_ck") == km.ToString())
                        .Sum(x => x.Field<decimal?>(columnname));
            if (SumTotal != null)
                result = ParseDecimal(SumTotal, 0);
            // decimal.TryParse(SumTotal.ToString(), out result);
            return result;
        }
        #endregion

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            //string test = StartUp.GetLanguageString(code, language);
            //Debug.WriteLine(string.Format("{0}=>{1}", code, test));
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        #region ParseDecimal
        public decimal ParseDecimal(object obj, decimal defaultvalue)
        {
            decimal ketqua = defaultvalue;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

        #region ParseInt
        public int ParseInt(object obj, int defaultvalue)
        {
            int ketqua = defaultvalue;
            int.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

        #region ChkPx_gia_dd_Click
        private void ChkPx_gia_dd_Click(object sender, RoutedEventArgs e)
        {
            IsVisibilityFieldsXamDataGridByPx_gia_dd();
        }
        #endregion

        #region ChkSua_tkthue_Click
        private void ChkSua_tkthue_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSua_tkthue.IsChecked == true)
                txtTk_thue_no.IsFocus = true;
        }
        #endregion

        #region ChkSua_thue_Click
        private void ChkSua_thue_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSua_thue.IsChecked == true)
                txtT_thue_nt.Focus();
            UpdateTongThue();
        }
        #endregion

        #region ChkTinh_ck_Click
        private void ChkTinh_ck_Click(object sender, RoutedEventArgs e)
        {
            UpdateTongThue();
        }
        #endregion

        #region txtt_tien_nt2_ValueChanged
        private void txtt_tien_nt2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsInEditMode.Value)
                UpdateMoney_NT();
        }
        #endregion

        #region UpdateMoney_NT
        void UpdateMoney_NT()
        {
            decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);
            int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
            int tinh_ck = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["tinh_ck"], 0);
            int sua_thue = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"], 0);

            decimal t_tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"], 0);
            decimal t_ck_nt = SumFunction(StartUp.DsTrans.Tables[1], "ck_nt", 0);
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck_nt"] = t_ck_nt;

            decimal t_tien_sau_ck_nt = t_tien_nt2 - t_ck_nt;
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_sau_ck_nt"] = t_tien_sau_ck_nt;

            decimal t_thue_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"], 0);
            if (sua_thue == 0)
            {
                if (tinh_ck == 1)
                    t_thue_nt = SysFunc.Round((t_tien_nt2 * thue_suat) / 100, StartUp.M_ROUND_NT);
                else
                    t_thue_nt = SysFunc.Round((t_tien_sau_ck_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_nt"] = t_thue_nt;

                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"] = t_thue_nt;
            }

            //tong thanh toan
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt_nt"] = t_thue_nt + t_tien_sau_ck_nt;

            decimal t_tien2 = SysFunc.Round(t_tien_nt2 * ty_gia, StartUp.M_ROUND);
            //if (t_tien2 != 0 && sua_tien == 0)
            //if (t_tien2 != 0)
            //{
            //    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = t_tien2;
            //}


        }
        #endregion

        #region UpdateMoney
        void UpdateMoney()
        {
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);
            int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
            int tinh_ck = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["tinh_ck"], 0);
            int sua_thue = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"], 0);

            decimal t_tien2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"], 0);
            decimal t_ck = SumFunction(StartUp.DsTrans.Tables[1], "ck", 0);
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_ck"] = t_ck;

            decimal t_tien_sau_ck = t_tien2 - t_ck;
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_sau_ck"] = t_tien_sau_ck;

            decimal t_thue = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"], 0);

            if (sua_thue == 0)
            {
                if (tinh_ck == 1)
                    t_thue = SysFunc.Round((t_tien2 * thue_suat) / 100, StartUp.M_ROUND);
                else
                    t_thue = SysFunc.Round((t_tien_sau_ck * thue_suat) / 100, StartUp.M_ROUND);
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"] = t_thue;
            }

            //tong thanh toan
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt"] = t_thue + t_tien_sau_ck;

        }
        #endregion

        #region txtt_tien2_ValueChanged
        private void txtt_tien2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsInEditMode.Value)
                UpdateMoney();
        }
        #endregion

        #region txtt_ck_nt_ValueChanged
        private void txtt_ck_nt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsInEditMode.Value)
                UpdateMoney_NT();
        }
        #endregion

        #region txtt_ck_ValueChanged
        private void txtt_ck_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsInEditMode.Value)
                UpdateMoney();
        }
        #endregion

        #region txtT_thue_nt_ValueChanged
        private void txtT_thue_nt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateTongTT_NT();
        }
        #endregion

        #region txtT_thue_ValueChanged
        private void txtT_thue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateTongTT();
        }
        #endregion

        #region txtThue_suat_ValueChanged
        private void txtThue_suat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsInEditMode.Value)
            {
                UpdateTongThue();
                UpdateThueKM();
            }
        }
        #endregion

        #region txtMa_thue_PreviewLostFocus
        private void txtMa_thue_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_thue.RowResult != null && txtMa_thue.IsDataChanged)
            {
                txtThue_suat.Value = ParseDecimal(txtMa_thue.RowResult["thue_suat"], 0);
                txtTk_thue_co.Text = txtMa_thue.RowResult["tk_thue_co"].ToString().Trim();

                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tk_co"] = txtMa_thue.RowResult["loai_tk_co"].ToString().Trim();

                StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_co_dmthue"] = txtMa_thue.RowResult["tk_thue_co"].ToString().Trim();
                txtTk_thue_co.SearchInit();
                if (txtTk_thue_co.RowResult == null || string.IsNullOrEmpty(txtTk_thue_co.Text.Trim()))
                    return;
                StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_co_cn"] = txtTk_thue_co.RowResult["tk_cn"];
            }
        }
        #endregion

        #region UpdateTongTT_NT
        void UpdateTongTT_NT()
        {
            decimal t_thue_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"], 0);
            decimal t_tien_sau_ck_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck_nt"], 0);
            //tong thanh toan
            //  decimal t_tt_nt = t_thue_nt + t_tien_sau_ck_nt;
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt_nt"] = t_thue_nt + t_tien_sau_ck_nt;//t_tt_nt;
            //decimal t_tien_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km_nt"], 0);
            //decimal t_thue_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km_nt"], 0);
            ////tổng cộng
            //StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc_nt"] = t_tt_nt + t_tien_km_nt + t_thue_km_nt;
            if (ChkSua_thue.IsChecked == true)
            {
                decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["ty_gia"], 0);
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"] = SysFunc.Round(t_thue_nt * ty_gia, StartUp.M_ROUND);
            }
        }
        #endregion

        #region UpdateTongTT
        void UpdateTongTT()
        {
            decimal t_thue = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"], 0);
            decimal t_tien_sau_ck = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck"], 0);
            //tong thanh toan
            // decimal t_tt = t_thue + t_tien_sau_ck;
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt"] = t_thue + t_tien_sau_ck;//t_tt;
            //decimal t_tien_km = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"], 0);
            //decimal t_thue_km = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"], 0);
            ////tổng cộng
            //StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc"] = t_tt + t_tien_km + t_thue_km;
        }
        #endregion

        #region UpdateTongThue
        void UpdateTongThue()
        {
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);
            int tinh_ck = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["tinh_ck"], 0);
            int sua_thue = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"], 0);

            decimal t_thue_nt = 0, t_thue = 0;
            if (tinh_ck == 1)
            {
                decimal t_tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"], 0);
                decimal t_tien2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"], 0);
                t_thue_nt = SysFunc.Round((t_tien_nt2 * thue_suat) / 100, StartUp.M_ROUND_NT);
                t_thue = SysFunc.Round((t_tien2 * thue_suat) / 100, StartUp.M_ROUND);
            }
            else
            {
                decimal t_tien_sau_ck_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck_nt"], 0);
                decimal t_tien_sau_ck = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck"], 0);
                t_thue_nt = SysFunc.Round((t_tien_sau_ck_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                t_thue = SysFunc.Round((t_tien_sau_ck * thue_suat) / 100, StartUp.M_ROUND);
            }

            if (sua_thue == 0)
            {
                //  if ( t_thue_nt != 0 || StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_nt"] = t_thue_nt;
                // if ( t_thue != 0 || StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"] = t_thue;
            }
        }
        #endregion

        #region UpdateGia
        //Giá được update trong store Post
        //private void UpdateGia()
        //{
        //    try
        //    {
        //        if (StartUp.M_UPDATE_GIA2 == 0)
        //            return;
        //        StartUp.UpdateGia(StartUp.Ma_ct, StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //}
        #endregion

        #region UpdateTonKho
        private void UpdateTonKho()
        {
            string lstma_kho = "";
            string lstma_vt = "";
            string lstma_vv = "";
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["vt_ton_kho"], 0) == 1)
                {
                    //decimal ton_moi = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString());
                    //StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = ton_moi;
                    lstma_kho += ";" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim();
                    lstma_vt += ";" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim();
                    lstma_vv += ";" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vv_i"].ToString().Trim();
                }
                else
                    StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = DBNull.Value;
            }
            if (!string.IsNullOrEmpty(lstma_kho) && !string.IsNullOrEmpty(lstma_vt))
            {
                lstma_kho = lstma_kho.Substring(1);
                lstma_vt = lstma_vt.Substring(1);
                lstma_vv = lstma_vv.Substring(1);
                DataTable dt = InvtLib.InFuncLib.GetListTon13(StartUp.SysObj, lstma_kho, lstma_vt, lstma_vv);
                if (dt != null)
                    for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                    {
                        DataRow[] _listdr = dt.Select("ma_kho LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim() + "' AND ma_vt LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim() + "' AND ma_vv LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vv_i"].ToString().Trim() + "'");
                        if (StartUp.SysObj.GetOption("M_TON_KHO13").ToString().Trim().Equals("1"))
                        {
                            _listdr = dt.Select("ma_kho LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim() + "' AND ma_vt LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim() + "'");
                        }
                        if (_listdr.Length > 0)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = _listdr[0]["ton13"];
                        }
                        else
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = 0;
                    }
            }
            StartUp.DsTrans.Tables[1].AcceptChanges();

        }
        #endregion

        #region GetTonKho
        decimal GetTonKho(string ma_kho, string ma_vt, object ngay_ct)
        {
            decimal tonkho = 0;
            try
            {
                string advance = "ma_kho = '" + ma_kho + "'";
                advance += " AND ma_vt = '" + ma_vt + "'";
                SqlCommand cmd = new SqlCommand("exec CheckTonXuatAm 1, @ngay_ct, @stt_rec, @advance");
                cmd.Parameters.Add("@ngay_ct", SqlDbType.SmallDateTime).Value = ngay_ct;
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char).Value = StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"];
                cmd.Parameters.Add("@advance", SqlDbType.VarChar).Value = advance;
                DataTable dt = StartUp.SysObj.ExcuteReader(cmd).Tables[0];
                if (dt.Rows.Count > 0)
                    tonkho = ParseDecimal(dt.Rows[0]["ton00"], 0);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return tonkho;
        }
        #endregion

        #region btnThongTin_Click
        private void btnThongTin_Click(object sender, RoutedEventArgs e)
        {
            FrmTotalInfo formTotal = new FrmTotalInfo();
            formTotal.Title = StartUp.M_Tilte;
            formTotal.ShowDialog();
        }
        #endregion

        #region UpdateTienKM_NT
        void UpdateTienKM_NT()
        {
            decimal t_tien_km_nt = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 1);
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);

            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km_nt"] = t_tien_km_nt;

            decimal t_thue_km_nt = 0;
            if (StartUp.M_THUE_KM_CK == 1)
                t_thue_km_nt = SysFunc.Round((t_tien_km_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
            //if (t_thue_km_nt != 0)
            //    StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km_nt"] = t_thue_km_nt;

            //t_thue_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_km_nt"], 0);
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km_nt"] = t_thue_km_nt;
            decimal t_tt_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt_nt"], 0);
            //tổng cộng
            StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc_nt"] = t_tt_nt + t_tien_km_nt + t_thue_km_nt;

        }
        #endregion

        #region UpdateTienKM
        void UpdateTienKM()
        {
            decimal t_tien_km = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 1);
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);

            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"] = t_tien_km;
            decimal t_thue_km = 0;
            if (StartUp.M_THUE_KM_CK == 1)
                t_thue_km = SysFunc.Round((t_tien_km * thue_suat) / 100, StartUp.M_ROUND);
            //if (t_thue_km != 0)
            //    StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"] = t_thue_km;
            //t_thue_km = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_km"], 0);
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"] = t_thue_km;
            decimal t_tt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt"], 0);
            //tổng cộng
            StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc"] = t_tt + t_tien_km + t_thue_km;
        }
        #endregion

        #region UpdateThueKM
        void UpdateThueKM()
        {
            decimal t_tien_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_km_nt"], 0);
            decimal t_tien_km = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_km"], 0);
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);

            decimal t_thue_km_nt = 0, t_thue_km = 0;
            if (StartUp.M_THUE_KM_CK == 1)
            {
                t_thue_km_nt = SysFunc.Round((t_tien_km_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                t_thue_km = SysFunc.Round((t_tien_km * thue_suat) / 100, StartUp.M_ROUND);
            }
            //if (t_thue_km_nt != 0)
            //    StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km_nt"] = t_thue_km_nt;
            //if (t_thue_km != 0)
            //    StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"] = t_thue_km;
            //t_thue_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_km_nt"], 0);
            //t_thue_km = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_km"], 0);
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km_nt"] = t_thue_km_nt;
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"] = t_thue_km;

            decimal t_tt_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt_nt"], 0);
            decimal t_tt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt"], 0);
            //tổng cộng
            StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc_nt"] = t_tt_nt + t_tien_km_nt + t_thue_km_nt;
            StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc"] = t_tt + t_tien_km + t_thue_km;
        }

        #endregion

        #region txtT_thue_LostFocus
        private void txtT_thue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtT_thue.IsFocusWithin && IsInEditMode.Value)
            {
                if (txtT_thue.Value == null || txtT_thue.Value.ToString() == "")
                    txtT_thue.Value = 0;
            }
        }
        #endregion

        #region txtT_thue_nt_LostFocus
        private void txtT_thue_nt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtT_thue_nt.IsFocusWithin && IsInEditMode.Value)
            {
                if (txtT_thue_nt.Value == null || txtT_thue_nt.Value.ToString() == "")
                    txtT_thue_nt.Value = 0;
                if (StartUp.M_ma_nt0 == txtMa_nt.Text.Trim())
                {
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_nt"];
                }
            }
        }
        #endregion

        #region txtt_so_luong_ValueChanged
        private void txtt_so_luong_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            StartUp.DsTrans.Tables[0].Rows[iRow]["t_sl_km"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 1);
        }
        #endregion

        #region txtma_bp_GotFocus
        private void txtma_bp_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value)
            {
                if (StartUp.M_BP_BH == 0)
                    SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
        #endregion

        #region txtTk_thue_no_GotFocus
        private void txtTk_thue_no_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ChkSua_tkthue.IsChecked == false)
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
        }
        #endregion

        #region txtMa_kh2_GotFocus
        private void txtMa_kh2_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_co_cn"].ToString() == "0")
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
        }
        #endregion

        #region txtma_bp_PreviewLostFocus
        private void txtma_bp_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtma_bp.RowResult == null)
                    txtTen_bp.Text = "";
                else
                    txtTen_bp.Text = StartUp.M_LAN == "V" ? txtma_bp.RowResult["ten_bp"].ToString() : txtma_bp.RowResult["ten_bp2"].ToString();
            }
        }
        #endregion

        #region txtTk_thue_co_PreviewGotFocus
        private void txtTk_thue_co_PreviewGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //string tk_thue_co_dmthue = StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_co_dmthue"].ToString().Trim();
            //if (tk_thue_co_dmthue != "" || StartUp.IsTkMe(tk_thue_co_dmthue))
            //    SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            if (txtTk_thue_co.IsReadOnly == true)
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
        }
        #endregion

        #region FormMain_Closed
        private void FormMain_Closed(object sender, EventArgs e)
        {
            //if currActinTask đang là none thì thoát luôn
            //nếu không phải là None thì Hủy phiếu đang thực hiện
            if (currActionTask == ActionTask.None || IsInEditMode.Value == false)
                return;
            V_Huy();
        }
        #endregion

        #region PhanBoThueInCT_CBTien
        void PhanBoThueInCT_CBTien()
        {
            try
            {
                if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                    return;

                decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                string stt_rec = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();

                decimal t_thue_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"], 0);
                decimal t_thue = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"], 0);
                decimal t_tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"], 0);
                decimal t_tien2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"], 0);

                decimal _t_tien = t_tien2;
                decimal _t_tien_nt = t_tien_nt2;
                //tinh thue sau c.khau
                if (ChkTinh_ck.IsChecked == false)
                {
                    _t_tien = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck"], 0);
                    _t_tien_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck_nt"], 0);
                }

                decimal t_thue_km_nt = 0, t_thue_km = 0;
                decimal t_tien_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_km_nt"], 0);
                decimal t_tien_km = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_km"], 0);
                if (StartUp.M_THUE_KM_CK == 1)
                {
                    t_thue_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_km_nt"], 0);
                    t_thue_km = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_km"], 0);
                }

                decimal t_tien2_temp = SysFunc.Round(t_tien_nt2 * ty_gia, StartUp.M_ROUND);
                decimal t_tien_km_temp = SysFunc.Round(t_tien_km_nt * ty_gia, StartUp.M_ROUND);

                decimal tien_nt2 = 0, tien2 = 0;
                decimal thue_nt = 0, thue = 0;

                bool CanBang = false, CanBangKM = false;
                if (ty_gia == 0 || ChkSua_tien.IsChecked == true)
                {
                    CanBang = true;
                    CanBangKM = true;
                }
                int indexThue = -1, indexThue_km = -1;
                decimal _t_thue = 0, _t_thue_nt = 0, _t_thue_km = 0, _t_thue_km_nt = 0;
                for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                {

                    tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"], 0);
                    tien2 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"], 0);


                    if (tien_nt2 != 0 && tien2 != 0)
                    {
                        #region can bang tien
                        if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"], 0) == 0 && CanBang == false)
                        {
                            tien2 += t_tien2_temp - t_tien2;
                            t_tien2 = t_tien2_temp;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"] = tien2;
                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = t_tien2;
                            CanBang = true;
                        }
                        #endregion

                        #region can bang tien km
                        else if (CanBangKM == false)
                        {
                            tien2 += t_tien_km_temp - t_tien_km;
                            t_tien_km = t_tien_km_temp;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"] = tien2;
                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"] = t_tien_km;
                            CanBangKM = true;
                        }
                        #endregion
                    }


                    if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"], 0) == 0)
                    {
                        #region tinh thue cac vat tu
                        decimal _tien = 0, _tien_nt = 0;

                        //tinh thue truoc c.khau
                        if (ChkTinh_ck.IsChecked == true)
                        {
                            _tien = tien2;
                            _tien_nt = tien_nt2;
                        }
                        //tinh thue sau c.khau
                        else
                        {
                            _tien = tien2 - ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ck"], 0);
                            _tien_nt = tien_nt2 - ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ck_nt"], 0);
                        }
                        //nếu loại tiền là ngoại tệ
                        if (txtMa_nt.Text != StartUp.M_ma_nt0)
                        {
                            //nếu tiền nguyên tệ = 0
                            if (_tien_nt == 0)
                            {
                                thue_nt = 0;
                            }
                            else
                            {
                                thue_nt = _t_tien_nt == 0 ? 0 : SmLib.SysFunc.Round((_tien_nt / _t_tien_nt) * t_thue_nt, StartUp.M_ROUND_NT);
                            }

                            //nếu tiền = 0
                            if (_tien == 0)
                            {
                                thue = 0;
                            }
                            else
                            {
                                thue = _t_tien == 0 ? 0 : SmLib.SysFunc.Round((_tien / _t_tien) * t_thue, StartUp.M_ROUND);
                            }
                        }
                        else
                        {
                            thue_nt = _t_tien_nt == 0 ? 0 : SmLib.SysFunc.Round((_tien_nt / _t_tien_nt) * t_thue_nt, StartUp.M_ROUND_NT);
                            thue = _t_tien == 0 ? 0 : SmLib.SysFunc.Round((_tien / _t_tien) * t_thue, StartUp.M_ROUND);
                        }
                        StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = thue_nt;
                        StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = thue;
                        _t_thue += thue;
                        _t_thue_nt += thue_nt;
                        if (indexThue == -1)
                            indexThue = i;
                        #endregion
                    }
                    else
                    {
                        #region tinh thue vat tu km
                        if (StartUp.M_THUE_KM_CK == 1)
                        {
                            //nếu loại tiền là ngoại tệ
                            if (txtMa_nt.Text != StartUp.M_ma_nt0)
                            {
                                //nếu tiền nguyên tệ = 0
                                if (tien_nt2 == 0)
                                {
                                    thue_nt = 0;
                                }
                                else
                                {
                                    thue_nt = t_tien_km_nt == 0 ? 0 : SmLib.SysFunc.Round((tien_nt2 / t_tien_km_nt) * t_thue_km_nt, StartUp.M_ROUND_NT);
                                }

                                //nếu tiền ngoại tệ = 0
                                if (tien2 == 0)
                                {
                                    thue = 0;
                                }
                                else
                                {
                                    thue = t_tien_km == 0 ? 0 : SmLib.SysFunc.Round((tien2 / t_tien_km) * t_thue_km, StartUp.M_ROUND);
                                }
                            }
                            else
                            {
                                thue_nt = t_tien_km_nt == 0 ? 0 : SmLib.SysFunc.Round((tien_nt2 / t_tien_km_nt) * t_thue_km_nt, StartUp.M_ROUND_NT);
                                thue = t_tien_km == 0 ? 0 : SmLib.SysFunc.Round((tien2 / t_tien_km) * t_thue_km, StartUp.M_ROUND);
                            }
                            StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = thue_nt;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = thue;
                            _t_thue_km += thue;
                            _t_thue_km_nt += thue_nt;
                        }
                        else
                        {
                            StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = 0;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = 0;
                        }
                        if (indexThue_km == -1)
                            indexThue_km = i;
                        #endregion
                    }

                }

                #region can bang thue trong ct
                if (indexThue != -1)
                {
                    if (t_thue != _t_thue)
                        StartUp.DsTrans.Tables[1].DefaultView[indexThue]["thue"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[indexThue]["thue"], 0) + (t_thue - _t_thue);
                    if (t_thue_nt != _t_thue_nt)
                        StartUp.DsTrans.Tables[1].DefaultView[indexThue]["thue_nt"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[indexThue]["thue_nt"], 0) + (t_thue_nt - _t_thue_nt);
                }
                #endregion

                #region can bang thue km trong ct
                if (indexThue_km != -1)
                {
                    if (t_thue_km != _t_thue_km)
                        StartUp.DsTrans.Tables[1].DefaultView[indexThue_km]["thue"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[indexThue_km]["thue"], 0) + (t_thue_km - _t_thue_km);
                    if (t_thue_km_nt != _t_thue_km_nt)
                        StartUp.DsTrans.Tables[1].DefaultView[indexThue_km]["thue_nt"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[indexThue_km]["thue_nt"], 0) + (t_thue_km_nt - _t_thue_km_nt);
                }
                #endregion

                StartUp.DsTrans.Tables[0].AcceptChanges();
                StartUp.DsTrans.Tables[1].AcceptChanges();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region txtT_tt_nt_ValueChanged
        private void txtT_tt_nt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            decimal t_tt_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt_nt"], 0);
            decimal t_tien_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km_nt"], 0);
            decimal t_thue_km_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km_nt"], 0);
            //tổng cộng
            StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc_nt"] = t_tt_nt + t_tien_km_nt + t_thue_km_nt;
        }
        #endregion

        #region txtT_tt_ValueChanged
        private void txtT_tt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            decimal t_tt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tt"], 0);
            decimal t_tien_km = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_km"], 0);
            decimal t_thue_km = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_km"], 0);
            //tổng cộng
            StartUp.DsTrans.Tables[0].Rows[iRow]["tien_tc"] = t_tt + t_tien_km + t_thue_km;
        }
        #endregion

        #region txtHan_tt_GotFocus
        private void txtHan_tt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHan_tt.SelectAll();
        }
        #endregion

        #region txtHt_tt_LostFocus
        private void txtHt_tt_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            }));
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value)
            {
                try
                {
                    FrmTT_SOCTHDA_HDDTHdm form = new FrmTT_SOCTHDA_HDDTHdm();
                    form.txtMa_kh.Text = txtMa_kh.Text;
                    form.tblTen_kh.Text = txtTen_kh.Text;
                    form.ShowDialog();
                    if (form.isOk)
                    {
                        int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                        for (int i = 0; i < count; i++)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView.Delete(0);
                        }
                        StartUp.DsTrans.Tables[1].AcceptChanges();
                        string ma_ntPH = txtMa_nt.Text.ToUpper();
                        string ma_ntHD = form.dsHdm.Tables[0].DefaultView[0]["ma_nt"].ToString().ToUpper();

                        for (int i = 0; i < form.dsHdm.Tables[1].DefaultView.Count; i++)
                        {
                            DataRow rowHdm = form.dsHdm.Tables[1].DefaultView[i].Row;
                            DataRow NewRecord = StartUp.DsTrans.Tables[1].NewRow();

                            DataTable dt1 = form.dsHdm.Tables[1].Clone();
                            dt1.Rows.Add(rowHdm.ItemArray);
                            DataTable dt2 = StartUp.DsTrans.Tables[1].Clone();
                            dt2.Merge(dt1, true, MissingSchemaAction.Ignore);

                            //NewRecord.ItemArray = rowHdm.ItemArray;
                            if (dt2.Rows.Count > 0)
                            {
                                NewRecord.ItemArray = dt2.Rows[0].ItemArray;
                            }
                            //NewRecord["ton13"] = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, NewRecord["ma_kho_i"].ToString(), NewRecord["ma_vt"].ToString(), NewRecord["ma_vv_i"].ToString());
                            decimal tl_ck = 0;
                            decimal.TryParse(rowHdm["tl_ck"].ToString(), out tl_ck);

                            decimal so_luong = 0;
                            decimal.TryParse(rowHdm["so_luong"].ToString(), out so_luong);
                            //trường hợp ma_nt của phiếu và hợp đồng giống nhau
                            if (ma_ntPH.Equals(ma_ntHD))
                            {
                                //Bằng ma_nt gốc
                                if (ma_ntPH.Equals(StartUp.M_ma_nt0))
                                {
                                    NewRecord["gia_nt2"] = rowHdm["gia2"];
                                    NewRecord["gia2"] = rowHdm["gia2"];
                                    NewRecord["tien_nt2"] = rowHdm["tien2"];
                                    NewRecord["tien2"] = rowHdm["tien2"];
                                    NewRecord["ck_nt"] = rowHdm["ck_nt"];
                                    NewRecord["ck"] = rowHdm["ck"];
                                }
                                //Khác ma_nt gốc
                                else
                                {
                                    NewRecord["gia_nt2"] = rowHdm["gia_nt2"];
                                    NewRecord["gia2"] = SmLib.SysFunc.Round(Convert.ToDecimal(NewRecord["gia_nt2"]) * txtTy_gia.nValue, StartUp.M_ROUND_GIA);
                                    NewRecord["tien_nt2"] = rowHdm["tien_nt2"];
                                    NewRecord["tien2"] = SmLib.SysFunc.Round(Convert.ToDecimal(NewRecord["gia2"]) * so_luong, StartUp.M_ROUND);
                                    NewRecord["ck_nt"] = rowHdm["ck_nt"];
                                    NewRecord["ck"] = SmLib.SysFunc.Round(Convert.ToDecimal(NewRecord["tien2"]) * tl_ck / 100, StartUp.M_ROUND);
                                }
                            }
                            //trường hợp ma_nt khác nhau
                            else
                            {
                                //ma_nt trong phiếu bằng ma_nt gốc
                                if (ma_ntPH.Equals(StartUp.M_ma_nt0))
                                {
                                    NewRecord["gia_nt2"] = rowHdm["gia2"];
                                    NewRecord["gia2"] = rowHdm["gia2"];
                                    NewRecord["tien_nt2"] = rowHdm["tien2"];
                                    NewRecord["tien2"] = rowHdm["tien2"];
                                    NewRecord["ck_nt"] = rowHdm["ck_nt"];
                                    NewRecord["ck"] = rowHdm["ck"];
                                }
                                else
                                {
                                    NewRecord["gia_nt2"] = SysFunc.Round(Convert.ToDecimal(rowHdm["gia2"]) / txtTy_gia.nValue, StartUp.M_ROUND_GIA_NT);
                                    NewRecord["gia2"] = rowHdm["gia2"];
                                    NewRecord["tien_nt2"] = SysFunc.Round(Convert.ToDecimal(NewRecord["gia_nt2"]) * so_luong, StartUp.M_ROUND_NT);
                                    NewRecord["tien2"] = rowHdm["tien2"];
                                    NewRecord["ck_nt"] = SmLib.SysFunc.Round(Convert.ToDecimal(NewRecord["tien_nt2"]) * tl_ck / 100, StartUp.M_ROUND_NT);
                                    NewRecord["ck"] = rowHdm["ck"];
                                }
                            }
                            NewRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];

                            int Stt_rec0 = 0, Stt_rec0ct = 0;
                            if (GrdCt.Records.Count > 0)
                            {
                                var _max_sttrec0ct = StartUp.DsTrans.Tables[1].AsEnumerable()
                                                   .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                                   .Max(x => x.Field<string>("stt_rec0"));
                                if (_max_sttrec0ct != null)
                                    int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
                            }

                            Stt_rec0 = Stt_rec0ct;
                            Stt_rec0++;

                            NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                            NewRecord["km_ck"] = 0;
                            NewRecord["ma_hd_i"] = rowHdm["ma_hd"];
                            StartUp.DsTrans.Tables[1].Rows.Add(NewRecord);
                        }

                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);
                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);

                        UpdateTonKho();
                        UpdateMoney_NT();
                        UpdateMoney();
                        GrdCt.Focus();
                    }
                }
                catch (Exception ex)
                {
                    SmErrorLib.ErrorLog.CatchMessage(ex);
                }
            }
        }

        private void txtSo_seri_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtSo_seri.Text = txtSo_seri.Text.Trim();
        }

        private void txtma_thck_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtma_thck.RowResult != null)
                if (txtHan_tt.Value == DBNull.Value || txtHan_tt.nValue == 0)
                {
                    txtHan_tt.Value = txtma_thck.RowResult["han_tt"];
                }
        }

        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
        }

        private void txtHt_tt_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtHt_tt.SelectionStart = 0;
            txtHan_tt.SelectionLength = 0;
        }

        private void btnViewPT1_Click(object sender, RoutedEventArgs e)
        {
            //Kiểm tra PT1 có tồn tại hay không? nếu không thì reset lại stt_rec_pt bằng rỗng
            if (string.IsNullOrEmpty(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim()))
            {
                return;
            }
            if (StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Trim().Equals("PT1"))
            {
                SqlCommand _cmd = new SqlCommand();
                _cmd.CommandText = "Select count(1) from ph41 WHERE stt_Rec = @stt_rec_pt";
                _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString();
                if ((int)BindingSysObj.ExcuteScalar(_cmd) == 0)
                {
                    if (ExMessageBox.Show(693, StartUp.SysObj, "Phiếu thu không tồn tại, có xóa thông tin phiếu thu trên hóa đơn không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        _cmd = new SqlCommand();
                        _cmd.CommandText = "UPDATE ph81 Set stt_rec_pt = '', so_ct_pt = '', ma_ct_pt = '' WHERE stt_rec = @stt_rec_pt; ";
                        _cmd.CommandText += "UPDATE cttt20 Set tat_toan = 0, stt_rec_tt = '' WHERE stt_rec = @stt_rec";
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        BindingSysObj.ExcuteNonQuery(_cmd);
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"] = "";
                    }
                }
                else
                    SysFunc.EditVoucherFromBrowse(BindingSysObj, "PT1", StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString(), System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), BindingSysObj.M_ProcessName);
            }
            else
            {
                SqlCommand _cmd = new SqlCommand();
                _cmd.CommandText = "Select count(1) from ph51 WHERE stt_Rec = @stt_rec_pt";
                _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString();
                if ((int)BindingSysObj.ExcuteScalar(_cmd) == 0)
                {
                    if (ExMessageBox.Show(693, StartUp.SysObj, "Phiếu thu không tồn tại, có xóa thông tin phiếu thu trên hóa đơn không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        _cmd = new SqlCommand();
                        _cmd.CommandText = "UPDATE ph81 Set stt_rec_pt = '', so_ct_pt = '', ma_ct_pt = '' WHERE stt_rec = @stt_rec_pt; ";
                        _cmd.CommandText += "UPDATE cttt20 Set tat_toan = 0, stt_rec_tt = '' WHERE stt_rec = @stt_rec";
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        BindingSysObj.ExcuteNonQuery(_cmd);
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"] = "";
                    }
                }
                else
                    SysFunc.EditVoucherFromBrowse(BindingSysObj, "BC1", StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString(), System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), BindingSysObj.M_ProcessName);
            }
        }

        private void txtMa_kh_PreviewGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ma_kh_old = txtMa_kh.Text.Trim();
        }

        private void btnHuyHD_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                {
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "0" || StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "8")
                    {
                        ExMessageBox.Show(1447, StartUp.SysObj, "Hóa đơn chưa phát hành Hoặc Hóa đơn đã hủy.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                        //Huy hoa don phat hanh
                        HddtLib.Interface.IPublishInvoice handler = HddtLib.Utils.CreatePublishHandler();
                        HddtLib.Interface.FunctionResult<InvoiceInfo> result = handler.CancelSignedInvoice(_stt_rec, "HDA", null);
                        if (result.Status == ResultCode.Success)
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = "Update Ph81 set tt_hddt_tcn = '8' where stt_rec = '" + _stt_rec + "'";
                            StartUp.SysObj.ExcuteNonQuery(cmd);
                            //Refresh lại Table[0], Table[1]
                            if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                            {
                               // txtNgay_ct.Text = "";
                                iRow = iRow > StartUp.DsTrans.Tables[0].Rows.Count ? iRow : iRow;
                                //load lại form theo stt_rec
                                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
                            }
                           // HddtLib.Utils.UpdateHddtInfo(StartUp.Ma_ct, _stt_rec, result.OtherInfo as HddtLib.PublishInvoiceInfo, result.Data);
                            PostAfterPublish(StartUp.Ma_ct, _stt_rec, 8);

                            StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"] = 8;
                            StartUp.DsTrans.Tables[0].DefaultView[0]["status"] = "0";
                           
                            ExMessageBox.Show(9005, StartUp.SysObj, "Hủy phát hành hóa đơn điện tử thành công.\nTrạng thái hóa đơn chuyển sang chưa ghi sổ", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi thực hiện hủy phát hành hóa đơn điện tử\n" + result.Message);
                        }

                       

                    }
                }
            }
        }
        private void btnThayTheHD_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                {
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "0" || StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "8")
                    {
                        ExMessageBox.Show(1447, StartUp.SysObj, "Hóa đơn chưa phát hành Hoặc Hóa đơn đã hủy.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        flag_button = "5";
                        _so_ct_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim();
                        _stt_rec_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                        _ma_kh_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString().Trim();
                        V_Moi();
                    }
                }
            }
        }
        private void btnDCTangHD_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                {
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "0" || StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "8")
                    {
                        ExMessageBox.Show(1447, StartUp.SysObj, "Hóa đơn chưa phát hành Hoặc Hóa đơn đã hủy.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        flag_button = "3";
                        _so_ct_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim();
                        _stt_rec_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                        _ma_kh_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString().Trim();
                        V_Moi();
                    }
                }


            }
        }

        private void btnDCGiamHD_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                {
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "0" || StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "8")
                    {
                        ExMessageBox.Show(1447, StartUp.SysObj, "Hóa đơn chưa phát hành Hoặc Hóa đơn đã hủy.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        flag_button = "4";
                        _so_ct_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim();
                        _stt_rec_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                        _ma_kh_tt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString().Trim();
                        V_Moi();
                    }
                }


            }
        }

        private void btnDCTTHD_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                {
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "0" || StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() == "8")
                    {
                        ExMessageBox.Show(1447, StartUp.SysObj, "Hóa đơn chưa phát hành Hoặc Hóa đơn đã hủy.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        flag_button = "2";
                        V_Sua();
                    }
                }
            }
        }

        private void btnPHHDCho_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() != "0")
                    {
                        ExMessageBox.Show(1446, StartUp.SysObj, "Hóa đơn đã phát hành.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "0")
                        {
                            //Chay phat hanh hoa don dien tu => trả lại giá trị
                            //Update Ph81: 
                            //Truong hop 1: Lập mới phát hành
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)                        
                        }
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "3")
                        {
                            //Goi Dieu chinh tang hoa don
                            ////Update Ph81: 
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            // + Update tt_hddt_tcn của hóa đơn điều chỉnh/thay thế về trạng thái = 3 lọc theo stt_rec_tt 
                        }
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "4")
                        {
                            //Goi Dieu chinh giảm hoa don
                            ////Update Ph81: 
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            // + Update tt_hddt_tcn của hóa đơn điều chỉnh/thay thế về trạng thái = 4 lọc theo stt_rec_tt 
                        }
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "5")
                        {
                            //goi Thay the hoa don
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            // + Update tt_hddt_tcn của hóa đơn thay thế về trạng thái = 8 - Hủy hóa đơn tương ứng lọc theo stt_rec_tt

                        }
                    }
            }
        }

        private void btnPH_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                ExMessageBox.Show(1460, StartUp.SysObj, "Phải lưu chứng từ rồi mới thực hiện!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                else
                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"].ToString().Trim() != "0")
                    {
                        ExMessageBox.Show(1446, StartUp.SysObj, "Hóa đơn đã phát hành.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "0" || StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim()=="")
                        {
                            //Chay phat hanh hoa don dien tu => trả lại giá trị
                            //Update Ph81: 
                            //Truong hop 1: Lập mới phát hành
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)                        
                            string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();

                            HddtLib.Interface.IPublishInvoice handler = HddtLib.Utils.CreatePublishHandler();
                            HddtLib.Interface.FunctionResult<InvoiceInfo> result = handler.PublishAndSignInvoice(_stt_rec, "HDA", null);
                            if (result.Status == ResultCode.Success)
                            {
                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandText = "Update Ph81 set tt_hddt_tcn = '1', user_id_ph = '" + StartUp.M_User_Id + "' where stt_rec = '" + _stt_rec + "'";
                                StartUp.SysObj.ExcuteNonQuery(cmd);
                                HddtLib.Utils.UpdateHddtInfo(StartUp.Ma_ct, _stt_rec,  result.Data,1);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["tt_hddt_tcn"] = 1;
                                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"]=result.Data.MaQs_Hddt;
                                StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"] = result.Data.InvoiceNo;
                                StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri"] = result.Data.Serial;

                                PostAfterPublish(StartUp.Ma_ct, _stt_rec, 1);
                                ExMessageBox.Show(9000, StartUp.SysObj, "Phát hành hóa đơn điện tử thành công", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Có lỗi khi thực hiện phát hành hóa đơn điện tử\n" + result.Message);
                            }

                        }
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "3")
                        {
                            //Goi Dieu chinh tang hoa don
                            ////Update Ph81: 
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            // + Update tt_hddt_tcn của hóa đơn điều chỉnh/thay thế về trạng thái = 3 lọc theo stt_rec_tt 
                            string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = "Update Ph81 set tt_hddt_tcn = '1', user_id_ph = '" + StartUp.M_User_Id + "' where stt_rec = '" + _stt_rec + "'";
                            StartUp.SysObj.ExcuteNonQuery(cmd);
                        }
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "4")
                        {
                            //Goi Dieu chinh giảm hoa don
                            ////Update Ph81: 
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            // + Update tt_hddt_tcn của hóa đơn điều chỉnh/thay thế về trạng thái = 4 lọc theo stt_rec_tt 
                            string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = "Update Ph81 set tt_hddt_tcn = '1', user_id_ph = '" + StartUp.M_User_Id + "' where stt_rec = '" + _stt_rec + "'";
                            StartUp.SysObj.ExcuteNonQuery(cmd);
                        }
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["flag_btn"].ToString().Trim() == "5")
                        {
                            //goi Thay the hoa don
                            // + tt_hddt_tcn = "1", user_id_ph = m_user_id, 
                            // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)
                            // + Update tt_hddt_tcn của hóa đơn thay thế về trạng thái = 8 - Hủy hóa đơn tương ứng lọc theo stt_rec_tt
                            string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = "Update Ph81 set tt_hddt_tcn = '1', user_id_ph = '" + StartUp.M_User_Id + "' where stt_rec = '" + _stt_rec + "'";
                            StartUp.SysObj.ExcuteNonQuery(cmd);
                        }
                    }
            }
        }

        /// <summary>
        /// Cập nhật sổ sách
        /// </summary>
        /// <param name="ma_ct"></param>
        /// <param name="stt_rec"></param>
        /// <param name="state">1-Phát hành, </param>
        void PostAfterPublish(string ma_ct, string stt_rec, int state)
        {
            ActionAfterPublishInv action = HddtLib.Utils.GetActionAfterPublishInvoice(ma_ct, stt_rec, state);
            if (action == HddtLib.ActionAfterPublishInv.Post)
            {
                //Gọi hàm Post lại sổ sách
                string ma_qs_pt = HddtLib.Utils.GetCellStringValue(StartUpTrans.DsTrans.Tables[0].DefaultView[0].Row, "ma_qs_pt");
                string so_ct_pt = HddtLib.Utils.GetCellStringValue(StartUpTrans.DsTrans.Tables[0].DefaultView[0].Row, "so_ct_pt");
                string stt_rec_pt = HddtLib.Utils.GetCellStringValue(StartUpTrans.DsTrans.Tables[0].DefaultView[0].Row, "stt_rec_pt");
                string ma_ct_pt = HddtLib.Utils.GetCellStringValue(StartUpTrans.DsTrans.Tables[0].DefaultView[0].Row, "ma_ct_pt");

                if (ma_qs_pt.Length > 0 && stt_rec_pt.Length > 0 && so_ct_pt.Length > 0)
                    Post(1);
                else
                    Post(0);
            }
            else if (action == ActionAfterPublishInv.UpdateInfo)
            {
                //Gọi store để cập nhật lại số chứng từ, seri, mã quyển trên các table liên quan

                SqlCommand cmd = new SqlCommand(@"IF OBJECT_ID('TT_UpdateHddtInfo') IS NOT NULL
BEGIN
	EXEC TT_UpdateHddtInfo @ma_ct,@stt_rec,@state
END");
                cmd.Parameters.Add("@ma_ct", ma_ct);
                cmd.Parameters.Add("@stt_rec", stt_rec);
                cmd.Parameters.Add("@state", state);
                StartUp.SysObj.ExcuteNonQuery(cmd);
            }

        }
        //THAMTT
        void IsEnableButton1(string stt_hddt_tcn, string duyet_hddt)
        {

            if (stt_hddt_tcn == "0") //-moi tao, chua phat hanh
            {
                this.btnPH.IsEnabled = true;
                this.btnPHHDCho.IsEnabled = true;
                this.btnDCGiamHD.IsEnabled = false;
                this.btnDCTangHD.IsEnabled = false;
                this.btnHuyHD.IsEnabled = false;
                this.btnThayTheHD.IsEnabled = false;
                this.btnDCTTHD.IsEnabled = false;

                //(this.Toolbar.FindName("btnEdit") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Visible;
                //(this.Toolbar.FindName("btnCopy") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Visible;
                //(this.Toolbar.FindName("btnDelete") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Visible;
            }
            else
                if (stt_hddt_tcn == "8")// - Hoa don huy
                {
                    this.btnPH.IsEnabled = false;
                    this.btnPHHDCho.IsEnabled = false;
                    this.btnDCGiamHD.IsEnabled = false;
                    this.btnDCTangHD.IsEnabled = false;
                    this.btnHuyHD.IsEnabled = false;
                    this.btnThayTheHD.IsEnabled = false;
                    this.btnDCTTHD.IsEnabled = false;

                    //(this.Toolbar.FindName("btnEdit") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Hidden;
                    //(this.Toolbar.FindName("btnCopy") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Hidden;
                    //(this.Toolbar.FindName("btnDelete") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Hidden;
                }
                else
                {
                    this.btnPH.IsEnabled = false;
                    this.btnPHHDCho.IsEnabled = false;
                    this.btnDCGiamHD.IsEnabled = true;
                    this.btnDCTangHD.IsEnabled = true;
                    this.btnHuyHD.IsEnabled = true;
                    this.btnThayTheHD.IsEnabled = true;
                    this.btnDCTTHD.IsEnabled = true;
                    //this.Toolbar.Visibility = Visibility.Hidden;
                    //(this.Toolbar.FindName("btnEdit") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Hidden;
                    //(this.Toolbar.FindName("btnCopy") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Hidden;
                    // (this.Toolbar.FindName("btnDelete") as SmVoucherLib.ToolBarButton).Visibility = Visibility.Hidden;
                }
            //ENDTHAMTT
        }

        //THAMTT
        private string GetDMQS_HD(SysLib.SysObject SysObj, string ma_ct, DateTime ngay_ct, int user_id, string loai_qs, string _ma_qs)
        {
            SqlCommand sqlCommand = new SqlCommand("SELECT [status] FROM dmqs WHERE ma_qs = @ma_qs AND ma_qs IN(SELECT ma_qs FROM dmuserqs WHERE [user_id] = @user_id) AND ngay_qs1 <= @ngay_ct and loai_qs = @loai_qs");
            sqlCommand.Parameters.Add("@ma_qs", SqlDbType.VarChar).Value = _ma_qs;
            sqlCommand.Parameters.Add("@ngay_ct", SqlDbType.DateTime).Value = ngay_ct;
            sqlCommand.Parameters.Add("@user_id", SqlDbType.Int).Value = user_id;
            sqlCommand.Parameters.Add("@loai_qs", SqlDbType.VarChar).Value = loai_qs;

            DataTable dataTable = SysObj.ExcuteReader(sqlCommand).Tables[0];
            string resultqs;
            if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["status"].ToString() == "1")
            {
                resultqs = _ma_qs;
            }
            else
            {
                resultqs = GetDMQS(SysObj, ma_ct, ngay_ct, user_id);
            }
            return resultqs;
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            HddtLib.SoftDreams.PublishHandler h = new HddtLib.SoftDreams.PublishHandler();
            h.ShowConfig();
        }


    }
}
