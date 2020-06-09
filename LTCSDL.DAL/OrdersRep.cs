using LTCSDL.Common.DAL;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LTCSDL.DAL
{
    public class OrdersRep : GenericRep<NorthwindContext, Orders>
    {
        #region --Override--
        public override Orders Read(int id)
        {
            var res = All.FirstOrDefault(p => p.OrderId == id);
            return res;
        }

        public int Remove(int id)
        {
            var m = All.First(i => i.OrderId == id);
            m = base.Delete(m);
            return m.OrderId;
        }
        #endregion

        #region -- Methods --
        public SingleRsp CreateOrder(Orders pro)
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
                        var t = context.Orders.Add(pro);
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

        public SingleRsp UpdateOrder(Orders pro)
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
                        var t = context.Orders.Update(pro);
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

        public List<object> GetCustOrderHist(String custID)
        {
            List<Object> res = new List<object>();
            var cnn = (SqlConnection)Context.Database.GetDbConnection();
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = "CustOrderHist";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", custID);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            ProductName = row["ProductName"],
                            Total = row["Total"]
                        };
                        res.Add(x);
                    }
                }
            } catch (Exception ex)
            {
                res = null;
            }
            return res;
        }

        public object GetCustOrderHist_Linq(String custID)
        {
            var pro = Context.Products.Join(Context.OrderDetails, a => a.ProductId, b => b.ProductId, (a, b) => new
            {
                a.ProductId,
                a.ProductName,
                b.Quantity,
                b.OrderId
            }).Join(Context.Orders, a => a.OrderId, b => b.OrderId, (a, b) => new
            {
                a.ProductName,
                a.ProductId,
                a.Quantity,
                b.CustomerId
            }).Where(x => x.CustomerId == custID).ToList();
            var res = pro.GroupBy(x => x.ProductName)
                .Select(x => new
                {
                    ProductName = x.First().ProductName,
                    ProductId = x.First().ProductId,
                    Total = x.Sum(x => x.Quantity)
                }).ToList();
            return res;
        }

        public object GetCustOrdersDetail_Linq(int orderId)
        {
            var res = from p in Context.Products
                      join d in Context.OrderDetails on p.ProductId equals d.ProductId
                      where d.OrderId == orderId
                      select new
                      {
                          d.OrderId,
                          p.ProductName,
                          d.UnitPrice,
                          d.Quantity,
                          Discount = d.Discount * 100,
                          ExtendedPrice = ((decimal)d.Quantity * (1 - (decimal)d.Discount) * (d.UnitPrice))
                      };
            return res;
        }

        public object GetCustOrdersDetail_Linq1(int orderId)
        {
            var res = Context.Products.Join(Context.OrderDetails, a => a.ProductId, b => b.ProductId, (a, b) => new
            {
                a.ProductName,
                b.UnitPrice,
                b.Quantity,
                b.OrderId,
                Discount = b.Discount * 100,
                ExtendedPrice = b.Quantity * (1 - (decimal)b.Discount) * b.UnitPrice
            }).Where(x => x.OrderId == orderId).ToList();
            return res;
        }

        public List<object> GetOrderInSpaceTime(DateTime dateFrom, DateTime dateTo, int page, int size)
        {
            List<object> res = new List<object>();
            var cnn = (SqlConnection)Context.Database.GetDbConnection();
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = "dh_DonHangTheoNgay";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                cmd.Parameters.AddWithValue("@dateTo", dateTo);
                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@size", size);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            STT = row["STT"],
                            OrderId = row["OrderId"],
                            CustomerId = row["CustomerId"],
                            EmployeeId = row["EmployeeId"],
                            OrderDate = row["OrderDate"],
                            RequiredDate = row["RequiredDate"],
                            ShippedDate = row["ShippedDate"],
                            ShipVia = row["ShipVia"],
                            Freight = row["Freight"],
                            ShipName = row["ShipName"],
                            ShipAddress = row["ShipAddress"],
                            ShipCity = row["ShipCity"],
                            ShipRegion = row["ShipRegion"],
                            ShipPostalCode = row["ShipPostalCode"],
                            ShipCountry = row["ShipCountry"]

                        };
                        res.Add(x);
                    }
                }
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }

        public object GetOrderInSpaceTime_Linq(DateTime dateFrom, DateTime dateTo, int page, int size)
        {
            var res = All.Where(x => x.OrderDate >= dateFrom && x.OrderDate <= dateTo);
            var offSet = (page - 1) * size;
            var total = res.Count();
            int totalPage = (total % size) == 0 ? (int)(total / size) : ((int)(total / size) + 1);
            var data = res.OrderBy(x => x.OrderDate).Skip(offSet).Take(size).ToList();
            List<object> lst = new List<object>();
            for(int i = 0; i <  data.Count(); i++)
            {
                var item = data[i];
                var tam = new
                {
                    STT = i + 1 + offSet,
                    OrderId = item.OrderId,
                    CustomerId = item.CustomerId,
                    EmployeeId = item.EmployeeId,
                    OrderDate = item.OrderDate,
                    RequiredDate = item.RequiredDate,
                    ShippedDate = item.ShippedDate,
                    ShipVia = item.ShipVia,
                    Freight = item.Freight,
                    ShipName = item.ShipName,
                    ShipAddress = item.ShipAddress,
                    ShipCity = item.ShipCity,
                    ShipRegion = item.ShipRegion,
                    ShipPostalCode = item.ShipPostalCode,
                    ShipCountry = item.ShipCountry

                };
                lst.Add(tam);
            }    
            return new
            {
                Data = lst,
                TotalRecords = total,
                Page = page,
                Size = size,
                TotalPages = totalPage
            };
        }

        public List<object> GetOrderDetailByOrderId(int id)
        {
            List<object> res = new List<object>();
            var cnn = (SqlConnection)Context.Database.GetDbConnection();
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                var cmd = cnn.CreateCommand();
                cmd.CommandText = "dH_ChiTietDonHang";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", id);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            OrderId = row["OrderId"],
                            ProductName = row["ProductName"],
                            EmployeeId = row["EmployeeId"],
                            CustomerId = row["CustomerId"],
                            OrderDate = row["OrderDate"],
                            Total = row["Total"]
                        };
                        res.Add(x);
                    }
                }
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }
        #endregion
    }
}
