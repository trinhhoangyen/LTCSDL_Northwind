using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTCSDL.BLL
{
    public class EmployeesSvc : GenericSvc<EmployeesRep, Employees>
    {
        public object DoanhThuTheoNgay(DateTime date)
        {
            return _rep.DoanhThuTheoNgay(date);
        }
        //public object DoanhThuTheoNgay_Linq(DateTime date)
        //{
        //    var res = _rep.DoanhThuTheoNgay_Linq(date);
        //    return res;
        //}
        public object DoanhThuTheoThoiGian(OrderFullReq req)
        {
            return _rep.DoanhThuTheoThoiGian(req);
        }
    }
}
