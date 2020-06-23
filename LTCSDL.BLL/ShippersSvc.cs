using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTCSDL.BLL
{
    public class ShippersSvc : GenericSvc<ShipperRep, Shippers>
    {
        #region override
        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();

            var m = _rep.Read(id);
            res.Data = m;

            return res;
        }

        public override SingleRsp Update(Shippers m)
        {
            var res = new SingleRsp();
            var m1 = m.ShipperId > 0 ? _rep.Read(m.ShipperId) : _rep.Read(m.CompanyName);
            if (m1 == null)
                res.SetError("EZ103", "No Data.");
            else
            {
                res = base.Update(m);
                res.Data = m;
            }
            return res;
        }
        #endregion

        #region Methods
        public SingleRsp CreateShipper(ShipperReq req)
        {
            var res = new SingleRsp();
            Shippers obj = new Shippers();

            obj.ShipperId = req.ShipperId;
            obj.CompanyName = req.CompanyName;
            obj.Phone = req.Phone;

            res = _rep.CreateShipper(obj);
            res.Data = obj;
            return res;
        }

        public SingleRsp UpdateShipper(ShipperReq req)
        {
            var res = new SingleRsp();
            Shippers obj = new Shippers();

            obj.ShipperId = req.ShipperId;
            obj.CompanyName = req.CompanyName;
            obj.Phone = req.Phone;

            res = _rep.UpdateShipper(obj);
            res.Data = obj;
            return res;
        }
        public SingleRsp DoanhThuShipper(DateReq req)
        {
            return _rep.DoanhThuShipper(req);
        }
        #endregion
    }
}
