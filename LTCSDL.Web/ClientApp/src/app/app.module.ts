import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { CategoryComponent } from './category/category.component';
import { ProductComponent } from './product/product.component';
import { DoanhThuComponent } from './doanh-thu/doanh-thu.component';
import { OrderComponent } from './order/order.component';
import { ShipperComponent } from './shipper/shipper.component';
import { StatisticComponent } from './statistic/statistic.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    CategoryComponent,
    ProductComponent,
    DoanhThuComponent,
    OrderComponent,
    ShipperComponent,
    StatisticComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'category', component: CategoryComponent },
      { path: 'product', component: ProductComponent },
      { path: 'doanh-thu', component: DoanhThuComponent },
      { path: 'order', component: OrderComponent },
      { path: 'shipper', component: ShipperComponent },
      { path: 'statistic', component: StatisticComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
