using LTCSDL.Common.BLL;
using LTCSDL.Common.Req;
using LTCSDL.Common.Rsp;
using LTCSDL.DAL;
using LTCSDL.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTCSDL.BLL
{
    public class ProductsSvc : GenericSvc<ProductsRep, Products>
    {
        #region --Override --
        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();

            var m = _rep.Read(id);
            res.Data = m;

            return res;
        }
        #endregion

        public object SearchProduct(string kw, int page, int size)
        {
            var pro = All.Where(x => x.ProductName.Contains(kw));

            var offset = (page - 1) * size;
            var total = pro.Count();
            int totalPages = (total % size) == 0 ? (total / size) : (int)(total / size + 1);
            var data = pro.OrderBy(x => x.ProductName).Skip(offset).Take(size).ToList();

            var res = new
            {
                Data = data,
                TotalRecord = total,
                TotalPages = totalPages,
                Page = page,
                Size = size
            };
            return res;
        }

        public SingleRsp CreateProduct(ProductsReq pro)
        {
            var res = new SingleRsp();
            Products products = new Products();

            products.ProductId = pro.ProductId;
            products.ProductName = pro.ProductName;
            products.SupplierId = pro.SupplierId;
            products.CategoryId = pro.CategoryId;
            products.QuantityPerUnit = pro.QuantityPerUnit;
            products.UnitPrice = pro.UnitPrice;
            products.UnitsInStock = pro.UnitsInStock;
            products.UnitsOnOrder = pro.UnitsOnOrder;
            products.ReorderLevel = pro.ReorderLevel;
            products.Discontinued = pro.Discontinued;

            res = _rep.CreateProduct(products);
            res.Data = products;
            return res;
        }

        public SingleRsp UpdateProduct(ProductsReq pro)
        {
            var res = new SingleRsp();
            Products products = new Products();

            products.ProductId = pro.ProductId;
            products.ProductName = pro.ProductName;
            products.SupplierId = pro.SupplierId;
            products.CategoryId = pro.CategoryId;
            products.QuantityPerUnit = pro.QuantityPerUnit;
            products.UnitPrice = pro.UnitPrice;
            products.UnitsInStock = pro.UnitsInStock;
            products.UnitsOnOrder = pro.UnitsOnOrder;
            products.ReorderLevel = pro.ReorderLevel;
            products.Discontinued = pro.Discontinued;

            res = _rep.UpdateProduct(products);
            res.Data = products;
            return res;
        }

        public object GetProductById( int id)
        {
            var pro = All.FirstOrDefault(p => p.ProductId == id);
            var sup = _rep.Context.Suppliers.FirstOrDefault(s => s.SupplierId == pro.SupplierId);
            var cate = _rep.Context.Categories.FirstOrDefault(s => s.CategoryId == pro.CategoryId);
            var product = new
            {
                pro.ProductId,
                pro.ProductName,
                SupplierName = sup.CompanyName,
                cate.CategoryName,
                pro.QuantityPerUnit,
                pro.UnitPrice,
                pro.UnitsInStock,
                pro.UnitsOnOrder,
                pro.ReorderLevel,
                pro.Discontinued
            };
            return product;
        }

        public List<object> ProductNotOrder(GetProductReq req)
        {
            return _rep.ProductNotOrder(req);
        }

        public object QuantityProducts(TimeReq req)
        {
            return _rep.QuantityProducts(req);
        }
    }
}
