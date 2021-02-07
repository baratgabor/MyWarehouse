import { NgModule } from '@angular/core';
import {CommonModule, CurrencyPipe} from '@angular/common';

import { TransactionsRoutingModule } from './transactions-routing.module';
import { TransactionIndexComponent } from './useCases/listAll/pages/transaction-index/transaction-index.component';
import { TransactionViewComponent } from './useCases/viewExisting/components/transaction-view/transaction-view.component';
import { TransactionNewComponent } from './useCases/createNew/components/transaction-new/transaction-new.component';
import { FontAwesomeModule, FaIconLibrary } from '@fortawesome/angular-fontawesome';
import {CoreModule} from '../core/core.module';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {PartnersModule} from '../partners/partners.module';
import {ProductsModule} from '../products/products.module';

@NgModule({
  declarations: [
    TransactionIndexComponent,
    TransactionViewComponent,
    TransactionNewComponent
  ],
  imports: [
    CommonModule,
    TransactionsRoutingModule,
    FontAwesomeModule,
    CoreModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    PartnersModule,
    ProductsModule
  ],
  exports: [
    TransactionIndexComponent
  ],
  providers: [
    CurrencyPipe
  ]
})
export class TransactionsModule { }
