import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-doanh-thu',
  templateUrl: './doanh-thu.component.html'
})
export class DoanhThuComponent {
  tableLeft: any = [];
  tableRight: any = [];
  Date: Date;
  DateFrom: Date;
  DateTo: Date;
  isLeft: boolean = false;
  isRight: boolean =false;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {}

  TraCuuDTNVTheoNgay(){
    this.isLeft = true;
    let date= {
      DateFrom: this.Date,
      DateTo: this.DateTo
    }
    this.http.post('https://localhost:44380/' + 'api/Employees/doanh-thu-nv-theo-ngay', date).subscribe(result => {
      var res : any = result;
      this.tableLeft = res.data;
    }, error => console.error(error));
  }
  TraCuuDTNVTheoThoiGian(){
    this.isRight = true;
    let date= {
      DateFrom: this.DateFrom,
      DateTo: this.DateTo
    }
    this.http.post('https://localhost:44380/' + 'api/Employees/doanh-thu-nv-theo-thoi-gian', date).subscribe(result => {
      var res : any = result;
      this.tableRight = res.data;
    }, error => console.error(error));
  }
}