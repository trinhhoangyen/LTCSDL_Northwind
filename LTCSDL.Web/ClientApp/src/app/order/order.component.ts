import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent {
  listOrder: any = [];
  dateFrom: Date;
  dateTo: Date;
  orderId: any;
  listOrderDetail: any;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }
  XemDSDH(){
    let date= {
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
      page: 1,
      size: 5,
      keyword: "",
      month: 0,
      year: 0,
      isQuantity: 0
    }
    this.http.post('https://localhost:44380/' + 'api/Orders/get-order-in-space-time', date).subscribe(result => {
      var res  :any = result;
      this.listOrder = res.data;
      console.log(this.listOrder);
    }, error => console.error(error));
  }
  TraCuu(){
    let orderReq= {
      id: this.orderId,
      keyword: ""
    }
    this.http.post('https://localhost:44380/' + 'api/Orders/get-order-detail-by-id', orderReq).subscribe(result => {
      var res : any = result;
      this.listOrderDetail = res.data;
      console.log(this.listOrderDetail);
    }, error => console.error(error));
  }
}
