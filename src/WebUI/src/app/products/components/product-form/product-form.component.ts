import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { CustomValidators } from 'app/core/errorhandling/form-validation/validators/custom-validators';
import {Product} from '../../models/product';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html'
})

// Reusable transaction form for multiple operations (update/new)
export class ProductFormComponent implements OnInit {

  @Input() showInCard = true;
  @Input() formTitle = 'Product';
  @Input() submitText = 'Submit';
  @Input() secondaryBtnText = '';
  @Input() validateSecondaryBtn = true;
  @Input() stockIsEditable = true;

  // Product model to load into the form
  private _product: Product;
  @Input()
  set product(product: Product) {
    this._product = product;

    if (product) {
      this.productPriceCurrency = product.priceCurrencyCode;
      this.productMassUnit = product.massUnitSymbol;
      this.initForm();
    }
    else
      this.makeEmptyDisabledForm();
  }
  get product() {
    return this._product;
  }

  // Events emitted
  @Output() submitClicked = new EventEmitter<Product>();
  @Output() secondaryBtnClicked = new EventEmitter<Product>();
  @Output() closeBtnClicked = new EventEmitter<Product>();

  productForm: FormGroup;
  submitAttempted: boolean;

  productPriceCurrency: string = "N/A";
  productMassUnit: string = "N/A";

  // Convenience getter for easy access to form fields
  get f() { return this.productForm.controls; }

  constructor(
    private formBuilder: FormBuilder) {}

  ngOnInit() {
    if (!this.stockIsEditable && this.f.numberInStock)
      this.f.numberInStock.disable();
  }

  private initForm() {

    if (this.productForm && this.productForm.disabled)
      this.productForm.enable();

    // https://github.com/angular/angular/issues/22556
    if (!this.stockIsEditable && this.f.numberInStock)
      this.f.numberInStock.disable();

    this.productForm = this.formBuilder.group({
      id:             [this.product.id],
      name:           [this.product.name ?? '', [Validators.required, Validators.maxLength(100)]],
      description:    [this.product.description ?? '', Validators.maxLength(200)],
      priceAmount:    [this.product.priceAmount, CustomValidators.requiredMinNumber(0.01)],
      massValue:      [this.product.massValue, CustomValidators.requiredMinNumber(0.01)],
      numberInStock:  [this.product.numberInStock, [CustomValidators.requiredWholeNumber, CustomValidators.requiredMinNumber(0)]]
    });
  }

  private makeEmptyDisabledForm() {
    this.productForm = this.formBuilder.group({
      id:             [''],
      name:           ['', [Validators.required, Validators.maxLength(100)]],
      description:    ['', Validators.maxLength(200)],
      priceAmount:    ['', [Validators.required, Validators.min(0.01)]],
      massValue:      ['', [Validators.required, Validators.min(0.01)]],
      numberInStock:  ['']
    });
    this.productForm.disable();
  }

  onSubmit() {
    if (!this.validate())
      return;

    Object.assign(this.product, this.productForm.value);
    this.submitClicked.emit(this.product);
  }

  onSecondary() {
    if (this.validateSecondaryBtn && !this.validate())
      return;

    Object.assign(this.product, this.productForm.value);
    this.secondaryBtnClicked.emit(this.product);
  }

  private validate(): boolean {
    this.submitAttempted = true;
    return !this.productForm.invalid;
  }
}
