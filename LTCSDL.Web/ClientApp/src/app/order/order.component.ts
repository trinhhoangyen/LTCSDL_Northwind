import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent {
  list: any = [];
  dateFrom: Date;
  dateTo: Date;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }

  
  TraCuu(){
    let date= {
      BeginTime: this.dateFrom,
      EndTime: this.dateTo,
      page: 1,
      size: 5
    }
    this.http.post('https://localhost:44380/' + 'api/Orders/get-order-in-space-time', date).subscribe(result => {
      var res  :any = result;
      this.list = res.data;
      console.log(this.list);
    }, error => console.error(error));
  }

}
