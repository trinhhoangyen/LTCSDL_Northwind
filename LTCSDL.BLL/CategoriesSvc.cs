using System;
using System.Collections.Generic;
using System.Text;

using LTCSDL.Common.BLL;
using LTCSDL.Common.Rsp;

namespace LTCSDL.BLL
{
    using DAL;
    using DAL.Models;

    public class CategoriesSvc: GenericSvc<CategoriesRep, Categories>
    {
        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();

            var m = _rep.Read(id);
            res.Data = m;

            return res;
        }

        public override SingleRsp Update(Categories m)
        {
            var res = new SingleRsp();
            var m1 = m.CategoryId > 0 ? _rep.Read(m.CategoryId) : _rep.Read(m.Description);
            if (m1 == null)
                res.SetError("EZ103", "No Data.");
            else
            {
                res = base.Update(m);
                res.Data = m;
            }
            return res;
        }
    }
}
