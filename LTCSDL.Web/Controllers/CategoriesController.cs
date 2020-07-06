﻿using Microsoft.AspNetCore.Mvc;

namespace LTCSDL.Web.Controllers
{
    using BLL;
    using Common.Req;
    using Common.Rsp;

    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController()
        {
            _svc = new CategoriesSvc();
        }

        [HttpPost("get-by-id")]
        public IActionResult getCategoryById([FromBody] SimpleReq req)
        {
            var res = new SingleRsp();

            res = _svc.Read(req.Id);

            return Ok(res);
        }

        [HttpPost("get-all")]
        public IActionResult getAllCategory()
        {
            var res = new SingleRsp();
            res.Data = _svc.All;
            return Ok(res);
        }

        private readonly CategoriesSvc _svc;
    }
}