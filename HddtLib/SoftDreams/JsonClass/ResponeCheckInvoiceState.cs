using HddtLib.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HddtLib.SoftDreams.JsonClass
{
    public class CheckInvoiceStateData
    {
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public Hashtable KeyInvoiceMsg { get; set; }

        public Hashtable KeyStatus { get; set; }
        public InvoiceState GetInvState(string key)
        {
            if (KeyInvoiceMsg != null)
                return (InvoiceState)int.Parse(KeyInvoiceMsg[key].ToString());
            if (KeyStatus != null)
                return (InvoiceState)int.Parse(KeyStatus[key].ToString());
            return InvoiceState.NaN;
        }
    }

    public class ResponeBase
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public string RawData { get; set; }
    }

    public class ResponeBaseGenneric<T> : ResponeBase
    {
        public T Data { get; set; }
    }
}
