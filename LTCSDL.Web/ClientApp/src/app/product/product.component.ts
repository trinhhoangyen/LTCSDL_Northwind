import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
declare var $:any; // it dont know jquery
@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent{
  size: number = 10;
  products: any={
    data:[],
    page: 0,
    size: this.size,
    totalPages: 0,
    totalRecord: 0,
  };
  isEdit: boolean = false;

  product ={
    productId: 0,
    productName: "",
    supplierId: "0",
    categoryId: "0",
    quantityPerUnit: "",
    unitPrice: "0",
    unitsInStock: "0",
    unitsOnOrder: "0",
    reorderLevel: "0",
    discontinued: false,
  };

  constructor(
    private http: HttpClient, 
    @Inject('BASE_URL') baseUrl: string) {
        this.searchProduct(1);
}
  searchProduct(cPage)
  {
    let x ={
      page:cPage,
      size: this.size,
      keyword:""
    };
    this.http.post('https://localhost:44380/' + 'api/Products/search-products', x).subscribe(result => {
      var res: any = result;
      if(res.success)
      {
        this.products = res.data;
      }
      else {
        alert(res.message);
      }
    }, error => {
      console.error(error)
      alert(error);
    });
  }

  searchNext()
  {
    if( this.products.page < this.products.totalPages)
    {
      let nextPage = this.products.page + 1;
      let x ={
        page:nextPage,
        size:this.size,
        keyword:""
      };
      this.http.post('https://localhost:44380/' + 'api/Products/search-products', x).subscribe(result => {
        this.products = result;
        this.products = this.products.data;
        console.log(this.products);
      }, error => console.error(error));
    }
    else
    {
      alert("Bạn đang ở trang cuối");
    }
  }
  searchPrevious()
  {
    if( this.products.page > 1)
    {
      let nextPage = this.products.page - 1;
      let x ={
        page:nextPage,
        size:this.size,
        keyword:""
      };
      this.http.post('https://localhost:44380/' + 'api/Products/search-products', x).subscribe(result => {
        this.products = result;
        this.products = this.products.data;
        console.log(this.products);
      }, error => console.error(error));
    }
    else
    {
      alert("Bạn đang ở trang đầu");
    }
  }

  openModal(isEdit, index)
  {
    this.isEdit = isEdit;
    if(isEdit)
    {
      var item = this.products.data[index];
      this.product = {
        productId: item.productId,
        productName: item.productName,
        supplierId: item.supplierId.toString(),
        categoryId: item.categoryId.toString(),
        quantityPerUnit: item.quantityPerUnit,
        unitPrice: item.unitPrice,
        unitsInStock: item.unitsInStock,
        unitsOnOrder: item.unitsOnOrder,
        reorderLevel: item.reorderLevel,
        discontinued: item.discontinued
      };
    }
    $('#modalProduct').modal('show');
  }
  createProduct()
  {
    var x = {
      productId: 0,
      productName: this.product.productName,
      supplierId: parseInt(this.product.supplierId),
      categoryId: parseInt(this.product.categoryId),
      quantityPerUnit: this.product.quantityPerUnit,
      unitPrice: parseFloat(this.product.unitPrice),
      unitsInStock: parseInt(this.product.unitsInStock),
      unitsOnOrder: parseInt(this.product.unitsOnOrder),
      reorderLevel: parseInt(this.product.reorderLevel),
      discontinued: false
    };
    this.http.post('https://localhost:44380/' + 'api/Products/create-product', x).subscribe(result => {
      var res: any = result;
      if(res.success)
      {
        this.products = res.data;
        this.isEdit = true;
        this.searchProduct(1);
        alert("New product has been added! Now you can modified! ");
      }
      else {
        alert(res.message);
      }
    }, error => {
      console.error(error)
      alert(error);
    });
  }

  saveProduct()
  {
    var x = {
      productId: this.product.productId,
      productName: this.product.productName,
      supplierId: parseInt(this.product.supplierId),
      categoryId: parseInt(this.product.categoryId),
      quantityPerUnit: this.product.quantityPerUnit,
      unitPrice: parseFloat(this.product.unitPrice),
      unitsInStock: parseInt(this.product.unitsInStock),
      unitsOnOrder: parseInt(this.product.unitsOnOrder),
      reorderLevel: parseInt(this.product.reorderLevel),
      discontinued: false
    };
    this.http.post('https://localhost:44380/' + 'api/Products/update-product', x).subscribe(result => {
      var res: any = result;
      if(res.success)
      {
        this.products = res.data;
        this.searchProduct(1);
        alert("A product has been updated successfully");
      }
      else {
        alert(res.message);
      }
    }, error => {
      console.error(error)
      alert(error);
    });
  }
}