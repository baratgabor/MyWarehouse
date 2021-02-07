import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockRoutingModule } from './stock-routing.module';
import { StockSummaryComponent } from './components/stock-summary/stock-summary.component';
import { ProductInsightsComponent } from '../products/components/product-insights/product-insights.component';
import { StockIndexComponent } from './pages/stock-index/stock-index.component';
import { CoreModule } from '../core/core.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ProductsModule } from '../products/products.module';
import { StockMetricsComponent } from './components/stock-metrics/stock-metrics.component';

@NgModule({
  declarations: [
    StockSummaryComponent,
    ProductInsightsComponent,
    StockIndexComponent,
    StockMetricsComponent
  ],
  exports: [
    StockIndexComponent
  ],
  imports: [
    CommonModule,
    StockRoutingModule,
    CoreModule,
    FontAwesomeModule,
    ProductsModule
  ]
})

export class StockModule { }
