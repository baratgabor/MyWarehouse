import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {ProductIndexComponent} from './pages/product-index/product-index.component';
import {LoggedInCanActivate} from '../core/auth/services/logged-in-can-activate.service';

const routes: Routes = [
  {
    path: 'products',
    component: ProductIndexComponent,
    canActivate: [ LoggedInCanActivate ],
    children: [
      {
        path: 'stocked',
        component: ProductIndexComponent,
      },
      {
        path: 'edit/:id',
        component: ProductIndexComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
