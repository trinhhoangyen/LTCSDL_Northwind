using LTCSDL.BLL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
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

        [HttpPost("doanh-thu-nv-theo-ngay")]
        public IActionResult DoanhThuNVTheoNgay([FromBody] TimeReq req)
        {
            var res = new SingleRsp();
            res.Data = _svc.DoanhThuNVTheoNgay(req.DateFrom);
            return Ok(res);
        }

        [HttpPost("doanh-thu-nv-theo-ngay-linq")]
        public IActionResult DoanhThuNVTheoNgay_Linq([FromBody] TimeReq req)
        {
            var res = new SingleRsp();
            res.Data = _svc.DoanhThuNVTheoNgay_Linq(req.DateFrom);
            return Ok(res);
        }
        [HttpPost("doanh-thu-nv-theo-ngay-linq1")]
        public IActionResult DoanhThuNVTheoNgay_Linq1([FromBody] TimeReq req)
        {
            var res = new SingleRsp();
            res.Data = _svc.DoanhThuNVTheoNgay_Linq1(req.DateFrom);
            return Ok(res);
        }

        [HttpPost("doanh-thu-nv-theo-thoi-gian")]
        public IActionResult DoanhThuNVTheoThoiGian([FromBody] TimeReq req)
        {
            var res = new SingleRsp();
            res.Data = _svc.DoanhThuNVTheoThoiGian(req);
            return Ok(res);
        }
        [HttpPost("doanh-thu-nv-theo-thoi-gian-linq")]
        public IActionResult DoanhThuNVTheoThoiGian_Linq([FromBody] TimeReq req)
        {
            var res = new SingleRsp();
            res.Data = _svc.DoanhThuNVTheoThoiGian_Linq(req);
            return Ok(res);
        }
    }
}