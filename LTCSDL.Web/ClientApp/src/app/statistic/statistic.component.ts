import { Component, OnInit, Inject , ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
declare var $: any;
declare var google: any;
@Component({
  selector: 'app-statistic',
  templateUrl: './statistic.component.html',
  styleUrls: ['./statistic.component.css']
})
export class StatisticComponent{
  @ViewChild('barChart', { static: true }) barChart: ElementRef;
  dateFrom: any
  dateTo: any
  dateReq: any
  list: any
  isTable: boolean = false
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }
  onShowTable() {
    this.isTable = true;
    this.dateReq = {
      dateFrom: this.dateFrom,
      dateTo : this.dateTo
    }
    this.http.post('https://localhost:44380/' + 'api/Products/quantity-products-in-order', this.dateReq).subscribe(result => {
      var res: any = result;
      this.list = res
    }, error => {
      alert(error);
    });
  }
  onShowChart = () => {
    this.isTable = false;
    this.dateReq = {
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
    }
    this.http.post('https://localhost:44380/' + 'api/Products/quantity-products-in-order', this.dateReq).subscribe(result => {
      var res: any = result;
      var chartData = this.convert(res);
      console.log(chartData)


      var data = google.visualization.arrayToDataTable(chartData);
      var view = new google.visualization.DataView(data);
      view.setColumns([0, 1,
        {
          calc: "stringify",
          sourceColumn: 1,
          type: "string",
          role: "annotation"
        },
        2]);
      var options = {
        title: "Sản phẩm cần giao từ " + this.dateFrom + "/" + this.dateTo,
        width: 600,
        height: 400,
        bar: { groupWidth: "95%" },
        legend: { position: "none" },
      };
      var chart = new google.visualization.BarChart(document.getElementById("barchart_values"));
      chart.draw(view, options);
    });
  }
  convert(lst) {
    let list: any[] = ["Ngày", "Số lượng", { role: 'style' }]
    let res: any[] = []
    res.push(list)
    lst.forEach(element => {
      res.push([element.orderDate,element.soLuong,'pink'])
    });
    return res;
  }
}