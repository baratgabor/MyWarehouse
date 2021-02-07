import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Product} from '../../models/product';
import {ProductService} from '../../services/product.service';
import {ConfirmationDialogService} from '../../../core/notifications/services/confirmation-dialog/confirmation-dialog.service';
import {IndividualConfig, ToastrService} from 'ngx-toastr';
import { faSpinner } from '@fortawesome/free-solid-svg-icons';

enum LoadState {
  Loading,
  Success,
  Error
}

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html'
})

export class ProductEditComponent implements OnInit {

  @Input() showInCard = true;

  private _productId: number;
  @Input()
  set productId(id: number) {
    this._productId = id;
    if (id > 0)
      this.loadProduct();
  }
  get productId() {
    return this._productId;
  }

  @Output() closeRequested = new EventEmitter(); //TODO
  @Output() productUpdated = new EventEmitter<Product>();
  @Output() productDeleted = new EventEmitter<number>();

  faSpinner = faSpinner;

  product: Product;

  loadState = LoadState.Loading;
  get state() { return LoadState; }

  constructor(private ps: ProductService,
              private confirmDialog: ConfirmationDialogService) { }

  ngOnInit() {
  }

  loadProduct() {
    this.loadState = LoadState.Loading;
    this.ps.getProduct(this.productId).subscribe(
      res => { this.product = res; this.loadState = LoadState.Success; },
      _err => this.loadState = LoadState.Error);
  }

  onSaveChanges(product: Product) {
    this.ps.updateProduct(this.productId, product).subscribe(
      _res => this.productUpdated.emit(product)
    );
  }

  onDeleteProduct(product: Product) {
    this.confirmDialog.confirm(
      'Confirm deletion',
      'Are you sure you delete this product?',
      'Delete',
      'Cancel').then(confirmed => {
      if (confirmed)
        this.deleteProduct()});
  }

  deleteProduct() {
    this.ps.deleteProduct(this.productId).subscribe(
      _res => this.productDeleted.emit(this.productId)
    );
  }
}
