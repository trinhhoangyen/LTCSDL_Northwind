using System;

namespace LTCSDL.Common.Req
{
    public class OrderTodayReq
    {
        public DateTime Date { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
