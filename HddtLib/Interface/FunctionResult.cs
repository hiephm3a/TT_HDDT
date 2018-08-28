using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HddtLib.Interface
{
    public class FunctionResult<T>
    {
        public ResultCode Status { get; set; }

        public string Message { get; set; }
        public object OtherInfo { get; set; }
        public byte[] ResponeData { get; set; }

        public T Data { get; set; }
    }


    public enum ResultCode
    {
        Success,
        Error,
        Warning,
        Exception
    }
}
