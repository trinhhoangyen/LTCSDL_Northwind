import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-doanh-thu',
  templateUrl: './doanh-thu.component.html'
})
export class DoanhThuComponent {
  lstDoanhThu: any = [];
  ngayTraCuu: Date;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    
  }

  TraCuuDTNVTheoNgay(){
    let date= {
      BeginTime: this.ngayTraCuu,
      EndTime: this.ngayTraCuu,
      page: 1,
      size: 5
    }
    this.http.post('https://localhost:44380/' + 'api/Employees/get-dtnv-trong-ngay', date).subscribe(result => {
      var res  :any = result;
      this.lstDoanhThu = res.data;
      console.log(this.lstDoanhThu);
    }, error => console.error(error));
  }
}