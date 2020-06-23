using LTCSDL.Common.DAL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTCSDL.DAL
{
    public class ShipperRep : GenericRep<NorthwindContext, Shippers>
    {
        #region --Override--
        public override Shippers Read(int id)
        {
            var res = All.FirstOrDefault(p => p.ShipperId == id);
            return res;
        }

        public int Remove(int id)
        {
            var m = All.First(i => i.ShipperId == id);
            m = base.Delete(m);
            return m.ShipperId;
        }
        #endregion

        #region Methods
        public SingleRsp CreateShipper(Shippers ship)
        {
            var res = new SingleRsp();
            // using để dùng được transaction
            using (var context = new NorthwindContext())
            {
                // dùng tran để nếu khi insert sai hoặc có vấn đề gì đó nó tự động pass chứ k insert dữ liệu
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var t = context.Shippers.Add(ship);
                        context.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        res.SetError(ex.StackTrace);
                    }
                }
            }
            return res;
        }
        public SingleRsp UpdateShipper(Shippers ship)
        {
            var res = new SingleRsp();
            // using để dùng được transaction
            using (var context = new NorthwindContext())
            {
                // dùng tran để nếu khi insert sai hoặc có vấn đề gì đó nó tự động pass chứ k insert dữ liệu
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var t = context.Shippers.Update(ship);
                        context.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        res.SetError(ex.StackTrace);
                    }
                }
            }
            return res;
        }
        public SingleRsp DoanhThuShipper(DateReq req)
        {
            var res = from s in Context.Shippers
                      join o in Context.Orders on s.ShipperId equals o.ShipVia
                      where o.OrderDate.Value.Month == req.Month && o.OrderDate.Value.Year == req.Year
                      select new
                      {
                          s.ShipperId,
                          o.Freight
                      };

            SingleRsp result = new SingleRsp();
            result.Data = from r in res
                          group r by new
                          {
                              r.ShipperId
                          } into e
                          select new
                          {
                              e.Key.ShipperId,
                              DoanhThu = e.Sum(x => x.Freight).ToString()
                          };
            return result;
        }
        #endregion
    }
}
