import { Component, OnInit, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
declare var $: any;
declare var google: any;
@Component({
  selector: 'app-shipper',
  templateUrl: './shipper.component.html',
  styleUrls: ['./shipper.component.css']
})
export class ShipperComponent {
  @ViewChild('barChart', { static: true }) barChart: ElementRef;
  month: any
  year: any
  dateReq: any
  list: any
  isTable: boolean = false
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }
  onShowTable() {
    this.isTable = true;
    this.dateReq = {
      month: this.month,
      year: this.year,
    }
    this.http.post('https://localhost:44380/' + 'api/Shippers/doanh-thu-shipper-theo-thang-nam', this.dateReq).subscribe(result => {
      var res: any = result;
      console.log(result)
      this.list = res.data
    }, error => {
      alert(error);
    });
  }
  onShowChart = () => {
    this.isTable = false;
    this.dateReq = {
      month: this.month,
      year: this.year,
    }
    this.http.post('https://localhost:44380/' + 'api/Shippers/doanh-thu-shipper-theo-thang-nam', this.dateReq).subscribe(result => {
      var res: any = result;
      var chartData = this.convert(res.data);
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
        title: "Thống kê doanh thu shipper theo " + this.month + "/" + this.year,
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
    var res = [["Shipper", "Doanh Thu", { role: 'style' }]]
    lst.forEach(element => {
      var item = [];
      item.push(element.shipperId.toString());
      item.push(parseFloat(parseFloat(element.doanhThu.toString()).toFixed(2)));
      item.push("pink");
      res.push(item);
    });
    return res;
  }
}
