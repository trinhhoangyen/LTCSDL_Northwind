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
    public class ShippersController : ControllerBase
    {
        public ShippersController()
        {
            _svc = new ShippersSvc();
        }

        [HttpPost("get-by-id")]
        public IActionResult getCategoryById([FromBody] SimpleReq req)
        {
            var res = new SingleRsp();

            res = _svc.Read(req.Id);

            return Ok(res);
        }

        [HttpPost("get-all")]
        public IActionResult getAllShipper()
        {
            var res = new SingleRsp();
            res.Data = _svc.All;
            return Ok(res);
        }


        [HttpPost("create-shipper")]
        public IActionResult CreateShipper(ShipperReq req)
        {
            var res = _svc.CreateShipper(req);
            return Ok(res);
        }

        [HttpPost("update-shipper")]
        public IActionResult UpdateShipper(ShipperReq req)
        {
            var res = _svc.UpdateShipper(req);
            return Ok(res);
        }


        [HttpPost("doanh-thu-shipper-theo-thang-nam")]
        public IActionResult DoanhThuShipper(DateReq req)
        {
            var res = _svc.DoanhThuShipper(req);
            return Ok(res);
        }

        private readonly ShippersSvc _svc;
    }
}
