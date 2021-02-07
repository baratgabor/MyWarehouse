import { TransactionLineCreate } from "../models/transaction-create";

const nameof = <T>(name: keyof T) => name;

// Common place for control names used in multiple places (e.g. in form and in validator).
export class TransactionFormConstants {
    public static Line_ProductId = nameof<TransactionLineCreate>("productId");
    public static Line_ProductQuantity = nameof<TransactionLineCreate>("productQuantity");

    // Reference only. No payload equivalent.
    public static Line_ProductName = 'productName';
    public static Line_ProductStock = 'productStock';
    public static Line_ProductUnitPrice = 'productPrice';
    public static Line_ProductUnitPriceCurrency = 'productPriceCurrency';
}