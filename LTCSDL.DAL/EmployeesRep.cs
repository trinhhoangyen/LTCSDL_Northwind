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
        #region -- Overrides --
        /// <summary>
        /// Read single object
        /// </summary>
        /// <param name="id">Primary key</param>
        /// <returns>Return the object</returns>
        public override Employees Read(int id)
        {
            var res = All.FirstOrDefault(p => p.EmployeeId == id);
            return res;
        }

        /// <summary>
        /// Remove and not restore
        /// </summary>
        /// <param name="id">Primary key</param>
        /// <returns>Number of affect</returns>
        public int Remove(int id)
        {
            var m = base.All.First(i => i.EmployeeId == id);
            m = base.Delete(m); //TODO
            return m.EmployeeId;
        }
        #endregion

        #region -- Methods --
        // đề 1
        /// <summary>
        /// 1a: doanh thu nhân viên theo ngày
        /// </summary>
        /// <param name="date"></param>
        /// <returns>danh sách nhân viên và doanh thu tương ứng trong ngày đó</returns>
        public object DoanhThuNVTheoNgay(DateTime date)
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
                cmd.CommandText = "nv_DoanhThuNVTheoNgay";
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
                            Name = row["FirstName"].ToString() + " " + row["LastName"].ToString(),
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
        //syntax
        public object DoanhThuNVTheoNgay_Linq(DateTime date)
        {
            var data = from e in Context.Employees
                       join o in Context.Orders on e.EmployeeId equals o.EmployeeId
                       join od in Context.OrderDetails on o.OrderId equals od.OrderId
                       where o.OrderDate == date
                       select new
                       {
                           e.EmployeeId,
                           Name = e.FirstName + " " + e.LastName,
                           DoanhThu = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)
                       };
            var res = from d in data
                      group d by new { d.EmployeeId, d.Name } into g
                      select new
                      {
                          g.Key.EmployeeId,
                          g.Key.Name,
                          DoanhThu = Math.Round(float.Parse(g.Sum(x => x.DoanhThu).ToString()))
                      };
            return res;
        }
        //method
        public object DoanhThuNVTheoNgay_Linq1(DateTime date)
        {
            var data = Context.Employees
                    .Join(Context.Orders, e => e.EmployeeId, o => o.EmployeeId, (e, o) => new
                    {
                        e.EmployeeId,
                        Name = e.FirstName + " " + e.LastName,
                        o.OrderDate,
                        o.OrderId
                    }).Where(x => x.OrderDate.Value.Date == date.Date)
                    .Join(Context.OrderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new
                    {
                        o.EmployeeId,
                        o.Name,
                        DoanhThu = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)
                    });
            var res = data.GroupBy(x=>new { x.EmployeeId, x.Name })
                    .Select(x => new 
                    {
                        x.Key.EmployeeId,
                        x.Key.Name,
                        DoanhThuTrongNgay = Math.Round(float.Parse(x.Sum(x => x.DoanhThu).ToString()))
                    });

            return res;
        }

        /// <summary>
        /// 1b: doanh thu nhân viên theo khoảng thời gian
        /// </summary>
        /// <param name="date"></param>
        /// <returns>danh sách nhân viên và doanh thu tương ứng trong khoảng thời gian đó</returns>
        public object DoanhThuNVTheoThoiGian(TimeReq req)
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
                cmd.CommandText = "nv_DoanhThuNVTheoThoiGian";
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
                            Name = row["FirstName"].ToString() + " " + row["LastName"].ToString(),
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
        // method
        public object DoanhThuNVTheoThoiGian_Linq(TimeReq req)
        {
            var data = from e in Context.Employees
                       join o in Context.Orders on e.EmployeeId equals o.EmployeeId
                       join od in Context.OrderDetails on o.OrderId equals od.OrderId
                       where o.OrderDate >= req.DateFrom && o.OrderDate <= req.DateTo
                       select new
                       {
                           e.EmployeeId,
                           Name = e.FirstName + " " + e.LastName,
                           DoanhThu = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)
                       };
            var res = from d in data
                      group d by new { d.EmployeeId, d.Name } into g
                      select new
                      {
                          g.Key.EmployeeId,
                          g.Key.Name,
                          DoanhThu = Math.Round(float.Parse(g.Sum(x => x.DoanhThu).ToString()))
                      };
            return res;
        }
        #endregion
    }
}