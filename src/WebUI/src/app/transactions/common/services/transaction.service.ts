import { Injectable } from '@angular/core';
import {HttpParams} from '@angular/common/http';
import {TransactionDetails} from '../../useCases/viewExisting/models/transaction-details';
import {TransactionCreate} from '../../useCases/createNew/models/transaction-create';
import {Observable} from 'rxjs';
import {PagedState} from 'app/core/http/models/paged-state';
import {TransactionType} from '../models/transaction-type';
import {TransactionListing} from '../../useCases/listAll/models/transaction-listing';
import { ApiService } from 'app/core/http/services/api-service';

@Injectable({
  providedIn: 'root'
})

export class TransactionService {

  apiRouteName: string = 'transactions';

  constructor(private apiService: ApiService) {}
  
  getAll(params: HttpParams, filterTransactionType: TransactionType = null): Observable<PagedState<TransactionListing>>
  {
    if (filterTransactionType != null)
      params = params.append('type', filterTransactionType.toString());

    return this.apiService.get<PagedState<TransactionListing>>(this.apiRouteName, params);
  }

  get(id: number): Observable<TransactionDetails> {
    return this.apiService.get<TransactionDetails>(`${this.apiRouteName}/${id}`);
  }

  create(transaction: TransactionCreate): Observable<number> {
    return this.apiService.post<TransactionCreate, number>(this.apiRouteName, transaction, null, {
      successMessage: 'Transaction created.',
      failMessage: 'Transaction creation failed.'
    });
  }
}
