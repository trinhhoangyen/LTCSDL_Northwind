using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTCSDL.BLL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LTCSDL.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public EmployeesController()
        {
            _svc = new EmployeesSvc();
        }

        private readonly EmployeesSvc _svc;

        [HttpPost("get-dtnv-trong-ngay")]
        public IActionResult DoanhThuTheoNgay([FromBody] DatetimeReq req)
        {
            var res = new SingleRsp();
            res.Data = _svc.DoanhThuTheoNgay(req.BeginTime);
            return Ok(res);
        }

        //[HttpPost("get-dtnv-trong-ngay-linq")]
        //public IActionResult DoanhThuTheoNgay_Linq([FromBody] DatetimeReq req)
        //{
        //    var res = new SingleRsp();
        //    res.Data = _svc.DoanhThuTheoNgay_Linq(req.BeginTime);
        //    return Ok(res);
        //}

        [HttpPost("get-doanh-thu-theo-thoi-gian")]
        public IActionResult DoanhThuTheoThoiGian([FromBody] DatetimeReq date)
        {
            var res = new SingleRsp(); 
            res.Data =_svc.DoanhThuTheoThoiGian(date.BeginTime, date.EndTime);
            return Ok(res);
        }
    }
}