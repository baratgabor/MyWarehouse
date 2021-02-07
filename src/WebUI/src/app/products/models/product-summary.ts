export interface ProductSummary {
  id: number;
  name: string;
  description: string;

  priceAmount: number;
  priceCurrencyCode: string;

  massValue: number;
  massUnitSymbol: string;

  numberInStock: number;
}
