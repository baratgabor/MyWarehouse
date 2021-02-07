import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { GlobalToastService } from '../notifications/services/global-toast-service';
import { environment } from 'environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class ApiService {

  protected readonly hostAddress = environment.apiHost;

  protected constructor(private http: HttpClient, private toast: GlobalToastService) { }

  public get<T>(route: string, params?: HttpParams, descriptor?: OperationDescriptor): Observable<T> {
    return this.getPiped(this.http.get<T>(`${this.hostAddress}/${route}`, { params: params }),
      descriptor);
  }

  public put<TSend, TResult>(route: string, payload: TSend, params?: HttpParams, descriptor?: OperationDescriptor) {
    return this.getPiped(this.http.put<TResult>(`${this.hostAddress}/${route}`, payload, { params: params }),
      descriptor);
  }

  public post<TSend, TResult>(route: string, payload: TSend, params?: HttpParams, descriptor?: OperationDescriptor) {
    return this.getPiped(this.http.post<TResult>(`${this.hostAddress}/${route}`, payload, { params: params }),
      descriptor);
  }

  public delete<TResult>(route: string, params?: HttpParams, descriptor?: OperationDescriptor) {
    return this.getPiped(this.http.delete<TResult>(`${this.hostAddress}/${route}`, { params: params }),
      descriptor);
  }

  private getPiped<T>(observable: Observable<T>, descriptor?: OperationDescriptor): Observable<T> {
    return observable
      .pipe(
        tap(_ => {
          if (descriptor?.successMessage)
            this.toast.showSuccess(descriptor.successMessage);
        }),
        catchError(err => {
          if (descriptor?.failMessage)
            this.toast.showError(descriptor.failMessage);

          return throwError(err);
        })
      );
  }
}

export interface OperationDescriptor {
  failMessage?: string,
  successMessage?: string
}