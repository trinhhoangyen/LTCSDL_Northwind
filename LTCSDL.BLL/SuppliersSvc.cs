using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;

namespace LTCSDL.BLL
{
    public class SuppliersSvc : GenericSvc<SuppliersRep, Suppliers>
    {
        #region -- đề 4 --
        /// <summary>
        /// 4a: Thêm mới 1 nhà cung cấp
        /// </summary>
        /// <param name="sup"></param>
        /// <returns>Nhà cung cấp mới thêm</returns>
        public object AddSupplier(SupplierReq req)
        {
            return _rep.AddSupplier(req);
        }
        public SingleRsp AddSupplier_Linq(SupplierReq req)
        {
            var res = new SingleRsp();
            Suppliers sup = new Suppliers();
            sup.SupplierId = req.SupplierId;
            sup.CompanyName = req.CompanyName;
            sup.ContactName = req.ContactName;
            sup.ContactTitle = req.ContactTitle;
            sup.Address = req.Address;
            sup.City = req.City;
            sup.Region = req.Region;
            sup.PostalCode = req.PostalCode;
            sup.Country = req.Country;
            sup.Phone = req.Phone;
            sup.Fax = req.Fax;
            sup.HomePage = req.HomePage;

            res = _rep.AddSupplier_Linq(sup);
            res.Data = sup;
            return res;
        }

        /// <summary>
        /// 4b: Sửa mới 1 nhà cung cấp
        /// </summary>
        /// <param name="sup"></param>
        /// <returns>Nhà cung cấp mới sửa</returns>
        public object UpdateSupplier(SupplierReq req)
        {
            return _rep.UpdateSupplier(req);
        }
        public SingleRsp UpdateSupplier_Linq(SupplierReq req)
        {
            var res = new SingleRsp();
            Suppliers sup = new Suppliers();
            sup.SupplierId = req.SupplierId;
            sup.CompanyName = req.CompanyName;
            sup.ContactName = req.ContactName;
            sup.ContactTitle = req.ContactTitle;
            sup.Address = req.Address;
            sup.City = req.City;
            sup.Region = req.Region;
            sup.PostalCode = req.PostalCode;
            sup.Country = req.Country;
            sup.Phone = req.Phone;
            sup.Fax = req.Fax;
            sup.HomePage = req.HomePage;

            res = _rep.UpdateSupplier_Linq(sup);
            res.Data = sup;
            return res;
        }
        #endregion
    }
}
