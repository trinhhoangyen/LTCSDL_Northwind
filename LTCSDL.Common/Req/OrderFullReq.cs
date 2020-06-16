using System;
using System.Collections.Generic;
using System.Text;

namespace LTCSDL.Common.Req
{
    public class OrderFullReq
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string Keyword { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int IsQuantity { get; set; }
    }
}
