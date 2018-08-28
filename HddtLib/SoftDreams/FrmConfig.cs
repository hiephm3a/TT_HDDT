
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HddtLib.SoftDreams
{
    public partial class FrmConfig : Form
    {
        public const string KEY_URL = "URL_API";
        public const string KEY_LOGIN_NAME = "LOGIN_NAME";
        public const string KEY_PASSWORD = "PASSWORD";
        public const string KEY_USE_CLIENT_CERT = "USE_CLIENT_CERT";
        public FrmConfig()
        {
            InitializeComponent();
        }

        void InitSchema()
        {
            string sqlCmd = @"IF OBJECT_ID('hddd_softdream_config') is null
BEGIN
	CREATE TABLE hddd_softdream_config(config_key varchar(64),config_value nvarchar(256),option1 nvarchar(128),option2 nvarchar(128),option3 nvarchar(256))
END";
            SqlCommand cmd = new SqlCommand(sqlCmd);
            Sm.Windows.Controls.StartupBase.SysObj.ExcuteNonQuery(cmd);
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            try
            {
                InitSchema();
                txtUrl.Text = GetConfig(KEY_URL);
                txtLoginName.Text = GetConfig(KEY_LOGIN_NAME);
                txtPassword.Text = GetConfig(KEY_PASSWORD);

                chkUseClientCert.Checked = GetConfig(KEY_USE_CLIENT_CERT) == "true";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static string GetConfig(string key)
        {
            SqlCommand cmd = new SqlCommand("Select * from hddd_softdream_config Where config_key='" + key + "'");
            DataSet ds = Sm.Windows.Controls.StartupBase.SysObj.ExcuteReader(cmd);
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["config_value"].ToString();
            else
                return "";

        }

        public static void SaveConfig(string key, string val, string op1, string op2, string op3)
        {
            SqlCommand cmd = new SqlCommand(@"DELETE hddd_softdream_config Where config_key=@key;
                INSERT INTO hddd_softdream_config(config_key,config_value,option1,option2,option3) VALUES(@key,@val,@op1,@op2,@op3);");
            cmd.Parameters.Add(new SqlParameter("@key", key));
            cmd.Parameters.Add(new SqlParameter("@val", val));
            cmd.Parameters.Add(new SqlParameter("@op1", op1));
            cmd.Parameters.Add(new SqlParameter("@op2", op2));
            cmd.Parameters.Add(new SqlParameter("@op3", op3));
            Sm.Windows.Controls.StartupBase.SysObj.ExcuteNonQuery(cmd);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtUrl.Text.Trim()=="")
            {
                MessageBox.Show("Chưa nhập URL");
                return;
            }

            if (txtLoginName.Text.Trim() == "")
            {
                MessageBox.Show("Chưa nhập tên đăng nhập");
                return;
            }
            if (txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Chưa nhập mật khẩu");
                return;
            }
            SaveConfig(KEY_URL, txtUrl.Text,"","","");
            SaveConfig(KEY_LOGIN_NAME, txtLoginName.Text, "", "", "");
            SaveConfig(KEY_PASSWORD, txtPassword.Text, "", "", "");
            SaveConfig(KEY_USE_CLIENT_CERT, chkUseClientCert.Checked.ToString().ToLower(), "", "", "");
            MessageBox.Show("Cập nhật thành công");
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
