import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {TransactionType} from '../../../../common/models/transaction-type';
import {TransactionService} from '../../../../common/services/transaction.service';
import {TransactionDetails} from '../../models/transaction-details';
import {ExchangeRates} from '../../../../../core/exchangeRates/models/exchange-rates';
import {CurrencyPipe} from '@angular/common';
import { faBuilding, faSpinner } from '@fortawesome/free-solid-svg-icons';
import { SelectedCurrency } from 'app/core/exchangeRates/components/currency-selector/currency-selector.component';

enum LoadState {
  None,
  Busy,
  Failed,
  Success
}

@Component({
  selector: 'app-transaction-view',
  templateUrl: './transaction-view.component.html',
  styles: [
    'hr { margin-top: 0!important; margin-bottom: 0!important; }',
    '.invalid-feedback { display: block!important; }'
  ]
})

export class TransactionViewComponent implements OnInit {

  @Input() set transactionId(value: number) { this._transactionId = value; this.loadTransaction(); }
  get transactionId() : number { return this._transactionId; }
  private _transactionId: number;

  @Output() closeRequested = new EventEmitter();

  get tt() { return TransactionType; }
  get ls() { return LoadState; }

  fa = {
    spinner: faSpinner,
    building: faBuilding
  };

  referenceCurrencyExchangeRate: number = null;
  referenceCurrencyCode: string = null;

  loadState : LoadState;
  model: TransactionDetails;

  get transactionName()
  {
    if(!this.model)
      return '';

    return this.model.transactionType == TransactionType.Sale ? 'Sales' : 'Procurement';
  }

  constructor(
    private service: TransactionService,
    private currencyPipe: CurrencyPipe) {}

  ngOnInit() {
  }

  loadTransaction()
  {
    this.loadState = LoadState.Busy;
    this.service.get(this._transactionId).subscribe(
      res => {
        this.loadState = LoadState.Success;
        this.model = res;
      },
      _err => {
        this.loadState = LoadState.Failed;
      }
    );
  }

  setReferenceCurrency(currency: SelectedCurrency) {
    this.referenceCurrencyCode = currency.code;
    this.referenceCurrencyExchangeRate = currency.rate;
  }

  getPriceDisplayText(price: number, currencyCode: string) : string {

    let result = this.currencyPipe.transform(price, currencyCode, 'symbol', '1.0-1','en-US');

    if (this.referenceCurrencyExchangeRate)
      result += ` (${this.currencyPipe.transform(this.referenceCurrencyExchangeRate * price, this.referenceCurrencyCode, 'symbol-narrow', '1.0-1', 'en-US')})`;

    return result;
  }

  getHeaderText(originalHeader: string) : string {
    if (this.referenceCurrencyCode)
      return originalHeader + ` (${this.referenceCurrencyCode})`;
    else
      return originalHeader;
  }
}
