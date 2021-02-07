import {Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import {ExchangeRates} from '../../models/exchange-rates';
import {ExchangeRatesService} from '../../services/exchange-rates.service';

@Component({
  selector: 'app-currency-selector',
  templateUrl: './currency-selector.component.html',
  styles: ['.dropdown-menu { max-height: 20rem!important; overflow-y: auto!important; }']
})
export class CurrencySelectorComponent implements OnInit {

  @Output() currentSelectedCurrency: string;
  @Output() currencySelected = new EventEmitter<SelectedCurrency>();

  availableCurrencies: string[];
  currencyDataFreshness: string;

  private rates: {[key: string]: number};
  private readonly currencySelectionStorageKey = 'foreignCurrency';
  private static instances: CurrencySelectorComponent[] = [];

  constructor(private ratesService: ExchangeRatesService) {
    CurrencySelectorComponent.instances.push(this);
  }

  setCurrency(currency: string, cascade = true) {
    this.currentSelectedCurrency = currency;
    this.currencySelected.emit({
      code: currency,
      rate: currency ? this.rates[currency] : null
    });

    localStorage.setItem(this.currencySelectionStorageKey, currency ? currency : '');

    // Distribute new setting to other potential instances.
    if (cascade) {
      CurrencySelectorComponent.instances.filter(i => i !== this)
        .forEach(i => i.setCurrency(currency, false));
    }
  }

  ngOnInit(): void {

    this.ratesService.getExchangeRates().then((res) => {
      this.applyCurrencyData(res);
      
      let storedCurrency = localStorage.getItem(this.currencySelectionStorageKey);
      if (storedCurrency && this.rates)
      {
        this.currentSelectedCurrency = storedCurrency;
        this.currencySelected.emit({
          code: storedCurrency,
          rate: this.rates[storedCurrency]
        });
      }
    });
  }

  applyCurrencyData(data: ExchangeRates) {
    this.availableCurrencies = Object.keys(data.rates);
    this.currencyDataFreshness = data.date;
    this.rates = data.rates;
  }
}

export interface SelectedCurrency {
  code: string;
  rate: number;
}
