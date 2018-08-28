using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HddtLib.Interface
{
    public class InvoiceInfo
    {
        /// <summary>
        /// ID hóa đơn của hệ thống hddt
        /// </summary>
        public string InvoiceId { get; set; }
        /// <summary>
        /// Tên mẫu hóa đơn
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// Ký hiệu hóa đơn
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Ngày hóa đơn
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Địa chỉ tải hóa đơn
        /// </summary>
        public string DownloadUrl { get; set; }

        public byte[] InvContent { get; set; }

        public InvoiceState InvState { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của Url: xml, pdf,html...
        /// </summary>
        public string DataType { get; set; }

        public string MaQs_Hddt { get; set; }
        public string MaQs_Luu { get; set; }

        public string Transform { get; set; }

    }

    public enum InvoiceState
    {
        //        •	-1: Hoá đơn không tồn tại trong hệ thống
        //•	 0: Hoá đơn mới tạo lập
        //•	 1: Hoá đơn có chữ ký số
        //•	 2: Hoá đơn đã khai báo thuế
        //•	 3: Hoá đơn bị thay thế
        //•	 4: Hoá đơn bị điều chỉnh
        //•	 5: Hoá đơn bị huỷ

        NaN = -99,
        NotExist = -1,
        Created = 0,
        Published = 1,
        Reported = 2,
        Replaced = 3,
        Adjusted = 4,
        Cancel = 5
      
    }
}
