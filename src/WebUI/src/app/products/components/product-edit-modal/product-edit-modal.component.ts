import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {ProductEditComponent} from '../product-edit/product-edit.component';
import {Product} from '../../models/product';

//TODO: utter crap, rework this as a service + component, like the confirmation dialog

@Component({
  selector: 'app-product-edit-modal',
  templateUrl: './product-edit-modal.component.html'
})
export class ProductEditModalComponent implements OnInit {

  //@ViewChild('productEdit', {static: true}) productEditor: ProductEditComponent;

  @Input() selectedProductId: number;

  //@Output() partnerUpdated: EventEmitter<Product>;
  //@Output() partnerDeleted: EventEmitter<Product>;

  constructor() { }

  ngOnInit() {
    //this.partnerUpdated = this.productEditor.partnerUpdated;
    //this.partnerDeleted = this.productEditor.partnerDeleted;
  }

}
