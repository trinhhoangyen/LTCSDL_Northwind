using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTCSDL.BLL
{
    public class OrdersSvc: GenericSvc<OrdersRep, Orders>
    {
        #region --Override --
        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();

            var m = _rep.Read(id);
            res.Data = m;

            return res;
        }
        #endregion

        #region Method
        // đề 2
        //2a
        public List<object> GetOrderInSpaceTime(OrderFullReq req)
        {
            List<object> res = new List<object>();
            res = _rep.GetOrderInSpaceTime(req);
            return res;
        }
        public object GetOrderInSpaceTime_Linq(OrderFullReq req)
        {
            return _rep.GetOrderInSpaceTime_Linq(req);
        }
        //2b
        public List<object> GetOrderDetailByOrderId(int id)
        {
            return _rep.GetOrderDetailByOrderId(id);
        }
        public object GetOrderDetailByOrderId_Linq(int id)
        {
            return _rep.GetOrderDetailByOrderId_Linq(id);
        }
        public object GetOrderDetailByOrderId_Linq1(int id)
        {
            return _rep.GetOrderDetailByOrderId_Linq1(id);
        }


        public SingleRsp Createordduct(OrderReq ord)
        {
            var res = new SingleRsp();
            Orders order = new Orders();

            order.OrderId = ord.OrderId;
            order.CustomerId = ord.CustomerId;
            order.EmployeeId = ord.EmployeeId;
            order.OrderDate = ord.OrderDate;
            order.RequiredDate = ord.RequiredDate;
            order.ShippedDate = ord.ShippedDate;
            order.ShipAddress = ord.ShipAddress;
            order.ShipCity = ord.ShipCity;
            order.ShipCountry = ord.ShipCountry;
            order.ShipName = ord.ShipName;
            order.ShippedDate = ord.ShippedDate;
            order.ShipPostalCode = ord.ShipPostalCode;
            order.ShipRegion = ord.ShipRegion;
            order.ShipVia = ord.ShipVia;
            order.Freight = ord.Freight;

            res = _rep.CreateOrder(order);
            res.Data = order;
            return res;
        }

        public SingleRsp UpdateProduct(OrderReq ord)
        {
            var res = new SingleRsp();
            Orders order = new Orders();

            order.OrderId = ord.OrderId;
            order.CustomerId = ord.CustomerId;
            order.EmployeeId = ord.EmployeeId;
            order.OrderDate = ord.OrderDate;
            order.RequiredDate = ord.RequiredDate;
            order.ShippedDate = ord.ShippedDate;
            order.ShipAddress = ord.ShipAddress;
            order.ShipCity = ord.ShipCity;
            order.ShipCountry = ord.ShipCountry;
            order.ShipName = ord.ShipName;
            order.ShippedDate = ord.ShippedDate;
            order.ShipPostalCode = ord.ShipPostalCode;
            order.ShipRegion = ord.ShipRegion;
            order.ShipVia = ord.ShipVia;
            order.Freight = ord.Freight;

            res = _rep.UpdateOrder(order);
            res.Data = order;
            return res;
        }

        public List<object> GetCustOrderHist(string id)
        {
            List<object> res = new List<object>();
            res = _rep.GetCustOrderHist(id);
            return res;
        }

        public object GetCustOrderHist_Linq(string id)
        {
            object res = new List<object>();
            res = _rep.GetCustOrderHist_Linq(id);
            return res;
        }

        public object GetCustOrdersDetail_Linq(int orderId)
        {
            object res = new List<object>();
            res = _rep.GetCustOrdersDetail_Linq(orderId);
            return res;
        }

        public object GetCustOrdersDetail_Linq1(int orderId)
        {
            object res = new List<object>();
            res = _rep.GetCustOrdersDetail_Linq1(orderId);
            return res;
        }
        public object GetOrderOfEmployee(OrderFullReq req)
        {
            return _rep.GetOrderOfEmployee(req);
        }
        public object GetOrderOfEmployee_Linq(OrderFullReq req)
        {
            return _rep.GetOrderOfEmployee_Linq(req);
        }
        public object GetBestSeller(OrderFullReq req)
        {
            return _rep.GetBestSeller(req);
        }

        public object DoanhThuTheoQG(OrderFullReq req)
        {
            return _rep.DoanhThuTheoQG(req);
        }
        public object SearchOrder(SearchReq req)
        {
            return _rep.SearchOrder(req);
        }
        public object OrdersInDay(OrderTodayReq req)
        {
            return _rep.OrdersInDay(req);
        }
        #endregion
    }
}
