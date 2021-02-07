import { TransactionType } from '../../../common/models/transaction-type';

export interface TransactionListing {
  id: number;
  createdAt: Date;
  transactionType: TransactionType;
  partnerName: string;
  totalAmount: number;
  totalCurrencyCode: string;
}
