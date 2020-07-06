using LTCSDL.Common.DAL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace LTCSDL.DAL
{
    public class SuppliersRep : GenericRep<NorthwindContext, Suppliers>
    {
        #region --Override--
        public override Suppliers Read(int id)
        {
            var res = All.FirstOrDefault(s => s.SupplierId == id);
            return res;
        }

        public int Remove(int id)
        {
            var m = All.First(i => i.SupplierId == id);
            m = base.Delete(m);
            return m.SupplierId;
        }
        #endregion

        #region -- đề 4 --
        /// <summary>
        /// 4a: Thêm mới 1 nhà cung cấp
        /// </summary>
        /// <param name="sup"></param>
        /// <returns>Nhà cung cấp mới thêm</returns>
        public object AddSupplier(SupplierReq sup)
        {
            object res = new object();
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
                cmd.CommandText = "sp_AddSupplier";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CompanyName", sup.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", sup.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", sup.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", sup.CompanyName);
                cmd.Parameters.AddWithValue("@City", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Region", sup.CompanyName);
                cmd.Parameters.AddWithValue("@PostalCode", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Country", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Phone", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Fax", sup.CompanyName);
                cmd.Parameters.AddWithValue("@HomePage", sup.CompanyName);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            SupplierId = row["SupplierId"],
                            CompanyName = row["CompanyName"],
                            ContactName = row["ContactName"],
                            ContactTitle = row["ContactTitle"],
                            Address = row["Address"],
                            City = row["City"],
                            Region = row["Region"],
                            PostalCode = row["PostalCode"],
                            Country = row["Country"],
                            Phone = row["Phone"],
                            Fax = row["Fax"],
                            HomePage = row["HomePage"],
                        };
                        res = x;
                    }
                }
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }
        public SingleRsp AddSupplier_Linq(Suppliers obj)
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
                        var t = context.Suppliers.Add(obj);
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

        /// <summary>
        /// 4b: Sửa mới 1 nhà cung cấp
        /// </summary>
        /// <param name="sup"></param>
        /// <returns>Nhà cung cấp mới sửa</returns>
        public object UpdateSupplier(SupplierReq sup)
        {
            object res = new object();
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
                cmd.CommandText = "sp_UpdateSupplier";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SupplierId", sup.SupplierId);
                cmd.Parameters.AddWithValue("@CompanyName", sup.CompanyName);
                cmd.Parameters.AddWithValue("@ContactName", sup.ContactName);
                cmd.Parameters.AddWithValue("@ContactTitle", sup.ContactTitle);
                cmd.Parameters.AddWithValue("@Address", sup.CompanyName);
                cmd.Parameters.AddWithValue("@City", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Region", sup.CompanyName);
                cmd.Parameters.AddWithValue("@PostalCode", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Country", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Phone", sup.CompanyName);
                cmd.Parameters.AddWithValue("@Fax", sup.CompanyName);
                cmd.Parameters.AddWithValue("@HomePage", sup.CompanyName);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            SupplierId = row["SupplierId"],
                            CompanyName = row["CompanyName"],
                            ContactName = row["ContactName"],
                            ContactTitle = row["ContactTitle"],
                            Address = row["Address"],
                            City = row["City"],
                            Region = row["Region"],
                            PostalCode = row["PostalCode"],
                            Country = row["Country"],
                            Phone = row["Phone"],
                            Fax = row["Fax"],
                            HomePage = row["HomePage"],
                        };
                        res = x;
                    }
                }
            }
            catch (Exception ex)
            {
                res = null;
            }
            return res;
        }
        public SingleRsp UpdateSupplier_Linq(Suppliers obj)
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
                        var t = context.Suppliers.Update(obj);
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
        #endregion
    }
}
