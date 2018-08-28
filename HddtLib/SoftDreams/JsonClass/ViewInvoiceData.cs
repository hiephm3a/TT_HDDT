using HddtLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HddtLib.SoftDreams.JsonClass
{
    public class ViewInvoiceData
    {
        public string Html { get; set; }
        public string InvoiceStatus { get; set; }
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public string No { get; set; }

        public string Ikey { get; set; }

        public string ArisingDate { get; set; }
        public string IssueDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Buyer { get; set; }

        public decimal Amount { get; set; }

        public InvoiceState InvState{
            get
            {
                try
                {
                    return (InvoiceState)int.Parse(InvoiceStatus);
                }
                catch { }
                return InvoiceState.NaN;
            }
        }
        
    }
}
