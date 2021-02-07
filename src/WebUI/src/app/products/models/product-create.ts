export interface ProductCreate {
  name: string;
  description: string;

  priceAmount: number;
  priceCurrencyCode: string;

  massValue: number;
  massUnitSymbol: string;
}
