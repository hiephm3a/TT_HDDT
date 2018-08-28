using HddtLib.Interface;
using Sm.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HddtLib
{
    public class Utils
    {


        public static Interface.IPublishInvoice CreatePublishHandler()
        {
            return new SoftDreams.PublishHandler();
        }
        public static DataTable GetQuyenHddtInfo(string ma_qs, int loai)
        {
            string sqlCmd = "select * from dmqs where ma_qs='" + ma_qs + "' and (loai_qs=" + loai.ToString() + " OR -1=" + loai.ToString() + ")";
            DataSet ds = StartupBase.SysObj.ExcuteReader(new SqlCommand(sqlCmd));
            return ds.Tables[0];
        }

        public static DataRow SelectDmkhInfo(string ma_kh)
        {
            string sqlCmd = "select * from dmkh where ma_kh='" + ma_kh + "'";
            DataSet ds = StartupBase.SysObj.ExcuteReader(new SqlCommand(sqlCmd));
            return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
        }

        public static DataTable SelectDmnt()
        {
            SqlCommand cmd = new SqlCommand("Select * from dmnt");
            DataSet dsNt = StartupBase.SysObj.ExcuteReader(cmd);
            return dsNt.Tables[0];
        }
        public static string GetCellStringValue(DataRow row, string field)
        {
            if (row.IsNull(field)) return "";
            else
                return row[field].ToString().Trim();
        }
        public static void UpdateHddtInfo(string ma_ct, string stt_rec, InvoiceInfo inv, int type)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM dmct where ma_ct=@ma_ct";
            cmd.Parameters.Add("@ma_ct", ma_ct);

            DataSet dsDmct = StartupBase.SysObj.ExcuteReader(cmd);
            if (dsDmct.Tables[0].Rows.Count > 0)
            {
                string vph = dsDmct.Tables[0].Rows[0]["v_phdbf"].ToString();
                string vct = dsDmct.Tables[0].Rows[0]["v_ctdbf"].ToString();
                string mph = dsDmct.Tables[0].Rows[0]["m_phdbf"].ToString();
                string mct = dsDmct.Tables[0].Rows[0]["m_ctdbf"].ToString();

                // + so_ct = số hóa đơn đt, ngay_ct = ngày hddt, ngay_lct = ngày hddt, ma_qs = mã qs ánh xạ của quyển hóa đơn tạm (ma_qs_tam)    

                int so_ct_lenght = GetFieldLenght(mph, "so_ct");

                string sqlCmd = "Update " + mph + " set so_ct=dbo.PADL(@so_ct," + so_ct_lenght.ToString() + ",' '),ngay_ct=@ngay_ct,so_seri=@so_seri, ma_qs=@ma_qs where stt_rec=@stt_rec";

                SqlCommand cmd2 = new SqlCommand(sqlCmd);
                cmd2.Parameters.Add("@so_ct", inv.InvoiceNo);
                cmd2.Parameters.Add("@ngay_ct", inv.InvoiceDate);
                cmd2.Parameters.Add("@so_seri", inv.Serial);
                cmd2.Parameters.Add("@ma_qs", inv.MaQs_Hddt);
                cmd2.Parameters.Add("@stt_rec", stt_rec);
                StartupBase.SysObj.ExcuteNonQuery(cmd2);
            }
        }

        public static int GetFieldLenght(string TblName, string fieldName)
        {
            string sqlCmd = "Select CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS Where COLUMN_NAME=@ColName AND TABLE_NAME=@TblName";
            SqlCommand cmd = new SqlCommand(sqlCmd);
            cmd.Parameters.Add("@ColName", fieldName);
            cmd.Parameters.Add("@TblName", TblName);
            return Convert.ToInt32(StartupBase.SysObj.ExcuteScalar(cmd));
        }

        public static ActionAfterPublishInv GetActionAfterPublishInvoice(string ma_ct, string stt_rec, int state)
        {
            return ActionAfterPublishInv.Post;
        }
    }
    public enum ActionAfterPublishInv
    {
        None,
        Post,
        UpdateInfo
    }
}
