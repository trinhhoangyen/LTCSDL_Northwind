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

        #region -- đề 2 --
        /// <summary>
        /// 2a: danh sách đơn hàng
        /// </summary>
        /// <param name="req"></param>
        /// <returns>danh sách đơn hàng trong khoảng thời gian</returns>
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
                cmd.CommandText = "dh_DonHangTheoThoiGian";
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
        // linq
        public object GetOrderInSpaceTime_Linq(OrderFullReq req)
        {
            var res = All.Where(x => x.OrderDate >= req.DateFrom && x.OrderDate <= req.DateTo);
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
                    item.OrderId,
                    item.CustomerId,
                    item.EmployeeId,
                    item.OrderDate,
                    item.RequiredDate,
                    item.ShippedDate,
                    item.ShipVia,
                    item.Freight,
                    item.ShipName,
                    item.ShipAddress,
                    item.ShipCity,
                    item.ShipRegion,
                    item.ShipPostalCode,
                    item.ShipCountry
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

        /// <summary>
        /// 2b: chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                cmd.CommandText = "dh_ChiTietDonHang";
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
                      join p in Context.Products on od.ProductId equals p.ProductId
                      where o.OrderId == id
                      select new
                      {
                          o.OrderId,
                          p.ProductName,
                          o.EmployeeId,
                          o.CustomerId,
                          o.OrderDate,
                          Total = od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount)
                      };
            return res;
        }
        public object GetOrderDetailByOrderId_Linq1(int id)
        {
            var res = Context.Products
                    .Join(Context.OrderDetails, p => p.ProductId, od => od.ProductId, (p, od) => new
                    {
                        od.OrderId,
                        p.ProductName,
                        Total = od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount)
                    })
                    .Where(x => x.OrderId == id)
                    .Join(Context.Orders, od => od.OrderId, o => o.OrderId, (od, o) => new
                    {
                        o.OrderId,
                        o.EmployeeId,
                        o.CustomerId,
                        o.OrderDate,
                        od.ProductName,
                        od.Total
                    });
            return res;
        }
        #endregion

        #region -- đề 3 --
        /// <summary>
        /// 3a: danh sách đơn hàng của nhân viên
        /// </summary>
        /// <param name="req"></param>
        /// <returns>danh sách đơn hàng của nhân viên trong khoảng thời gian</returns>
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
        //phân trang linq method
        public object GetOrderOfEmployee_Linq(OrderFullReq req)
        {
            var res = Context.Orders.Join(Context.Employees, o => o.EmployeeId, e => e.EmployeeId, (o, e) => new
            {
                o.OrderId,
                o.CustomerId,
                o.EmployeeId,
                o.OrderDate,
                o.RequiredDate,
                o.ShippedDate,
                o.ShipVia,
                o.Freight,
                o.ShipName,
                o.ShipAddress,
                o.ShipCity,
                o.ShipRegion,
                o.ShipPostalCode,
                o.ShipCountry,
                e.FirstName,
                e.LastName,
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
                    item.OrderId,
                    item.CustomerId,
                    item.EmployeeId,
                    item.OrderDate,
                    item.RequiredDate,
                    item.ShippedDate,
                    item.ShipVia,
                    item.Freight,
                    item.ShipName,
                    item.ShipAddress,
                    item.ShipCity,
                    item.ShipRegion,
                    item.ShipPostalCode,
                    item.ShipCountry
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

        /// <summary>
        /// 3b: danh sách sản phẩm bán chạy
        /// </summary>
        /// <param name="req"></param>
        /// <returns>danh sách sản phẩm bán chạy trong tháng năm nhập vô, theo số lượng hoặc doanh thu</returns>
        public object GetBestSeller(OrderFullReq req)
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
                cmd.CommandText = "dh_DSSPBanChay";
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
                    if (req.IsQuantity == 1)
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
                    else if (req.IsQuantity == 0)
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
        public object GetBestSeller_Linq(OrderFullReq req)
        {
            var res = from p in Context.Products
                      join od in Context.OrderDetails on p.ProductId equals od.ProductId
                      join o in Context.Orders on od.OrderId equals o.OrderId
                      group od by new { p.ProductId, p.ProductName } into g
                      select new
                      {
                          g.Key.ProductId,
                          g.Key.ProductName,
                          DoanhThu = g.Sum(x => x.Quantity * x.UnitPrice * (1 - (decimal)x.Discount))
                      };
            var offSet = (req.Page - 1) * req.Size;
            var total = res.Count();
            int totalPage = (total % req.Size) == 0 ? (int)(total / req.Size) : ((int)(total / req.Size) + 1);

            var data = res.OrderByDescending(x => x.DoanhThu).Skip(offSet).Take(req.Size).ToList();
            List<object> lst = new List<object>();
            for (int i = 0; i < data.Count(); i++)
            {
                var item = data[i];
                var tam = new
                {
                    STT = i + 1 + offSet,
                    item.ProductId,
                    item.ProductName,
                    item.DoanhThu
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

        /// <summary>
        /// 3b: doanh thu quốc gia trong tháng năm nhập vô
        /// </summary>
        /// <param name="req"></param>
        /// <returns>doanh thu quốc gia trong tháng năm nhập vô</returns>
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
                            DoanhThu = Math.Round(float.Parse(row["DoanhThu"].ToString()), 2)
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
        // suntax
        public object DoanhThuTheoQG_Linq(OrderFullReq req)
        {
            var res = from o in Context.Orders
                      where o.OrderDate.Value.Month == req.Month && o.OrderDate.Value.Year == req.Year
                      join od in Context.OrderDetails on o.OrderId equals od.OrderId
                      group od by o.ShipCountry into g
                      select new
                      {
                          g.Key,
                          DoanhThu = g.Sum(x => x.Quantity * x.UnitPrice * (1 - (decimal)x.Discount))
                      };

            return res;
        }
        // linq method
        public object DoanhThuTheoQG_Linq1(OrderFullReq req)
        {
            var res = Context.Orders
                    .Where(x => x.OrderDate.Value.Month == req.Month && x.OrderDate.Value.Year == req.Year)
                    .Join(Context.OrderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new
                    {
                        o.ShipCountry,
                        DoanhThu = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)
                    })
                    .GroupBy(x => new { x.ShipCountry })
                    .Select(x => new
                    {
                        x.Key.ShipCountry,
                        DoanhThu = x.Sum(x => x.DoanhThu)
                    });
            return res;
        }
        #endregion

        #region -- đề 5 --
        /// <summary>
        /// 1c: tìm kiếm hóa đơn theo CompanyName, EmployeeName
        /// </summary>
        /// <param name="req"></param>
        /// <returns>danh sách đơn hàng</returns>
        public object SearchOrder(SearchReq req)
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
                cmd.CommandText = "hd_SearchOrder";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@keyword", req.Keyword);
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
                            OrderId = row["OrderId"],
                            CustomerId = row["CustomerId"],
                            EmployeeName = row["FirstNameEmplpyee"].ToString() + " " + row["LastNameEmployee"].ToString(),
                            CompanyName = row["CompanyName"],
                            OrderDate = row["OrderDate"],
                            RequiredDate = row["RequiredDate"],
                            ShippedDate = row["ShippedDate"]

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

        /// <summary>
        /// câu 4: danh sách đơn hàng theo ngày nhập vô
        /// </summary>
        /// <param name="req"></param>
        /// <returns>danh sách đơn hàng, tên khách hàng, địa chỉ cần giao trong ngày đó có phân trang</returns>
        public object OrdersInDay(OrderTodayReq req)
        {
            var res = from o in Context.Orders
                      join c in Context.Customers on o.CustomerId equals c.CustomerId
                      where o.OrderDate == req.Date
                      select new
                      {
                          o.OrderId,
                          o.OrderDate,
                          c.ContactName,
                          o.ShipAddress
                      };
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
                    item.OrderId,
                    item.OrderDate,
                    item.ContactName,
                    item.ShipAddress

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
        #endregion

        #region No Complete
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
            }
            catch (Exception ex)
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
        #endregion
    }
}
