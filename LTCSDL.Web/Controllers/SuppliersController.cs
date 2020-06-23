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
    public class SuppliersController : ControllerBase
    {
        public SuppliersController()
        {
            _svc = new SuppliersSvc();
        }
        private readonly SuppliersSvc _svc;

        [HttpPost("add-supplier")]
        public IActionResult AddSupplier(SupplierReq req)
        {
            var res = _svc.AddSupplier(req);
            return Ok(res);
        }

        [HttpPost("update-supplier")]
        public IActionResult UpdateSupplier(SupplierReq req)
        {
            var res = _svc.UpdateSupplier(req);
            return Ok(res);
        }
    }
}
