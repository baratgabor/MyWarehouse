import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {PartnerIndexComponent} from './pages/partner-index/partner-index.component';
import {LoggedInCanActivate} from '../core/auth/services/logged-in-can-activate.service';

const routes: Routes = [
  {
    path: 'partners',
    component: PartnerIndexComponent,
    canActivate: [ LoggedInCanActivate ],
    children: [
      {
        path: 'edit/:id',
        component: PartnerIndexComponent
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnersRoutingModule { }
