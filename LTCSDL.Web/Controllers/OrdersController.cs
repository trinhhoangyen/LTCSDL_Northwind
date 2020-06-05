using LTCSDL.BLL;
using LTCSDL.Common.DAL;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTCSDL.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public OrdersController()
        {
            _svc = new OrdersSvc();
        }

        [HttpPost("get-all")]
        public IActionResult getAllProduct()
        {
            var res = new SingleRsp();
            res.Data = _svc.All;
            return Ok(res);
        }

        [HttpPost("get-customer-order-history")]
        public IActionResult GetCustOrderHist(string id)
        {
            var res = new SingleRsp();
            res.Data = _svc.GetCustOrderHist(id);
            return Ok(res);
        }

        [HttpPost("get-customer-order-history-linq")]
        public IActionResult GetCustOrderHistLinq(string id)
        {
            var res = new SingleRsp();
            res.Data = _svc.GetCustOrderHist_Linq(id);
            return Ok(res);
        }

        [HttpPost("get-customer-order-detail-linq")]
        public IActionResult GetCustOrdersDetailLinq(int orderId)
        {
            var res = new SingleRsp();
            res.Data = _svc.GetCustOrdersDetail_Linq(orderId);
            return Ok(res);
        }

        [HttpPost("get-customer-order-detail-linq1")]
        public IActionResult GetCustOrdersDetailLinq1(int orderId)
        {
            var res = new SingleRsp();
            res.Data = _svc.GetCustOrdersDetail_Linq(orderId);
            return Ok(res);
        }

        private readonly OrdersSvc _svc;
    }
}