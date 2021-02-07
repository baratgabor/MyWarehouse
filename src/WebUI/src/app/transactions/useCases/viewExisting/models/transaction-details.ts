import {TransactionType} from '../../../common/models/transaction-type';

export interface TransactionDetails {
  id: number;

  createdAt: Date;
  createdBy: string;

  modifiedAt: Date;
  modifiedBy: string;

  transactionType: TransactionType;

  partnerId: number;
  partnerName: string;
  partnerAddress: string;

  totalAmount: number;
  totalCurrencyCode: string;

  transactionLines: TransactionLine[];
}

export interface TransactionLine {
  productId: number;
  productName: string;
  quantity: number;

  unitPrice: string;
  unitPriceAmount: number;
  unitPriceCurrencyCode: string;
}