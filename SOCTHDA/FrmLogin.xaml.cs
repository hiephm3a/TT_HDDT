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
using System.Security.Cryptography;

namespace TT_SOCTHDA_HDDT
{
    /// <summary>
    /// Interaction logic for FrmLogin.xaml
    /// </summary>
    public partial class FrmLogin : Form
    {
        public bool IsLogined { get; set; }
        public FrmLogin()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            IsLogined = false;
        }

        #region Form_Loaded
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            txtpassword.Focus();
        } 
        #endregion

        #region Encrypt
        string Encrypt(string password)
        {
            //Cong nghe MD 5 Ham bam - sha256/128
            password = password.ToLower();
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(password);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes);
        }
        #endregion

        #region ConfirmGridView_OnOk
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (Encrypt(txtpassword.Password) != StartUp.SysObj.UserInfo.Rows[0]["password"].ToString())
            {
                ExMessageBox.Show( 385,StartUp.SysObj, "Mật khẩu không chính xác, vui lòng nhập lại mật khẩu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtpassword.SelectAll();
                txtpassword.Focus();
                return;
            }

            IsLogined = true;
            this.Close();
        } 
        #endregion
    }
}
