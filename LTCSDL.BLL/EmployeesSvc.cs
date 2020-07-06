using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;
using System;

namespace LTCSDL.BLL
{
    public class EmployeesSvc : GenericSvc<EmployeesRep, Employees>
    {// 1a
        public object DoanhThuNVTheoNgay(DateTime date)
        {
            return _rep.DoanhThuNVTheoNgay(date);
        }
        public object DoanhThuNVTheoNgay_Linq(DateTime date)
        {
            var res = _rep.DoanhThuNVTheoNgay_Linq(date);
            return res;
        }
        public object DoanhThuNVTheoNgay_Linq1(DateTime date)
        {
            var res = _rep.DoanhThuNVTheoNgay_Linq1(date);
            return res;
        }
        // 1b
        public object DoanhThuNVTheoThoiGian(TimeReq req)
        {
            return _rep.DoanhThuNVTheoThoiGian(req);
        }
        public object DoanhThuNVTheoThoiGian_Linq(TimeReq req)
        {
            return _rep.DoanhThuNVTheoThoiGian_Linq(req);
        }
    }
}
