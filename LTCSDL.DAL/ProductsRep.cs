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
    public class ProductsRep : GenericRep<NorthwindContext, Products>
    {
        #region --Override--
        public override Products Read(int id)
        {
            var res = All.FirstOrDefault(p => p.ProductId == id);
            return res;
        }

        public int Remove(int id)
        {
            var m = All.First(i => i.ProductId == id);
            m = base.Delete(m);
            return m.ProductId;
        }
        #endregion

        #region -- Methods --
        public SingleRsp CreateProduct(Products pro)
        {
            var res = new SingleRsp();
            // using để dùng được transaction
            using(var context = new NorthwindContext())
            {
                // dùng tran để nếu khi insert sai hoặc có vấn đề gì đó nó tự động pass chứ k insert dữ liệu
                using(var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var t = context.Products.Add(pro);
                        context.SaveChanges();
                        tran.Commit();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        res.SetError(ex.StackTrace);
                    }
                }    
            }    
            return res;
        }
        public SingleRsp UpdateProduct(Products pro)
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
                        var t = context.Products.Update(pro);
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
        public List<object> ProductNotOrder(GetProductReq req)
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
                cmd.CommandText = "pr_ProductsNotOrder";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@date", req.Date);
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
                            ProductId = row["ProductId"],
                            ProductName = row["ProductName"],
                            SupplierId = row["SupplierId"],
                            CategoryId = row["CategoryId"],
                            QuantityPerUnit = row["QuantityPerUnit"],
                            UnitPrice = row["UnitPrice"],
                            UnitsInStock = row["UnitsInStock"],
                            UnitsOnOrder = row["UnitsOnOrder"],
                            ReorderLevel = row["ReorderLevel"],
                            Discontinued = row["Discontinued"]

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

        public object QuantityProducts(TimeReq req)
        {
            var res = Context.Orders.Join(Context.OrderDetails, a => a.OrderId, b => b.OrderId, (a, b) => new
            {
                a.OrderId,
                a.OrderDate,
                a.ShippedDate,
                b.Quantity
            }).Where(x => x.OrderDate >= req.DateFrom && x.OrderDate <= req.DateTo && x.ShippedDate == null).ToList();

            var data = res.GroupBy(x => x.OrderDate)
                .Select(x => new
                {
                    OrderDate = x.First().OrderDate,
                    SoLuong = x.Sum(x => x.Quantity)
                }).ToList();
            return data;
        }
        #endregion
    }
}
