import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {StockIndexComponent} from './pages/stock-index/stock-index.component';
import {LoggedInCanActivate} from '../core/auth/services/logged-in-can-activate.service';

const routes: Routes = [
  {
    path: 'stock',
    component: StockIndexComponent,
    canActivate: [ LoggedInCanActivate ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockRoutingModule { }
