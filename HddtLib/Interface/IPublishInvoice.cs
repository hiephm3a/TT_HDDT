using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HddtLib.Interface
{
    public interface IPublishInvoice
    {
        void ShowConfig();

        /// <summary>
        /// Phát hành hóa đơn 
        /// </summary>
        /// <param name="stt_rec">Số thứ tự bản ghi</param>
        /// <param name="ma_ct">Mã loại chứng từ</param>
        /// <param name="htOverrireDmct">Chứa thông thi thay thế thông tin có trong dmct. Ví dụ như view của ct,ph để lấy dữ liệu</param>
        FunctionResult<InvoiceInfo> PublishAndSignInvoice(string stt_rec, string ma_ct, Hashtable htOverrireDmct);
        /// <summary>
        /// Thay thế hóa đơn
        /// </summary>
        /// <param name="stt_rec">Số thứ tự bản ghi</param>
        /// <param name="ma_ct">Mã loại chứng từ</param>
        /// <param name="org_stt_rec">Số thứ tự bản ghi cần thay thế</param>
        /// <param name="ma_ct">Mã loại chứng từ của bản ghi cần thay thế</param>
        /// <param name="htOverrireDmct">Chứa thông thi thay thế thông tin có trong dmct của bản ghi phát hành. Ví dụ như view của ct,ph để lấy dữ liệu</param>
        /// /// <param name="htOverrireDmct">Chứa thông thi thay thế thông tin có trong dmct của bản ghi cần thay thế. Ví dụ như view của ct,ph để lấy dữ liệu</param>
        FunctionResult<InvoiceInfo> ReplaceAndSignInvoice(string stt_rec, string ma_ct, string org_stt_rec, string org_ma_ct, Hashtable htOverrireDmct, Hashtable htOrgOverrireDmct);
        /// <summary>
        /// Điểu chỉnh hóa đơn
        /// </summary>
        /// <param name="stt_rec"></param>
        /// <param name="ma_ct"></param>
        /// <param name="org_stt_rec"></param>
        /// <param name="org_ma_ct"></param>
        /// <param name="htOverrireDmct"></param>
        /// <param name="htOrgOverrireDmct"></param>
        /// <returns></returns>
        FunctionResult<InvoiceInfo> AdjustAndSignInvoice(string stt_rec, string ma_ct, string org_stt_rec, string org_ma_ct, Hashtable htOverrireDmct, Hashtable htOrgOverrireDmct);

       /// <summary>
       /// Xóa bỏ một hóa đơn đã phát hành (Đã có số hóa đơn và đc ký số)
       /// </summary>
       /// <param name="stt_rec"></param>
       /// <param name="ma_ct"></param>
       /// <param name="htOverrireDmct"></param>
       /// <returns></returns>
        FunctionResult<InvoiceInfo> CancelSignedInvoice(string stt_rec, string ma_ct,Hashtable htOverrireDmct);

        /// <summary>
       /// Xóa bỏ một hóa đơn chưa phát hành
       /// </summary>
       /// <param name="stt_rec"></param>
       /// <param name="ma_ct"></param>
       /// <param name="htOverrireDmct"></param>
       /// <returns></returns>
        FunctionResult<InvoiceInfo> CancelUnsignedInvoice(string stt_rec, string ma_ct, Hashtable htOverrireDmct);
        
        /// <summary>
        /// Tạo hóa đơn trên hệ thống Invoice mà chưa ký hay cấp số hóa đơn
        /// </summary>
        /// <param name="stt_rec">Số thứ tự bản ghi</param>
        /// <param name="ma_ct">Mã loại chứng từ</param>
        /// <param name="htOverrireDmct">Chứa thông thi thay thế thông tin có trong dmct. Ví dụ như view của ct,ph để lấy dữ liệu</param>
        FunctionResult<InvoiceInfo> ImportSignInvoice(string stt_rec, string ma_ct, Hashtable htOverrireDmct);
    }
}
