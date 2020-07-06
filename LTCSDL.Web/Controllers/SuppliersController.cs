using LTCSDL.BLL;
using LTCSDL.Common.Req;
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

        #region -- đề 4 --
        [HttpPost("add-supplier")]
        public IActionResult AddSupplier([FromBody] SupplierReq req)
        {
            var res = _svc.AddSupplier(req);
            return Ok(res);
        }

        [HttpPost("add-supplier-linq")]
        public IActionResult AddSupplier_Linq([FromBody] SupplierReq req)
        {
            var res = _svc.AddSupplier_Linq(req);
            return Ok(res);
        }

        [HttpPost("update-supplier")]
        public IActionResult UpdateSupplier([FromBody] SupplierReq req)
        {
            var res = _svc.UpdateSupplier(req);
            return Ok(res);
        }

        [HttpPost("update-supplier-linq")]
        public IActionResult UpdateSupplier_Linq([FromBody] SupplierReq req)
        {
            var res = _svc.UpdateSupplier_Linq(req);
            return Ok(res);
        }
        #endregion
    }
}
