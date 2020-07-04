using System;
using System.Collections.Generic;
using System.Text;

namespace LTCSDL.Common.Req
{
    public class OrderTodayReq
    {
        public DateTime Date { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
