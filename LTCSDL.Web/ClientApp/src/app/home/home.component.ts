import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
declare var $: any;
declare var google: any;
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  month: any
  year: any
  orderReq: any
  lstDoanhThu: any
  isTable: boolean = false
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }

  onShowTable() {
    this.isTable = true;
    this.orderReq = {
      month: this.month,
      year: this.year,
    }
    this.http.post('https://localhost:44380/' + 'api/Orders/doanh-thu-theo-quoc-gia', this.orderReq).subscribe(result => {
      var res: any = result;
      this.lstDoanhThu = res.data
    }, error => {
      alert(error);
    });
  }

  onShowChart() {
    this.isTable = false;

    this.orderReq = {
      month: this.month,
      year: this.year,
    }
    this.http.post('https://localhost:44380/' + 'api/Orders/doanh-thu-theo-quoc-gia', this.orderReq).subscribe(result => {
      var res: any = result;
      // this.lstDoanhThu = res.data;
      var chartData = this.convert(res.data);
      console.log(chartData);
      var data = google.visualization.arrayToDataTable(chartData);
  
      var options = {
        legend: 'none',
        pieSliceText: 'label',
        title: 'Swiss Language Use (100 degree rotation)',
        pieStartAngle: 100,
      };
  
      var chart = new google.visualization.PieChart(document.getElementById('piechart'));
      chart.draw(data, options);
    }, error => {
      alert(error);
    });
  }

  convert(lst){
    var res = [['Country', 'DoanhThu']];
    lst.forEach(element => {
      var item = [];
      item.push(element.country);
      item.push(element.doanhThu);
      res.push(item);
    });

    return res;
  }
}

