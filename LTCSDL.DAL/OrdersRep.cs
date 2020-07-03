using LTCSDL.Common.DAL;
using LTCSDL.Common.Req;
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

        public object GetCustOrderHist_Linq(string custID)
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
                          ExtendedPrice = (d.Quantity * (1 - (decimal)d.Discount) * (d.UnitPrice))
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

        public List<object> GetOrderInSpaceTime(OrderFullReq req)
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
                cmd.Parameters.AddWithValue("@dateFrom", req.DateFrom);
                cmd.Parameters.AddWithValue("@dateTo", req.DateTo);
                cmd.Parameters.AddWithValue("@page", req.Page);
                cmd.Parameters.AddWithValue("@size", req.Size);
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

        public object GetOrderInSpaceTime_Linq(OrderFullReq req)
        {
            var res = All.Where(x => x.OrderDate >= req.DateFrom && x.OrderDate <= req.DateTo);
            var offSet = (req.Page - 1) * req.Size;
            var total = res.Count();
            int totalPage = (total % req.Size) == 0 ? (int)(total / req.Size) : ((int)(total / req.Size) + 1);
            var data = res.OrderBy(x => x.OrderDate).Skip(offSet).Take(req.Size).ToList();
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
                Page = req.Page,
                Size = req.Size,
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
        public object GetOrderDetailByOrderId_Linq(int id)
        {
            var res = from o in Context.Orders
                      join od in Context.OrderDetails on o.OrderId equals od.OrderId
                      join c in Context.Customers on o.CustomerId equals c.CustomerId
                      join p in Context.Products on od.ProductId equals p.ProductId
                      select new
                      {
                          o.OrderId,
                          p.ProductName,
                          o.EmployeeId,
                          o.CustomerId,
                          o.OrderDate,
                          Total = od.Quantity * od.UnitPrice * (1- (decimal)od.Discount)
                      };
            return res;
        }
        public List<object> GetOrderOfEmployee(OrderFullReq req)
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
                cmd.CommandText = "dh_DSDHNV";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@page", req.Page);
                cmd.Parameters.AddWithValue("@size", req.Size);
                cmd.Parameters.AddWithValue("@keyword", req.Keyword);
                cmd.Parameters.AddWithValue("@dateFrom", req.DateFrom);
                cmd.Parameters.AddWithValue("@dateTo", req.DateTo);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
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

        public object GetOrderOfEmployee_Linq(OrderFullReq req)
        {
            var res = Context.Orders.Join(Context.Employees, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new
            {
                a.OrderId,
                a.CustomerId,
                a.EmployeeId,
                a.OrderDate,
                b.FirstName,
                b.LastName
            }).Where(x => x.LastName.Equals(req.Keyword) && x.OrderDate >= req.DateFrom && x.OrderDate <= req.DateTo).ToList();
            
            var offSet = (req.Page - 1) * req.Size;
            var total = res.Count();
            int totalPage = (total % req.Size) == 0 ? (int)(total / req.Size) : ((int)(total / req.Size) + 1);
            var data = res.OrderBy(x => x.OrderDate).Skip(offSet).Take(req.Size).ToList();
            List<object> lst = new List<object>();
            for (int i = 0; i < data.Count(); i++)
            {
                var item = data[i];
                var tam = new
                {
                    STT = i + 1 + offSet,
                    OrderId = item.OrderId,
                    CustomerId = item.CustomerId,
                    EmployeeId = item.EmployeeId,
                    OrderDate = item.OrderDate,
                    FirstName = item.FirstName,
                    LastName = item.LastName
                };
                lst.Add(tam);
            }
            return new
            {
                Data = lst,
                TotalRecords = total,
                Page = req.Page,
                Size = req.Size,
                TotalPages = totalPage
            };
        }

        public List<object> GetBestSeller(OrderFullReq req)
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
                cmd.CommandText = "dh_DSMatHangChayNhat";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@page", req.Page);
                cmd.Parameters.AddWithValue("@size", req.Size);
                cmd.Parameters.AddWithValue("@month", req.Month);
                cmd.Parameters.AddWithValue("@year", req.Year);
                cmd.Parameters.AddWithValue("@isQuantity", req.IsQuantity);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if( req.IsQuantity == 1)
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {   
                            var x = new
                            {
                                ProductId = row["ProductId"],
                                ProductName = row["ProductName"],
                                DoanhThu = row["DoanhThu"]
                            };
                            res.Add(x);
                        }
                    else if(req.IsQuantity == 0)
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                var x = new
                                {
                                    ProductId = row["ProductId"],
                                    ProductName = row["ProductName"],
                                    SoLuong = row["SoLuong"]
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

        public List<object> DoanhThuTheoQG(OrderFullReq req)
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
                cmd.CommandText = "dh_DoanhThuTheoQG";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", req.Month);
                cmd.Parameters.AddWithValue("@year", req.Year);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            Country = row["Country"],
                            DoanhThu = row["DoanhThu"]
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

        // error
        public object DoanhThuTheoQG_Linq(OrderFullReq req)
        {
            var res = Context.Orders.Join(Context.Customers, a => a.CustomerId, b => b.CustomerId, (a, b) => new
            {
                a.OrderDate,
                b.Country
            }).Where(x => x.OrderDate.Value.Month == req.Month && x.OrderDate.Value.Year == req.Year).ToList();

            return res;
        }
        #endregion
    }
}
