import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AbstractControl, FormArray, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {TransactionType} from '../../../../common/models/transaction-type';
import {TransactionService} from '../../../../common/services/transaction.service';
import {ConfirmationDialogService} from '../../../../../core/notifications/services/confirmation-dialog/confirmation-dialog.service';
import {TransactionDetails} from '../../../viewExisting/models/transaction-details';
import {CurrencyPipe} from '@angular/common';
import { faBuilding, faToggleOff, faToggleOn, faTrashAlt, faSpinner, faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';
import { SelectedPartner } from 'app/transactions/useCases/createNew/models/selected-partner';
import { SelectedProduct } from 'app/transactions/useCases/createNew/models/selected-product';
import { SelectedCurrency } from 'app/core/exchangeRates/components/currency-selector/currency-selector.component';
import { TransactionCreate } from 'app/transactions/useCases/createNew/models/transaction-create';
import { transactionLinesValidator } from 'app/transactions/useCases/createNew/validators/transaction-lines-validator';
import { TransactionFormConstants as Constants } from '../../constants/transactionFormConstants';
import { HttpErrorResponse } from '@angular/common/http';
import { CustomValidators } from 'app/core/errorhandling/form-validation/validators/custom-validators';

enum SaveState {
  None,
  Busy,
  Failed,
  Success
}

@Component({
  selector: 'app-transaction-new',
  templateUrl: './transaction-new.component.html',
  styles: [
    'hr { margin-top: 0!important; margin-bottom: 0!important; }',
    '.invalid-feedback { display: block!important; }'
    ]
})

export class TransactionNewComponent implements OnInit {

  constructor(
    private formBuilder: FormBuilder,
    private service: TransactionService,
    private confirmDialog: ConfirmationDialogService,
    private currencyPipe: CurrencyPipe) {}

  fa = {
    building: faBuilding,
    trash: faTrashAlt,
    toggleOn: faToggleOn,
    toggleOff: faToggleOff,
    spinner: faSpinner,
    error : faExclamationTriangle
  };

  selectedPartner: SelectedPartner;
  transactionCurrencyCode: string = null;
  referenceCurrencyRate: number = null;
  referenceCurrencyCode: string = null;

  @Input() transactionType: TransactionType;
  get tt() { return TransactionType; }
  get ss() { return SaveState; }
  get f() { return this.mainForm.controls; }
  get transactionLines() : FormArray { return this.mainForm?.get('transactionLines') as FormArray; }

  // Events emitted
  @Output() transactionCreated = new EventEmitter<TransactionCreate>();
  @Output() formClosed = new EventEmitter<TransactionDetails>();

  mainForm : FormGroup;
  submitAttempted: boolean;
  saveState: SaveState;
  productLookupOnlyStocked: boolean;

  ngOnInit()
  {
    this.productLookupOnlyStocked = this.transactionType == TransactionType.Sale;

    this.mainForm = this.formBuilder.group({
      transactionType: [this.transactionType],
      partnerId: ['', CustomValidators.requiredField('A partner must be selected.')],
      transactionLines: this.formBuilder.array([], transactionLinesValidator(this.transactionType))
    });
  }

  createOrderLine(productId: number, name: string, stock: number, unitPrice: number, currency: string, quantity: number = 1) : FormGroup
  {
    return this.formBuilder.group(
      {
        [Constants.Line_ProductId]: [productId, [Validators.required, Validators.min(1), Validators.pattern('^[0-9]*$')]],
        [Constants.Line_ProductQuantity]: [quantity, [Validators.required, Validators.min(1), Validators.pattern('^[0-9]*$')]],
        
        // Not sent in payload. Used for reference only.
        [Constants.Line_ProductName]: name,
        [Constants.Line_ProductStock]: stock,
        [Constants.Line_ProductUnitPrice]: unitPrice,
        [Constants.Line_ProductUnitPriceCurrency]: currency
      }
    );
  }

  onSubmit()
  {
    this.submitAttempted = true;

    if (!this.validate())
      return;

    this.confirmDialog.confirm(
      'Confirm transaction',
      'Are you sure you submit this transaction?',
      'Yes',
      'Cancel').then(confirmed => {
      if (confirmed)
        this.sendTransaction(this.mainForm.value)});
  }

  cancelSubmit()
  {
    this.clearFailureState();
  }

  retrySubmit()
  {
    this.clearFailureState();
    
    this.submitAttempted = true;
    if (!this.validate())
      return;

    this.sendTransaction(this.mainForm.value);
  }

  addPartner(partner: SelectedPartner)
  {
    this.selectedPartner = partner;
    this.mainForm.get('partnerId').setValue(partner.id);
  }

  removePartner()
  {
    if (this.saveState == SaveState.Busy || this.saveState == SaveState.Failed)
      return;

    this.selectedPartner = null;
    this.mainForm.get('partnerId').setValue(null);
  }

  addLine(product: SelectedProduct)
  {
    if (this.saveState == SaveState.Busy || this.saveState == SaveState.Failed)
      return;

    this.transactionLines.push(this.createOrderLine(product.id, product.name, product.numberInStock, product.priceAmount, product.priceCurrencyCode));
  }

  deleteLine(index: number)
  {
    if (this.saveState == SaveState.Busy || this.saveState == SaveState.Failed)
      return;

    this.transactionLines.removeAt(index);
  }

  private validate(): boolean {
    this.submitAttempted = true;
    return !this.mainForm.invalid;
  }

  getFirstErrorMessage(c: AbstractControl) : string
  {
    if (!c || !c.errors)
      return null;

    return Object.values(c.errors)[0];
  }

  setReferenceCurrency(currency: SelectedCurrency) {
    this.referenceCurrencyCode = currency.code;
    this.referenceCurrencyRate = currency.rate;
  }

  getProductName(lineIndex: number): string {
    return this.transactionLines.controls[lineIndex].get(Constants.Line_ProductName).value;
  }

  getProductStock(lineIndex: number): number {
    return this.transactionLines.controls[lineIndex].get(Constants.Line_ProductStock).value;
  }

  getCurrencyHeaderText(originalHeader: string) : string {
    if (this.referenceCurrencyCode)
      return originalHeader + ` (${this.referenceCurrencyCode})`;
    else
      return originalHeader;
  }

  getFormattedProductPrice(lineIndex: number): string {

    const transactionLine = this.transactionLines.controls[lineIndex];
    const unitPrice = transactionLine.get(Constants.Line_ProductUnitPrice).value;
    const currency = transactionLine.get(Constants.Line_ProductUnitPriceCurrency).value;

    return this.getFormattedPrice(unitPrice, currency);
  }

  getFormattedLineTotal(productIndex: number): string {

    var lineTotal = this.transactionLines.controls[productIndex].get(Constants.Line_ProductUnitPrice).value
      * this.transactionLines.controls[productIndex].get(Constants.Line_ProductQuantity).value;

    return this.getFormattedPrice(
      lineTotal,
      this.transactionLines.controls[productIndex].get(Constants.Line_ProductUnitPriceCurrency).value);
  };

  getFormattedTransactionTotal(): string {

    var total = this.transactionLines.controls
      .reduce((sum, current) => sum +
        (current.get(Constants.Line_ProductUnitPrice).value * current.get(Constants.Line_ProductQuantity).value), 0);

    return this.getFormattedPrice(total, this.transactionCurrencyCode);
  }

  private getFormattedPrice(value: number, currency: string): string {

    let formattedPrice = this.currencyPipe.transform(value, currency, 'symbol', '1.0-2','en-US');

    if (this.referenceCurrencyRate) {
      formattedPrice += ` (${this.currencyPipe.transform(this.referenceCurrencyRate * value, this.referenceCurrencyCode, 'symbol-narrow', '1.0-2', 'en-US')})`;
    }

    return formattedPrice;
  }

  private clearFailureState() {
    if (this.saveState != SaveState.Failed)
      return;

    this.saveState = SaveState.None;
    this.submitAttempted = false;
    this.mainForm.enable();
  }

  private sendTransaction(data: TransactionCreate)
  {
    this.saveState = SaveState.Busy;
    this.mainForm.disable({ emitEvent: false });

    this.service.create(data).subscribe(
      _res => {
        this.saveState = SaveState.Success;
        //TODO: Decide about emitted entity type; reconsile with backend if created/updated entities should be returned.
        this.transactionCreated.emit(data);
      },
      err => {
        if (err instanceof HttpErrorResponse && err.status == 400) {
          //TODO: Implement display of server side model validation feedback.
        }
        else {
          this.saveState = SaveState.Failed;
        }
      }
    )
  }
}
