using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTCSDL.BLL
{
    public class SuppliersSvc : GenericSvc<SuppliersRep, Suppliers>
    {
        public object AddSupplier(SupplierReq req)
        {
            return _rep.AddSupplier(req);
        }
        public object UpdateSupplier(SupplierReq req)
        {
            return _rep.UpdateSupplier(req);
        }
    }
}
