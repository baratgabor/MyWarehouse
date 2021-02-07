import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnersRoutingModule } from './partners-routing.module';
import {PartnerIndexComponent} from './pages/partner-index/partner-index.component';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import { PartnerNewComponent } from './components/partner-new/partner-new.component';
import { PartnerEditComponent } from './components/partner-edit/partner-edit.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { PartnerFormComponent } from './components/partner-form/partner-form.component';
import { PartnerLookupFieldComponent } from './components/partner-lookup-field/partner-lookup-field.component';
import {CoreModule} from '../core/core.module';


@NgModule({
  declarations: [
    PartnerIndexComponent,
    PartnerNewComponent,
    PartnerEditComponent,
    PartnerFormComponent,
    PartnerLookupFieldComponent,
    PartnerLookupFieldComponent
  ],
  imports: [
    CommonModule,
    PartnersRoutingModule,
    FontAwesomeModule,
    NgbModule,
    ReactiveFormsModule,
    FormsModule,
    CoreModule
  ],
  exports: [
    PartnerIndexComponent,
    PartnerLookupFieldComponent
  ]
})
export class PartnersModule { }
