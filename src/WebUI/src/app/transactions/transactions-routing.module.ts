import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {TransactionIndexComponent} from './useCases/listAll/pages/transaction-index/transaction-index.component';
import {LoggedInCanActivate} from '../core/auth/services/logged-in-can-activate.service';

const routes: Routes = [
  {
    path: 'transactions',
    component: TransactionIndexComponent,
    canActivate: [ LoggedInCanActivate ],
    children: [
      {
        path: 'view/:id',
        component: TransactionIndexComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TransactionsRoutingModule { }
