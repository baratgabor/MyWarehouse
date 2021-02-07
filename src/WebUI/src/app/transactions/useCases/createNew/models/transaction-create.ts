import {TransactionType} from '../../../common/models/transaction-type';

export interface TransactionCreate {
  transactionType: TransactionType;
  partnerId: number;

  transactionLines: TransactionLineCreate[];
}

export interface TransactionLineCreate {
  productId: number;
  productQuantity: number;
}
