using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sm.Windows.Controls;
using System.Windows;
using SmErrorLib;
using System.Diagnostics;
using SmVoucherLib;
using SmDefine;

namespace TT_SOCTHDA_HDDT
{
    public class StartUp:StartUpTrans
    {
       
        static public SqlCommand TransFilterCmd;
       
        static public string M_Tilte = string.Empty;
        static public string M_ma_nt = string.Empty;

        static public int M_KM_CK;
        static public int M_AR_CK;
        static public int M_THUE_KM_CK;
      //  static public string M_ngay_lct;
        static public string M_CHK_TON_VT;

        static public string M_MA_THUE;
        static public string M_PHONE;
        static public int M_BP_BH = 0;
        static public int M_UPDATE_GIA2 = 0;
        public static string M_CHK_DATE_YN = "";
        public static DateTime? M_NGAY_BAT_DAU;
        public static DateTime? M_NGAY_KET_THUC;

        FrmTT_SOCTHDA_HDDT _Form;

        static public string stringBrowse1 = "";
        static public string stringBrowse2 = "";
        static public string stringBrowse3 = "";
        static public string stringBrowse4 = "";
        static public DateTime M_ngay_ct0;
        static public bool IsQLHD;
        static public DateTime ngay_gia_px;
        static public DataTable dtRegInfo;

        #region Run
        public override void Run()
        {
           Namespace = "TT_SOCTHDA_HDDT";
            Ma_ct = "HDA";
            filterId = "SOCTHDA";
            //Khoi tao cac tham so can thiet
            M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString().Trim().ToUpper();
            Ws_Id = SysObj.GetOption("M_WS_ID").ToString();
            M_LAN = SysObj.GetOption("M_LAN").ToString();
            //M_ROUND = Convert.ToInt16(SysObj.GetSysVar("M_ROUND"));
            //M_ROUND_NT = Convert.ToInt16(SysObj.GetSysVar("M_ROUND_NT"));
            //M_ROUND_GIA = Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND_GIA"));
            //M_ROUND_GIA_NT = Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND_GIA_NT"));

            M_User_Id = Convert.ToInt16(SysObj.UserInfo.Rows[0]["user_id"].ToString());
            M_MST_CHECK = SysObj.GetOption("M_MST_CHECK").ToString().Trim();

            M_CHK_TON_VT = SysObj.GetOption("M_CHK_TON_VT").ToString();

            M_MA_THUE = SysObj.GetOption("M_MA_THUE").ToString();
            M_PHONE = SysObj.GetOption("M_PHONE").ToString();
           
            M_THUE_KM_CK = Convert.ToInt16(SysObj.GetOption("M_THUE_KM_CK"));
            M_UPDATE_GIA2 = Convert.ToInt16(SysObj.GetOption("M_UPDATE_GIA2"));

            M_ngay_ct0 = Convert.ToDateTime(SysObj.GetSysVar("M_NGAY_KY1"));
            object ngay = SysObj.GetOption("M_NGAY_GIA_PX");
            ngay_gia_px = DateTime.Parse(ngay.ToString());

            CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);

            DmctInfo = SmDataLib.DataLoader.GetSqlFieldValue(SysObj, "dmct", "ma_ct", Ma_ct);

            
            if (CommandInfo == null)
            {
                ExMessageBox.Show( 835,SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }
            if (DmctInfo == null)
            {
                ExMessageBox.Show( 840,SysObj, "Chưa khai báo chứng từ hoặc chứng từ ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }
            dtRegInfo = StartUp.SysObj.GetRegInfo();

            M_ngay_lct = DmctInfo["m_ngay_lct"].ToString();
            M_ong_ba = DmctInfo["m_ong_ba"].ToString();

            int.TryParse(DmctInfo["m_sl_ct0"].ToString(), out M_sl_ct0);
            int.TryParse(DmctInfo["m_bp_bh"].ToString(), out M_BP_BH);
            
            string[] strBrowses = null;
            if (M_LAN.Trim().Equals("V"))
            {
                if (CommandInfo["Vbrowse2"].ToString().Trim() != "")
                    strBrowses = CommandInfo["Vbrowse2"].ToString().Split('|');
            }
            else
            {
                if (CommandInfo["Ebrowse2"].ToString().Trim() != "")
                    strBrowses = CommandInfo["Ebrowse2"].ToString().Split('|');
            }
            if (strBrowses != null)
            {
                stringBrowse1 = strBrowses[0];
                stringBrowse2 = strBrowses[1];
                stringBrowse3 = strBrowses[2];
                stringBrowse4 = strBrowses[3];
            }

            filterView = string.Format("{0};{1};{2}", StartUp.DmctInfo["v_phdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctgtdbf"].ToString().Trim());

            _Form = new FrmTT_SOCTHDA_HDDT();
       
            M_Tilte = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());

            string conditionPH = " AND 1=1";
            if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, Menu_Id))
                conditionPH = " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

            M_CHK_DATE_YN = SysObj.GetOption("M_CHK_DATE_YN").ToString().Trim();
            if (Editing_Stt_Rec.Equals(string.Empty) && M_CHK_DATE_YN == "1")
            {
                SmVoucherLib.SelectTime dlgSelTime = new SmVoucherLib.SelectTime();
                dlgSelTime.LanguageID = "TT_SOCTHDA_HDDTSelTime";

                SmLib.SysFunc.LoadIcon(dlgSelTime);
                dlgSelTime.ShowDialog();
                if (dlgSelTime.IsOK)
                {
                    M_NGAY_BAT_DAU = (DateTime)dlgSelTime.M_NGAY_CT1;
                    M_NGAY_KET_THUC = (DateTime)dlgSelTime.M_NGAY_CT2;
                    conditionPH += string.Format(" AND ngay_ct BETWEEN '{0:yyyyMMdd}' AND '{1:yyyyMMdd}'", M_NGAY_BAT_DAU, M_NGAY_KET_THUC);
                }
            }

            TransFilterCmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter, @Sl_ct");
            TransFilterCmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = Ma_ct;
            TransFilterCmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = (Editing_Stt_Rec.Equals(string.Empty) ? "1=1" + " AND ma_dvcs = '" + SysObj.M_ma_dvcs.Trim() + "'" : "stt_rec = '" + Editing_Stt_Rec + "'") + conditionPH;
            TransFilterCmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            TransFilterCmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            TransFilterCmd.Parameters.Add("@Sl_ct", SqlDbType.Int).Value = M_sl_ct0;
            DsTrans = SmVoucherLib.DataProvider.FillCommand(SysObj, TransFilterCmd);

            DsTrans.Tables[0].DefaultView.Sort = "ngay_ct asc, so_ct asc";
            DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";

            CreateColumnsKM();
    
            //Dòng để giữ focus khi xoá hoặc huỷ
            DataRow dr = DsTrans.Tables[0].NewRow();
            dr["stt_rec"] = string.Empty;
            dr["ma_nt"] = M_ma_nt0;
            DsTrans.Tables[0].Rows.InsertAt(dr, 0);
            tbStatus = SysObj.GetPostInfo(Ma_ct);
            //Kiểm tra cho in liên tục hay không
            SqlCommand CheckQLHDCmd = new SqlCommand("SELECT COUNT(*) FROM v_dmmauhd WHERE ma_ct_qs LIKE @ma_ct");
            CheckQLHDCmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = "%" + Ma_ct + "%";
            IsQLHD = Convert.ToDecimal(SysObj.ExcuteScalar(CheckQLHDCmd)) != 0;
            _Form.Title = M_Tilte;
            _Form.StartUpMain = this;


            //HddtLib.SoftDreams.PublishHandler h = new HddtLib.SoftDreams.PublishHandler();
            //h.ShowConfig();

            SmLib.SysFunc.LoadIcon(_Form);
            _Form.ShowDialog();
        }
        #endregion

        #region CreateColumnsKM
        void CreateColumnsKM()
        {
            DataColumn newcolumn = new DataColumn("t_sl_km", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);
            
            newcolumn = new DataColumn("t_tien_km_nt", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("t_tien_km", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("t_thue_km_nt", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("t_thue_km", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("tien_tc_nt", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("tien_tc", typeof(decimal));
            newcolumn.DefaultValue = 0;
            DsTrans.Tables[0].Columns.Add(newcolumn);
        }
        #endregion

        #region GetRates
        public static decimal GetRates(string _ma_nt, DateTime _ngay)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select [dbo].[GetRates](@ma_nt,@ngay_ct)");
                cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
                object o_ty_gia = SysObj.ExcuteScalar(cmd);
                decimal _ty_gia = Convert.ToDecimal(o_ty_gia.Equals(DBNull.Value) ? 0 : o_ty_gia);
                return _ty_gia;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return 0;
        }
        #endregion

        //#region UpdateRates
        //public static int UpdateRates(string _ma_nt, DateTime _ngay, decimal _ty_gia)
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand("exec [dbo].[SetRates] @ma_nt,@ngay_ct,@ty_gia, @user_id");
        //        cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
        //        cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
        //        cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = _ty_gia;
        //        cmd.Parameters.Add("@user_id", SqlDbType.Decimal).Value = M_User_Id;
        //        int _rno = StartUp.SysObj.ExcuteNonQuery(cmd);
        //        return _rno;
        //    }
        //    catch (Exception ex)
        //    {
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //    return 0;
        //}
        //#endregion

        #region DataFilter
        public static void DataFilter(string stt_rec)
        {
            string km_ck = "";
            //Refresh lai form
            if (M_KM_CK == 0)
                km_ck = " AND km_ck = '0'";
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'" + km_ck;
           
            StartUp.DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
           
        }
        #endregion

        #region GetLanguageString
        public static string GetLanguageString(string code, string language)
        {
            if (code == "M_MA_NT")
            {
                if (StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                    return "";
                else
                    return StartUp.DsTrans.Tables[0].Rows[FrmTT_SOCTHDA_HDDT.iRow]["ma_nt"].ToString();
            }
            if (code == "M_MA_NT0")
                return StartUp.M_ma_nt0;
            return code;
        }
        #endregion

        #region GetDmnt
        public static DataTable GetDmnt()
        {
            SqlCommand cmd = new SqlCommand("Select *,@ma_nt as ma_nt0, @Type as read_num_type from dmnt");
            cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = StartUp.M_ma_nt0;
            cmd.Parameters.Add("@Type", SqlDbType.Char, 1).Value = SysObj.GetOption("M_READ_NUM");


            DataTable tbNT = StartUp.SysObj.ExcuteReader(cmd).Tables[0].Copy();
            tbNT.TableName = "TableNTInfo";
            if (DsTrans.Tables.IndexOf("TableNTInfo") >= 0)
                DsTrans.Tables.Remove("TableNTInfo");
            return tbNT;
        }
        #endregion

        #region CheckData
        public static DataSet CheckData(int mode)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[SOCTHDA-CheckData] @status, @mode, @stt_rec");
            cmd.Parameters.Add("@status", SqlDbType.Char, 1).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
            cmd.Parameters.Add("@mode", SqlDbType.Int).Value = mode;
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            return StartUp.SysObj.ExcuteReader(cmd);
        }
        #endregion

        public static void DeleteVoucher(string _stt_rec, string ma_qs, ActionTask action, bool isNd51)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @ma_ct, @stt_rec");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
            StartUp.SysObj.ExcuteNonQuery(cmd);

            if (isNd51)
            {
                cmd = new SqlCommand("exec [dbo].[SOCTHDA-PostCTHHD] @stt_rec;");
                if (action == ActionTask.Delete)
                {
                    cmd.CommandText += " DECLARE @ngay_ct smalldatetime;";
                    cmd.CommandText += " DECLARE @so_ct numeric(16, 0), @so_ct1 numeric(16, 0);";
                    cmd.CommandText += " SELECT @so_ct = so_ct - 1, @so_ct1 = so_ct1 - 1 FROM dmqs WHERE ma_qs='" + ma_qs.Trim() + "';";
                    cmd.CommandText += " IF @so_ct = @so_ct1";
                    cmd.CommandText += "    SELECT @ngay_ct = ngay_qs1 FROM dmqs WHERE ma_qs='" + ma_qs.Trim() + "';";
                    cmd.CommandText += " ELSE";
                    cmd.CommandText += "    SELECT TOP 1 @ngay_ct = ngay_ct FROM cthhd WHERE ma_qs='" + ma_qs.Trim() + "' AND so_ct IS NOT NULL ORDER BY ngay_ct DESC;";
                    cmd.CommandText += " UPDATE dmqs SET so_ct = @so_ct, ngay_ct = ISNULL(@ngay_ct,ngay_qs1) WHERE ma_qs='" + ma_qs.Trim() + "';";
                }
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
                StartUp.SysObj.ExcuteNonQuery(cmd);
            }
        }

        public static void DeletePT(string _stt_rec, string ma_ct)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @ma_ct, @stt_rec");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = ma_ct;
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }

        #region IsTkMe
        static public bool IsTkMe(string tk)
        {
            bool result = false;
            try
            {
                SqlCommand cmdGet = new SqlCommand("exec CheckIsTkMe @tk");
                cmdGet.Parameters.Add("@tk", SqlDbType.Char).Value = tk;
                if ((int)StartUp.SysObj.ExcuteScalar(cmdGet) > 0)
                    result = true;
            }
            catch (SqlException ex)
            {
                ErrorLog.CatchMessage(ex);
            }
            return result;
        }
        #endregion

        #region GetPN
        public static DataTable GetPN(string ma_vt, string ma_kho, object ngay_ct)
        {
            DataTable tbNT = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("Exec GetPN @ma_vt,@ma_kho, @ngay_ct");
                cmd.Parameters.Add("@ma_vt", SqlDbType.VarChar).Value = ma_vt;
                cmd.Parameters.Add("@ma_kho", SqlDbType.VarChar).Value = ma_kho;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.DateTime).Value = ngay_ct;

                DataSet ds = StartUp.SysObj.ExcuteReader(cmd);
                if (ds != null || ds.Tables.Count > 0)
                {
                    tbNT = ds.Tables[0].Copy();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return tbNT;
        }
        #endregion

        #region UpdateSl_in
        public static void UpdateSl_in(string stt_rec)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[Update_sl_in] @stt_rec, @ma_ct, @user_id, @date, @time, @hostname");
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = stt_rec;
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
            cmd.Parameters.Add("@user_id", SqlDbType.Decimal).Value = M_User_Id;
            cmd.Parameters.Add("@date", SqlDbType.SmallDateTime).Value = DateTime.Now.Date.ToString("dd-MM-yyyy");
            cmd.Parameters.Add("@time", SqlDbType.Char, 8).Value = DateTime.Now.ToString("HH:mm:ss");
            cmd.Parameters.Add("@hostname", SqlDbType.NChar, 100).Value = System.Environment.MachineName;
            SysObj.ExcuteNonQuery(cmd);
        }
        #endregion

        #region GetHdm
        public static DataSet GetHdb(string filter)
        {
            SqlCommand cmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter, @sl_ct");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = "HDB";
            cmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = filter;
            cmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            cmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "";
            cmd.Parameters.Add("@sl_ct", SqlDbType.Int).Value = 0;

            DataSet ds = SmVoucherLib.DataProvider.FillCommand(SysObj, cmd);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["ma_nt"].ToString().Trim().ToUpper().Equals(StartUp.M_ma_nt0))
                {
                    DataRow[] ldr = ds.Tables[1].Select("stt_rec LIKE '" + ds.Tables[0].Rows[i]["stt_rec"].ToString() + "'");
                    foreach (DataRow dr in ldr)
                    {
                        dr["gia_nt2"] = 0;
                        dr["tien_nt2"] = 0;
                    }
                }
            }
            return ds;
        }
        #endregion

        #region UpdateGia
        //Giá được update trong store Post
        //public static void UpdateGia(string ma_ct,string stt_rec)
        //{
        //    SqlCommand cmd = new SqlCommand("exec [UpdateGia] @ma_ct, @stt_rec");
        //    cmd.Parameters.Add("@ma_ct", SqlDbType.VarChar).Value = ma_ct;
        //    cmd.Parameters.Add("@stt_rec", SqlDbType.Char).Value = stt_rec;

        //    SysObj.ExcuteNonQuery(cmd);
        //}
        #endregion
    }
}
