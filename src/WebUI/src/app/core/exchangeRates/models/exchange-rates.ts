export interface ExchangeRates {
  date: string;
  base: string;

  rates: {[key: string]: number};
}
