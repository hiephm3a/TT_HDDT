using EasyInvoice.Client.Api;
using HddtLib.Interface;
using HddtLib.SoftDreams.JsonClass;
using Sm.Windows.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace HddtLib.SoftDreams
{
    public class PublishHandler : IPublishInvoice
    {

        public static string ComputerAddress, ComputerName;
        static Dictionary<InvoiceState, string> InvStateDescription = new Dictionary<InvoiceState, string>();

        static PublishHandler()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                StringBuilder sb = new StringBuilder();
                foreach (var item in host.AddressList)
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        sb.AppendFormat("{0};", item.ToString());
                    }


                }
                ComputerAddress = sb.ToString();
            }

            ComputerName = Environment.MachineName;




            InvStateDescription.Add(InvoiceState.NaN, "Không xác định");
            InvStateDescription.Add(InvoiceState.NotExist, "Hoá đơn không tồn tại trong hệ thống");
            InvStateDescription.Add(InvoiceState.Created, "0: Hoá đơn mới tạo lập");
            InvStateDescription.Add(InvoiceState.Published, " 1: Hoá đơn có chữ ký số");
            InvStateDescription.Add(InvoiceState.Reported, "2: Hoá đơn đã khai báo thuế");
            InvStateDescription.Add(InvoiceState.Replaced, "3: Hoá đơn bị thay thế");
            InvStateDescription.Add(InvoiceState.Adjusted, "4: Hoá đơn bị điều chỉnh");
            InvStateDescription.Add(InvoiceState.Cancel, "5: Hoá đơn bị huỷ");


        }
        public PublishHandler()
        {
            InitSchema();
        }

        private void InitSchema()
        {
            //Bảng log
            SqlCommand cmd = new SqlCommand(@"IF OBJECT_ID('hddt_softdream_log') is null
        BEGIN
	        CREATE TABLE hddt_softdream_log(log_id uniqueidentifier,stt_rec varchar(32),ma_ct char(3),log_content nvarchar(max),request nvarchar(max),respone nvarchar(max),status_code nvarchar(32),end_point nvarchar(256),publish_type int,machine_ip nvarchar(128),machine_name nvarchar(64),date0 datetime, duration int ,duration_publish int);
        END
            if not exists(Select 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='duration_publish' AND TABLE_NAME='hddt_softdream_log')
	            ALTER TABLE hddt_softdream_log add duration_publish int
             if not exists(Select 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='user_id' AND TABLE_NAME='hddt_softdream_log')
	            ALTER TABLE hddt_softdream_log add user_id int
        IF OBJECT_ID('hddt_softdream_map') is null
        BEGIN
	         CREATE TABLE hddt_softdream_map(stt_rec varchar(32),ma_ct char(3),Inv_No nvarchar(12),Pattern nvarchar(32),Serial nvarchar(32),Inv_Date smalldatetime,Inv_Status int,date0 smalldatetime);
        END
"
                );
            StartupBase.SysObj.ExcuteNonQuery(cmd);
            //
        }
        public void ShowConfig()
        {
            FrmConfig frm = new FrmConfig();
            frm.ShowDialog();
        }
        public FunctionResult<InvoiceInfo> PublishAndSignInvoice(string stt_rec, string ma_ct, System.Collections.Hashtable htOverrireDmct)
        {
            FunctionResult<InvoiceInfo> result = new FunctionResult<InvoiceInfo>();
            result.Status = ResultCode.Error;
            result.Data = new InvoiceInfo();
            Guid logId = Guid.Empty;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();


            string log_content = "";

            try
            {
                sw.Start();
                logId = CreateLog(ma_ct, stt_rec, (int)PublishType.Publish);



                SqlCommand cmd = new SqlCommand("Select * from dmct where ma_ct='" + ma_ct + "'");
                DataSet dsDmct = StartupBase.SysObj.ExcuteReader(cmd);
                if (dsDmct.Tables[0].Rows.Count > 0)
                {
                    string ph = dsDmct.Tables[0].Rows[0]["v_phdbf"].ToString();
                    string ct = dsDmct.Tables[0].Rows[0]["v_ctdbf"].ToString();
                    string mph = dsDmct.Tables[0].Rows[0]["m_phdbf"].ToString();
                    string mct = dsDmct.Tables[0].Rows[0]["m_ctdbf"].ToString();
                    cmd = new SqlCommand("Select * FROM " + ph + " Where stt_rec=@stt_rec");
                    cmd.Parameters.Add(new SqlParameter("@stt_rec", stt_rec));
                    DataSet dsPH = StartupBase.SysObj.ExcuteReader(cmd);

                    cmd = new SqlCommand("Select * FROM " + ct + " Where stt_rec=@stt_rec");
                    cmd.Parameters.Add(new SqlParameter("@stt_rec", stt_rec));
                    DataSet dsCt = StartupBase.SysObj.ExcuteReader(cmd);

                    string ma_qs0 = dsPH.Tables[0].Rows[0]["ma_qs"].ToString();

                    DataTable tblDmqs = Utils.GetQuyenHddtInfo(ma_qs0, 0);
                    if (tblDmqs.Rows.Count == 0)
                        throw new Exception("Không tìm thấy quyển khai báo sử dụng hóa đơn điện tử cho quyển chứng từ " + ma_qs0);

                    if (tblDmqs.Rows[0]["ma_qs_ax"].ToString().Trim() == "")
                        throw new Exception(string.Format("Quyển chứng từ [{0}] chưa được gán mã quyển HĐĐT", tblDmqs.Rows[0]["ma_qs"]));

                    DataTable tblQHddt = Utils.GetQuyenHddtInfo(tblDmqs.Rows[0]["ma_qs_ax"].ToString(), 1);
                    if (tblQHddt.Rows.Count == 0)
                        throw new Exception("Không tìm thấy  khai báo quyển chứng từ có mã " + tblDmqs.Rows[0]["ma_qs_ax"].ToString());

                    DataRow dmKhRow = Utils.SelectDmkhInfo(dsPH.Tables[0].Rows[0]["ma_kh"].ToString());
                    if (dmKhRow["e_mail"].ToString().Trim() == "")
                        throw new Exception("Khách hàng mã " + dsPH.Tables[0].Rows[0]["ma_kh"].ToString() + " chưa được khai báo email để sử dụng hóa đơn điện tử");
                    string email_cus = dmKhRow["e_mail"].ToString().Trim();
                    string ma_qs = tblQHddt.Rows[0]["ma_qs"].ToString();
                    string pattern = tblQHddt.Rows[0]["kh_mau_hd"].ToString().Trim();
                    string serial = tblQHddt.Rows[0]["so_seri"].ToString().Trim();
                    string transform = tblQHddt.Rows[0]["transform"].ToString().Trim();
                    result.Data.MaQs_Luu = ma_qs0;
                    result.Data.MaQs_Hddt = ma_qs;
                    result.Data.Transform = transform;

                    //Kiểm tra xem hóa đơn đã được post trước đó chưa


                    ResponeBaseGenneric<CheckInvoiceStateData> rsCheck = checkInvoiceState(stt_rec);
                    InvoiceState state = rsCheck.Data.GetInvState(stt_rec);
                    if (state == InvoiceState.NotExist)
                    {
                        Hashtable htJson = new Hashtable();


                        string xmlContent = ConvertDataToXMLInvoice(dsPH.Tables[0], dsCt.Tables[0], ma_ct, (int)PublishType.Publish);

                        htJson.Add("XmlData", xmlContent);
                        htJson.Add("Pattern", pattern);
                        htJson.Add("Serial", serial);
                        htJson.Add("IkeyEmail", new KeyValuePair<string, string>(stt_rec, email_cus));
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(htJson);




                        string use_client_cert = FrmConfig.GetConfig(FrmConfig.KEY_USE_CLIENT_CERT);
                        string respone = "";

                        if (use_client_cert != "true")
                        {
                            sw2.Start();
                            log_content += Environment.NewLine + "Phát hành trực tiếp hóa đơn điện tử";
                            UpdateLogField(logId, new string[] { "request", "end_point" }, new object[] { json, "api/publish/importAndPublishInvoice" });
                            respone = Post2Server("api/publish/importAndPublishInvoice", SMethod.POST, json);
                            sw2.Stop();
                            UpdateLogField(logId, new string[] { "respone" }, new object[] { respone });
                            Hashtable htResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(respone);
                            string status = htResult["Status"].ToString();
                            string message = htResult["Message"].ToString();
                            if (status == "2")
                            {
                                result.Status = ResultCode.Success;
                                result.Message = message;
                                result.ResponeData = Encoding.UTF8.GetBytes(respone);
                                Hashtable htInvoiceInfo = htResult["Data"] as Hashtable;

                                result.Data.Pattern = htInvoiceInfo["Pattern"].ToString();
                                result.Data.Serial = htInvoiceInfo["Serial"].ToString();

                                Hashtable htKeyInvoiceNo = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htInvoiceInfo["KeyInvoiceNo"].ToString());
                                if (result.Data.Transform != "")
                                    result.Data.InvoiceNo = string.Format(result.Data.Transform, int.Parse(htKeyInvoiceNo[stt_rec].ToString()));
                                else
                                    result.Data.InvoiceNo = htKeyInvoiceNo[stt_rec].ToString();
                            }
                            else
                            {
                                result.Status = ResultCode.Error;
                                result.Message = message;
                            }
                        }
                        else
                        {
                            sw2.Start();
                            log_content += Environment.NewLine + "Phát hành 2 bước hóa đơn điện tử";
                            //Ký số 2 bước
                            //Bước 1.
                            //Tạo hóa đơn tạm api/publish/externalGetDigestForImportation
                            //Chọn CKS
                            X509Certificate2 signCert = null;
                            string certString = GetCertString(out signCert);
                            htJson.Add("CertString", certString);
                            json = Newtonsoft.Json.JsonConvert.SerializeObject(htJson);
                            log_content += Environment.NewLine + "1. Tạo hóa đơn tạm";
                            log_content += Environment.NewLine + "==>JSon:" + json;
                            respone = Post2Server("api/publish/externalGetDigestForImportation", SMethod.POST, json);
                            log_content += Environment.NewLine + "==>Respone:" + respone;

                            //Xử lý kết quả trả về

                            Hashtable htResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(respone);
                            if (htResult.ContainsKey("Status") && htResult["Status"].ToString() == "2")
                            {
                                //Tạo hóa đơn thành công
                                //Bước 2. Ký số hóa đơn và phát hành
                                Hashtable htData = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htResult["Data"].ToString());
                                //Lấy digest value 

                                Hashtable hDigestData = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htData["DigestData"].ToString());

                                Hashtable htData2Post2 = new Hashtable();
                                IDictionaryEnumerator ie = hDigestData.GetEnumerator();
                                while (ie.MoveNext())
                                {
                                    string sDigestData = hDigestData[ie.Key].ToString();
                                    string signHash = SignHash(signCert, sDigestData);
                                    htData2Post2.Add(ie.Key, signHash);
                                }


                                Hashtable htData2Post = new Hashtable();
                                htData2Post.Add("Pattern", htData["Pattern"]);
                                htData2Post.Add("Serial", htData["Serial"]);




                                htData2Post.Add("Signature", htData2Post2);
                                log_content += Environment.NewLine + "2. Phát hành hóa đơn tạm";
                                json = Newtonsoft.Json.JsonConvert.SerializeObject(htData2Post);
                                log_content += Environment.NewLine + "==>JSon:" + json;

                                respone = Post2Server("api/publish/externalWrapAndLaunchImportation", SMethod.POST, json);
                                log_content += Environment.NewLine + "==>Respone:" + respone;

                                htResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(respone);
                                if (htResult.ContainsKey("Status") && htResult["Status"].ToString() == "2")
                                {
                                    result.Status = ResultCode.Success;
                                    result.Message = "";
                                    result.ResponeData = Encoding.UTF8.GetBytes(respone);
                                    Hashtable htInvoiceInfo = htInvoiceInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htResult["Data"].ToString());
                                    result.Data.Pattern = htInvoiceInfo["Pattern"].ToString();
                                    result.Data.Serial = htInvoiceInfo["Serial"].ToString();
                                    result.Data.InvoiceDate = (DateTime)dsPH.Tables[0].Rows[0]["ngay_ct"];
                                    Hashtable htKeyInvoiceNo = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htInvoiceInfo["KeyInvoiceNo"].ToString());
                                    if (result.Data.Transform != "")
                                        result.Data.InvoiceNo = string.Format(result.Data.Transform, int.Parse(htKeyInvoiceNo[stt_rec].ToString()));
                                    else
                                        result.Data.InvoiceNo = htKeyInvoiceNo[stt_rec].ToString();
                                }
                                else
                                {
                                    //Lỗi
                                    result.Status = ResultCode.Error;
                                    result.Message = htResult["Message"].ToString();
                                }
                            }
                            else
                            {
                                result.Status = ResultCode.Error;
                                result.Message = htResult["Message"].ToString();
                            }

                            sw2.Stop();


                        }

                        UpdateLogField(logId, new string[] { "status_code" }, new object[] { result.Status.ToString() });


                    }
                    else
                    {
                        switch (state)
                        {
                            case InvoiceState.NaN:
                                throw new Exception("Lỗi khi tra cứu trạng thái hóa đơn");
                            case InvoiceState.Published:
                                if (MessageBox.Show("Hóa đơn hiện tại đã được phát hành. Bạn có muốn cập nhật lại trạng thái hóa đơn?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    result.Status = ResultCode.Success;
                                    ResponeBaseGenneric<ViewInvoiceData> rsViewInv = GetViewInvoice(stt_rec, rsCheck.Data.Pattern);
                                 //   result.Data = new InvoiceInfo();
                                    result.Data.InvState = rsViewInv.Data.InvState;
                                    if (result.Data.Transform != "")
                                        result.Data.InvoiceNo = string.Format(result.Data.Transform,int.Parse( rsViewInv.Data.No));
                                    else
                                        result.Data.InvoiceNo = rsViewInv.Data.No;


                                    result.Data.InvoiceDate = DateTime.ParseExact(rsViewInv.Data.ArisingDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    result.Data.Serial = rsViewInv.Data.Serial;
                                }
                                break;
                            default:
                                throw new Exception("Trạng thái hóa đơn hiện tại không cho phép cập nhật:" + InvStateDescription[state]);

                        }

                    }

                }
                else throw new Exception("Không tìm thấy mã chứng từ " + ma_ct);
            }
            catch (Exception ex)
            {
                result.Status = ResultCode.Exception;
                result.Message = ex.ToString();
                log_content += ex.ToString();
            }
            finally
            {
                sw.Stop();
                sw2.Stop();
                try
                {
                    UpdateLogField(logId, new string[] { "duration", "duration_publish", "log_content", "status_code" }, new object[] { sw.Elapsed.Seconds, sw2.Elapsed.Seconds, log_content, result.Status.ToString() });
                }
                catch
                {

                }
            }
            return result;
        }

        public ResponeBaseGenneric<ViewInvoiceData> GetViewInvoice(string stt_rec, string pattern)
        {
            InvoiceInfo ii = new InvoiceInfo();
            ii.InvState = InvoiceState.NaN;
            //api/publish/viewInvoice
            Hashtable ht = new Hashtable();
            ht.Add("Ikey", stt_rec);
            ht.Add("Pattern", pattern);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ht);
            string respone = Post2Server("api/publish/viewInvoice", SMethod.POST, json);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResponeBaseGenneric<ViewInvoiceData>>(respone);
        }

        private static string GetCertString(out X509Certificate2 signCert)
        {
            KeystoreService keystore = new KeystoreService();
            signCert = keystore.SelectCertificate();
            if (signCert == null)
                return null;
            return Convert.ToBase64String(signCert.RawData);
        }

        private static string SignHash(X509Certificate2 cert, string hash)
        {
            byte[] rgbHash = Convert.FromBase64String(hash);
            var signature = ((RSACryptoServiceProvider)cert.PrivateKey).SignHash(rgbHash, "SHA1");
            return Convert.ToBase64String(signature);
        }
        public void UpdateDmKh(string ma_kh)
        {
            DataRow khRow = Utils.SelectDmkhInfo(ma_kh);
            string xmlKh = ConvertDmKhToXml(khRow.Table);
            Hashtable ht = new Hashtable();
            ht.Add("XmlData", xmlKh);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ht);
            string jsonRespone = Post2Server("api/publish/updateCustomer", SMethod.POST, json);
            Hashtable htResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(jsonRespone);
            Console.WriteLine(jsonRespone);
        }
        public static string ConvertDmKhToXml(DataTable tblDmKh)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Customers");
            foreach (DataRow row in tblDmKh.Rows)
            {
                writer.WriteStartElement("Customer");
                writer.WriteElementString("Name", row["ten_kh"].ToString());
                writer.WriteElementString("Code", row["ma_kh"].ToString());
                writer.WriteElementString("AccountName", row["ma_kh"].ToString());
                writer.WriteElementString("TaxCode", row["ma_so_thue"].ToString());
                writer.WriteElementString("Address", row["dia_chi"].ToString());
                if (tblDmKh.Columns.Contains("ten_tk_nh"))
                    writer.WriteElementString("BankAccountName", row["ten_tk_nh"].ToString());
                else
                    writer.WriteElementString("BankAccountName", GetDmKhExtraInfo(row, "ten_tk_nh"));
                writer.WriteElementString("BankName", row["ten_nh"].ToString());
                writer.WriteElementString("BankNumber", row["tk_nh"].ToString());
                writer.WriteElementString("Email", row["e_mail"].ToString());
                writer.WriteElementString("Fax", row["fax"].ToString());
                writer.WriteElementString("Phone", row["dien_thoai"].ToString());

                if (tblDmKh.Columns.Contains("nguoi_lien_he"))
                    writer.WriteElementString("ContactPerson", row["nguoi_lien_he"].ToString());
                else
                    writer.WriteElementString("ContactPerson", GetDmKhExtraInfo(row, "nguoi_lien_he"));

                if (tblDmKh.Columns.Contains("nguoi_dai_dien"))
                    writer.WriteElementString("RepresentPerson", row["nguoi_dai_dien"].ToString());
                else
                    writer.WriteElementString("RepresentPerson", GetDmKhExtraInfo(row, "nguoi_dai_dien"));

                //  writer.WriteElementString("CusType", row["loai_kh"].ToString());

                if (row["ma_so_thue"].ToString().Trim() == "")
                    writer.WriteElementString("CusType", "0");
                else
                    writer.WriteElementString("CusType", "1");
                //
                //
                //Email
                //Fax
                //Phone
                //ContactPerson
                //RepresentPerson
                //CusType
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();
            return sb.ToString();
        }
        static string GetDmKhExtraInfo(DataRow khRow, string fieldName)
        {
            switch (fieldName)
            {
                case "nguoi_lien_he":
                case "nguoi_dai_dien":
                    return khRow["ong_ba"].ToString();
                case "ten_tk_nh":
                    return khRow["ten_kh"].ToString();


            }
            return "";
        }
        public static object _KeyToPost = new object();
        public string Post2Server(string apiName, string sMethod, string sData)
        {
            string apiUrl = FrmConfig.GetConfig(FrmConfig.KEY_URL);
            string _urlAPI = apiUrl + apiName;
            string _strReturn = "";
            string uid = FrmConfig.GetConfig(FrmConfig.KEY_LOGIN_NAME);
            string pwd = FrmConfig.GetConfig(FrmConfig.KEY_PASSWORD);

            try
            {
                ServicePointManager.ServerCertificateValidationCallback -= new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            }
            catch { }
            if (apiUrl.Contains("https:"))
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            //using (WebClient wc = new WebClient())
            //{

            //    wc.Headers[HttpRequestHeader.UserAgent] = "538c8b5f-2fb8-42be-a4ab-63fbee68e6af";
            //    wc.Headers[HttpRequestHeader.Accept] = "application/json"; //;
            //    wc.Headers[HttpRequestHeader.ContentType] = "application/json";//;charset=UTF-8

            //    string authen_token = GenerateToken(uid, pwd, sMethod);
            //    // wc.Headers[HttpRequestHeader.Authorization] = authen_token;
            //    wc.Headers.Add("Authentication", authen_token);
            //    wc.Encoding = UTF8Encoding.UTF8;
            //    try
            //    {
            //        lock (_KeyToPost)
            //        {
            //            if (sMethod == SMethod.GET)
            //                _strReturn = System.Text.Encoding.UTF8.GetString(wc.DownloadData(_urlAPI + "?" + sData));
            //            else
            //                _strReturn = wc.UploadString(_urlAPI, sMethod, sData);
            //        }
            //    }
            //    catch(WebException we)
            //    {
            //        if (we.Response is HttpWebResponse)
            //        {
            //            HttpWebResponse r = we.Response as HttpWebResponse;
            //            using (StreamReader sr = new StreamReader(r.GetResponseStream()))
            //            {
            //                _strReturn = sr.ReadToEnd();
            //            }
            //        }
            //    }
            //    catch(Exception ex)
            //    {
            //        throw;
            //    }

            //}
            //string authen_token = EasyRequest.GenAuthentication(uid, pwd, sMethod);// GenerateToken(uid, pwd, sMethod);
            ////HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlAPI);
            ////request.UserAgent = "EasyInvoice-Client/1.0.0.0";
            ////request.Accept = "application/json, text/json";
            ////request.ContentType = "application/json";
            ////request.Method = sMethod;
            ////request.Headers.Add("Authentication", authen_token);
            ////request.ServicePoint.Expect100Continue = false;
            ////request.KeepAlive = true;
            ////request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;

            ////using (Stream sw = request.GetRequestStream())
            ////{
            ////    byte[] d=Encoding.UTF8.GetBytes(sData);
            ////    sw.Write(d,0,d.Length);
            ////    sw.Flush();
            ////}
            ////HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            ////using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            ////{
            ////    _strReturn = sr.ReadToEnd();
            ////}

            EasyClient CLIENT = new EasyClient(apiUrl, uid, pwd);
            EasyResponse respone = CLIENT.PostJsonObject(apiName, sData);
            _strReturn = respone.Content;


            return _strReturn;
        }
        private static string GenerateToken(string id, string password, string httpMethod)
        {
            // UNIX time     
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string timestamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            string nonce = Guid.NewGuid().ToString("N").ToLower();

            // Tạo dữ liệu mã hóa
            string signatureRawData = string.Format("{0}{1}{2}", httpMethod.ToUpper(), timestamp, nonce);

            MD5 md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(signatureRawData));
            var signature = Convert.ToBase64String(hash);
            // Tạo dữ liệu Authentication
            return string.Format("{0}:{1}:{2}:{3}:{4}", signature, nonce, timestamp, id, password);


        }


        public class SMethod
        {
            public static string GET = "GET";
            public static string POST = "POST";
        }

        private bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        enum PublishType
        {
            Publish = 1,
            Update = 2,
            Delete = 7,
            Replace = 3

        }

        protected virtual void CreateCustomInfoElement(XmlWriter writer, DataRow msRow, DataRow khRow)
        {
            writer.WriteElementString("CusCode", msRow["ma_kh"].ToString().Trim());
            writer.WriteElementString("CusName", msRow["ten_kh"].ToString().Trim());
            writer.WriteElementString("Buyer", msRow["ong_ba"].ToString().Trim());
            writer.WriteElementString("CusAddress", msRow["dia_chi"].ToString().Trim());
            writer.WriteElementString("CusPhone", "");
            writer.WriteElementString("CusTaxCode", msRow["ma_so_thue"].ToString().Trim());
            writer.WriteElementString("CusBankName", khRow["ten_nh"].ToString().Trim());
            writer.WriteElementString("CusBankNo", khRow["tk_nh"].ToString().Trim());
        }

        protected virtual void CreateProductInfoElement(XmlWriter writer, DataRow msRow, DataRow proRow)
        {
            writer.WriteStartElement("Product");
            {

                writer.WriteElementString("Code", proRow["ma_vt"].ToString().Trim());
                writer.WriteElementString("ProdName", proRow["ten_vt"].ToString().Trim());
                writer.WriteElementString("ProdUnit", proRow["dvt1"].ToString().Trim());
                writer.WriteElementString("ProdQuantity", ((decimal)proRow["so_luong"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("ProdPrice", ((decimal)proRow["gia2"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("Total", ((decimal)proRow["tien2"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("VATRate", ((decimal)msRow["thue_suat"]).ToString("#", System.Globalization.CultureInfo.InvariantCulture));
                //Tổng tiền (Nếu hóa đơn thuộc mẫu chung thuế suất, Amount cần được gán giá trị của Total)
                writer.WriteElementString("Amount", ((decimal)proRow["tien2"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }
        protected virtual void CreateInvInfoElement(XmlWriter writer, DataRow msRow, DataRow[] itemRows)
        {
            writer.WriteStartElement("Inv");
            writer.WriteElementString("key", msRow["stt_rec"].ToString());
            writer.WriteStartElement("Invoice");
            {
                writer.WriteElementString("InvNo", "0");//Số hóa đơn sử dụng cách dải
                DataRow khRow = Utils.SelectDmkhInfo(msRow["ma_kh"].ToString());
                if (khRow == null)
                    throw new Exception("Không tồn tại  khách hàng có mã " + msRow["ma_kh"].ToString());

                CreateCustomInfoElement(writer, msRow, khRow);

                writer.WriteElementString("CurrencyUnit", msRow["ma_nt"].ToString().Trim());
                writer.WriteElementString("ExchangeRate", ((decimal)msRow["ty_gia"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("Extra", "");

                writer.WriteElementString("PaymentMethod", "TM/CK");

                writer.WriteElementString("ArisingDate", ((DateTime)msRow["ngay_ct"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));

                writer.WriteStartElement("Products");
                {

                    foreach (DataRow proRow in itemRows)
                    {
                        CreateProductInfoElement(writer, msRow, proRow);
                    }
                }
                writer.WriteEndElement();
                writer.WriteElementString("DiscountAmount", "0");
                writer.WriteElementString("Total", ((decimal)msRow["t_tien2"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("VATRate", ((decimal)msRow["thue_suat"]).ToString("#", System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("VATAmount", ((decimal)msRow["t_thue"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteElementString("Amount", ((decimal)msRow["t_tt"]).ToString(System.Globalization.CultureInfo.InvariantCulture));
                string soChu = SmLib.DocSo.Read("V", Utils.SelectDmnt().Rows, msRow["ma_nt"].ToString(), Convert.ToDouble((decimal)msRow["t_tt"]), true);
                writer.WriteElementString("AmountInWords", soChu.Trim());

            }
            writer.WriteEndElement();//End Invoice
            writer.WriteEndElement();//End Inv
            writer.Flush();
        }
        string ConvertDataToXMLInvoice(DataTable tblMaster, DataTable tblCt, string ma_ct, int publishType)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Invoices");
            foreach (DataRow msRow in tblMaster.Rows)
            {
                DataRow[] itemRows = tblCt.Select("stt_rec='" + msRow["stt_rec"].ToString() + "'");
                CreateInvInfoElement(writer, msRow, itemRows);
            }
            writer.WriteEndElement();
            writer.Flush();
            return sb.ToString();
        }

        public FunctionResult<InvoiceInfo> ReplaceAndSignInvoice(string stt_rec, string ma_ct, string org_stt_rec, string org_ma_ct, System.Collections.Hashtable htOverrireDmct, System.Collections.Hashtable htOrgOverrireDmct)
        {
            throw new NotImplementedException();
        }

        public FunctionResult<InvoiceInfo> AdjustAndSignInvoice(string stt_rec, string ma_ct, string org_stt_rec, string org_ma_ct, System.Collections.Hashtable htOverrireDmct, System.Collections.Hashtable htOrgOverrireDmct)
        {
            throw new NotImplementedException();
        }


        public ResponeBaseGenneric<CheckInvoiceStateData> checkInvoiceState(string stt_rec)
        {
            ResponeBaseGenneric<CheckInvoiceStateData> result = new ResponeBaseGenneric<CheckInvoiceStateData>();
            string request = "{\"Ikeys\":[\"" + stt_rec + "\"] }";
            string respone = Post2Server("api/publish/checkInvoiceState", SMethod.POST, request);
            result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponeBaseGenneric<CheckInvoiceStateData>>(respone);
            return result;
        }
        public FunctionResult<InvoiceInfo> CancelSignedInvoice(string stt_rec, string ma_ct, System.Collections.Hashtable htOverrireDmct)
        {
            FunctionResult<InvoiceInfo> result = new FunctionResult<InvoiceInfo>();
            result.Status = ResultCode.Error;
            Guid logId = Guid.Empty;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            try
            {
                sw.Start();
                logId = CreateLog(ma_ct, stt_rec, (int)PublishType.Delete);
                SqlCommand cmd = new SqlCommand("Select * from dmct where ma_ct='" + ma_ct + "'");
                DataSet dsDmct = StartupBase.SysObj.ExcuteReader(cmd);
                if (dsDmct.Tables[0].Rows.Count > 0)
                {
                    string ph = dsDmct.Tables[0].Rows[0]["v_phdbf"].ToString();
                    string ct = dsDmct.Tables[0].Rows[0]["v_ctdbf"].ToString();
                    string mph = dsDmct.Tables[0].Rows[0]["m_phdbf"].ToString();
                    string mct = dsDmct.Tables[0].Rows[0]["m_ctdbf"].ToString();
                    cmd = new SqlCommand("Select * FROM " + ph + " Where stt_rec=@stt_rec");
                    cmd.Parameters.Add(new SqlParameter("@stt_rec", stt_rec));
                    DataSet dsPH = StartupBase.SysObj.ExcuteReader(cmd);
                    if (dsPH.Tables[0].Rows.Count > 0)
                    {
                        string ma_qs = dsPH.Tables[0].Rows[0]["ma_qs"].ToString();
                        string seri = dsPH.Tables[0].Rows[0]["so_seri"].ToString();
                        string pattern = "";
                        DataTable tblQhddt = Utils.GetQuyenHddtInfo(ma_qs, -1);
                        if (tblQhddt.Rows.Count > 0)
                        {
                            if (seri != tblQhddt.Rows[0]["so_seri"])
                                seri = tblQhddt.Rows[0]["so_seri"].ToString();

                            pattern = tblQhddt.Rows[0]["kh_mau_hd"].ToString();
                        }

                        Hashtable htData = new Hashtable();
                        htData.Add("Ikey", stt_rec);
                        htData.Add("Pattern", pattern);


                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(htData);
                        UpdateLogField(logId, new string[] { "request", "end_point" }, new object[] { json, "api/business/cancelInvoice/" });
                        sw2.Start();
                        string respone = Post2Server("api/business/cancelInvoice/", SMethod.POST, json);
                        sw2.Stop();
                        UpdateLogField(logId, new string[] { "respone" }, new object[] { respone });
                        Hashtable htResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(respone);
                        if (htResult.ContainsKey("Status") && htResult["Status"].ToString() == "2")
                        {
                            //
                            result.Status = ResultCode.Success;
                            result.Data = new InvoiceInfo();
                            result.Data.InvoiceNo = dsPH.Tables[0].Rows[0]["so_ct"].ToString();
                            result.Data.InvoiceDate = (DateTime)dsPH.Tables[0].Rows[0]["ngay_ct"];
                            result.Data.Serial = dsPH.Tables[0].Rows[0]["so_seri"].ToString();
                            result.Data.Pattern = pattern;

                        }
                        else
                        {
                            result.Status = ResultCode.Error;
                            result.Message = htResult["Message"].ToString();
                            if (htResult.ContainsKey("Data"))
                            {
                                htData = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htResult["Data"].ToString());
                                if (htData.ContainsKey("KeyInvoiceMsg"))
                                {
                                    Hashtable htKeyStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(htData["KeyInvoiceMsg"].ToString());
                                    if (htKeyStatus.ContainsKey(stt_rec))
                                        result.Message += "\n" + stt_rec + ":" + htKeyStatus[stt_rec].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = ResultCode.Exception;
                result.Message = ex.ToString();
            }
            finally
            {
                sw.Stop();
                try
                {
                    UpdateLogField(logId, new string[] { "duration", "duration_publish", "status_code", "log_content" }, new object[] { sw.Elapsed.Seconds, sw2.Elapsed.Seconds, result.Status, result.Message });
                }
                catch
                {

                }
            }

            return result;
        }

        public FunctionResult<InvoiceInfo> CancelUnsignedInvoice(string stt_rec, string ma_ct, System.Collections.Hashtable htOverrireDmct)
        {
            throw new NotImplementedException();
        }

        public FunctionResult<InvoiceInfo> ImportSignInvoice(string stt_rec, string ma_ct, System.Collections.Hashtable htOverrireDmct)
        {
            throw new NotImplementedException();
        }


        #region LogUtil
        static Guid CreateLog(string ma_ct, string stt_rec, int publish_type)
        {
            string sqlCmd = "INSERT INTO hddt_softdream_log(log_id,stt_rec,ma_ct,publish_type,date0,machine_name,machine_ip,user_id) VALUES(@id,@stt_rec,@ma_ct,@publishType,getdate(),@machine_name,@machine_ip,@user_id)";
            SqlCommand cmd = new SqlCommand(sqlCmd);
            Guid id = Guid.NewGuid();
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.Parameters.Add(new SqlParameter("@stt_rec", stt_rec));
            cmd.Parameters.Add(new SqlParameter("@ma_ct", ma_ct));
            cmd.Parameters.Add(new SqlParameter("@publishType", publish_type));
            cmd.Parameters.Add(new SqlParameter("@machine_name", ComputerName));
            cmd.Parameters.Add(new SqlParameter("@machine_ip", ComputerAddress));
            cmd.Parameters.Add(new SqlParameter("@user_id", StartupBase.SysObj.UserInfo.Rows[0]["user_id"]));
            Sm.Windows.Controls.StartupBase.SysObj.ExcuteNonQuery(cmd);
            return id;
        }
        static void UpdateLogField(Guid logid, string[] fields, object[] values)
        {
            string updateField = "";
            for (int i = 0; i < fields.Length; i++)
            {
                updateField += (updateField.Length > 0 ? "," : "") + fields[i] + " = @p_" + fields[i];
            }
            string sqlCmd = "Update hddt_softdream_log set " + updateField + " WHERE log_id=@log_id";
            SqlCommand cmd = new SqlCommand(sqlCmd);
            for (int i = 0; i < fields.Length; i++)
            {
                cmd.Parameters.Add(new SqlParameter("@p_" + fields[i], (values[i] == null ? DBNull.Value : values[i])));
            }

            cmd.Parameters.Add(new SqlParameter("@log_id", logid));
            Sm.Windows.Controls.StartupBase.SysObj.ExcuteNonQuery(cmd);

        }
        #endregion
    }


}
