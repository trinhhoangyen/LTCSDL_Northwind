using LTCSDL.Common.DAL;
using LTCSDL.Common.Req;
using LTCSDL.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LTCSDL.DAL
{
    public class EmployeesRep : GenericRep<NorthwindContext, Employees>
    {
        public object DoanhThuTheoNgay(DateTime date)
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
                cmd.CommandText = "DoanhThuTheoNgay";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Date", date);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            EmployeeID = row["EmployeeID"],
                            FirstName = row["FirstName"],
                            LastName = row["LastName"],
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
        //public object DoanhThuTheoNgay_Linq(DateTime date)
        //{
        //    var context = new NorthwindContext();
        //    var employee = context.Employees.ToList();
        //    var order = context.Orders.ToList();
        //    var OrderDetail = context.OrderDetails.ToList();
        //    var res = from e in employee
        //              join o in order on e.EmployeeId equals o.EmployeeId
        //              join od in OrderDetail on o.OrderId equals od.OrderId
        //              where o.OrderDate.Value.Day == date.Day
        //                   && o.OrderDate.Value.Month == date.Month
        //                   && o.OrderDate.Value.Year == date.Year
        //              select new
        //              {
        //                  e.EmployeeId,
        //                  e.LastName,
        //                  e.FirstName,
        //                  DoanhThu = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)
        //              };
        //    var ret = from r in res
        //              group r by new
        //              {
        //                  r.EmployeeId,
        //                  r.LastName,
        //                  r.FirstName,
        //                  r.DoanhThu
        //              } into e
        //              select new
        //              {
        //                  e.Key.EmployeeId,
        //                  e.Key.LastName,
        //                  e.Key.FirstName,
        //                  Doanhthu = e.Sum(x => x.DoanhThu).ToString()
        //              };

        //    return ret;
        //}
        public object DoanhThuTheoThoiGian(OrderFullReq req)
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
                cmd.CommandText = "DoanhThuTheoThoiGian";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@begintime", req.DateFrom);
                cmd.Parameters.AddWithValue("@endTime", req.DateTo);
                da.SelectCommand = cmd;
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var x = new
                        {
                            EmployeeID = row["EmployeeID"],
                            FirstName = row["FirstName"],
                            LastName = row["LastName"],
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
    }
}
