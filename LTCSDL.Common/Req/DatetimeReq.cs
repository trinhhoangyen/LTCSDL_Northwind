﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LTCSDL.Common.Req
{
    public class DatetimeReq
    {
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
