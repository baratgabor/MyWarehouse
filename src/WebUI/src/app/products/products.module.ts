import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductsRoutingModule } from './products-routing.module';
import { ProductCardComponent } from './components/product-card/product-card.component';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {CoreModule} from '../core/core.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import {ProductIndexComponent} from './pages/product-index/product-index.component';
import {ProductFormComponent} from './components/product-form/product-form.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { ProductEditComponent } from './components/product-edit/product-edit.component';
import { ProductNewComponent } from './components/product-new/product-new.component';
import { ProductEditModalComponent } from './components/product-edit-modal/product-edit-modal.component';
import { ProductLookupFieldComponent } from './components/product-lookup-field/product-lookup-field.component';

@NgModule({
  declarations: [
    ProductCardComponent,
    ProductIndexComponent,
    ProductFormComponent,
    ProductEditComponent,
    ProductNewComponent,
    ProductEditModalComponent,
    ProductLookupFieldComponent
  ],
  exports: [
    ProductCardComponent,
    ProductIndexComponent,
    ProductFormComponent,
    ProductEditComponent,
    ProductNewComponent,
    ProductLookupFieldComponent
  ],
  imports: [
    CommonModule,
    ProductsRoutingModule,
    FontAwesomeModule,
    CoreModule,
    NgbModule,
    ReactiveFormsModule,
    FormsModule
  ],
  entryComponents: [
    ProductEditComponent,
    ProductNewComponent,
    ProductEditModalComponent
  ]
})
export class ProductsModule { }
