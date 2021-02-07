import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ExchangeRates} from '../models/exchange-rates';
import {DatePipe} from '@angular/common';

@Injectable({
  providedIn: 'root'
})

export class ExchangeRatesService {
  private readonly rateStaleDays = 2;
  private _exchangeRatesSource: ExchangeRatesSource;

  constructor(private http: HttpClient, private datePipe: DatePipe) {
    this._exchangeRatesSource =
      new ExchangeRatesObjectCachingDecorator(
        new ExchangeRatesLocalStorageCachingDecorator(
          new ExchangeRatesApiAccess(this.http)));
  }

  public async getExchangeRates(): Promise<ExchangeRates> {
    const r = await this._exchangeRatesSource.getExchangeRates();
    if (this.isStale(r.date)) {
      this._exchangeRatesSource.purge();
      return this._exchangeRatesSource.getExchangeRates();
    } else {
      return r;
    }
  }

  isStale = (yyyyMMdd: string): boolean => {
    const daysOld = (new Date(yyyyMMdd).getTime() - Date.now())/(1000*60*60*24.0);
    return daysOld > this.rateStaleDays;    
  };
}

interface ExchangeRatesSource {
  getExchangeRates() : Promise<ExchangeRates>;
  purge();
}

// Provides object cached access to data
class ExchangeRatesObjectCachingDecorator implements ExchangeRatesSource {
  private _exchangeRates: ExchangeRates;

  constructor(private decoratee: ExchangeRatesSource) {}

  async getExchangeRates(): Promise<ExchangeRates> {
    if (this._exchangeRates)
      return Promise.resolve(this._exchangeRates);

    const r = await this.decoratee.getExchangeRates();
    this._exchangeRates = r;
    return r;
  }

  purge() {
    this._exchangeRates = null;
    this.decoratee.purge();
  }
}

// Provides local storage cached access to data
class ExchangeRatesLocalStorageCachingDecorator implements ExchangeRatesSource {
  private localStorageSettingKey = 'exchangeRates';

  constructor(private decoratee: ExchangeRatesSource) {}

  async getExchangeRates(): Promise<ExchangeRates> {
    let ratesString = localStorage.getItem(this.localStorageSettingKey);

    if (ratesString)
      return Promise.resolve(JSON.parse(ratesString) as ExchangeRates);

    // Get data from decoratee, cache it in local storage, return same value
    const r = await this.decoratee.getExchangeRates();
    localStorage.setItem(
      this.localStorageSettingKey,
      JSON.stringify(r));
    return r;
  }

  purge() {
    localStorage.removeItem(this.localStorageSettingKey);
    this.decoratee.purge();
  }
}

// Retrieves data from web service
class ExchangeRatesApiAccess implements ExchangeRatesSource {
  private apiGetUrl = 'https://api.exchangeratesapi.io/latest?base=USD';

  constructor(private http: HttpClient) {}

  async getExchangeRates(): Promise<ExchangeRates> {
    return this.http.get<ExchangeRates>(this.apiGetUrl).toPromise();
  }

  purge() {
  }
}
