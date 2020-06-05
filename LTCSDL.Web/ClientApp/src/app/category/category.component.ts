import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent{
  public res: any;
  // list Category để hiển thị lên ở .html
  public lstCategory: [];
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.post('https://localhost:44380/' + 'api/Categories/get-all', null).subscribe(result => {
      this.res = result;
      // lấy list từ PT get-all và gán (angular k convert kịp data nên k được lấy data từ result)
      this.lstCategory = this.res.data;
      console.log(this.res);
    }, error => console.error(error));
  }
}
