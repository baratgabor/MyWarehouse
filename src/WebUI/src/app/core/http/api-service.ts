import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { GlobalToastService } from '../notifications/services/global-toast-service';
import { environment } from 'environments/environment';
import { Injectable } from '@angular/core';

// I'm not convinced this is elegant; in fact arguably it's rather crude,
// but I'm not that much of an Angular/RxJs pro, and this will do the job.
@Injectable({
  providedIn: 'root'
})
export class HttpContext {

  public get LastRequest() : httpMethod { return this._lastRequest; }
  public get LastParams() : HttpParams { return this._lastParams; }
  public get LastRoute() : string { return this._lastRoute; }
  public get LastPayload() : any { return this._lastPayload; }
  public get LastDescriptor() : any { return this._lastDescriptor; }
  
  private _lastRequest: httpMethod;
  private _lastParams: HttpParams;
  private _lastRoute: string;
  private _lastPayload: any;
  private _lastDescriptor: OperationDescriptor;

  protected setContext(method: httpMethod, params: HttpParams, route: string, payload: any, descriptor: OperationDescriptor) {
    this._lastRequest = method;
    this._lastParams = params;
    this._lastRoute = route;
    this._lastPayload = payload;
    this._lastDescriptor = descriptor;
  }
}

@Injectable({
  providedIn: 'root'
})
export class ApiService extends HttpContext {

  protected readonly hostAddress = environment.apiHost;

  protected constructor(private http: HttpClient, private toast: GlobalToastService) {
    super();
   }

  public get<T>(route: string, params?: HttpParams, descriptor?: OperationDescriptor): Observable<T> {
    this.setContext(httpMethod.get, params, route, null, descriptor);
    return this.getPiped(this.http.get<T>(`${this.hostAddress}/${route}`, { params: params }),
      descriptor);
  }

  public put<TSend, TResult>(route: string, payload: TSend, params?: HttpParams, descriptor?: OperationDescriptor) {
    this.setContext(httpMethod.put, params, route, payload, descriptor);
    return this.getPiped(this.http.put<TResult>(`${this.hostAddress}/${route}`, payload, { params: params }),
      descriptor);
  }

  public post<TSend, TResult>(route: string, payload: TSend, params?: HttpParams, descriptor?: OperationDescriptor) {
    this.setContext(httpMethod.post, params, route, payload, descriptor);
    return this.getPiped(this.http.post<TResult>(`${this.hostAddress}/${route}`, payload, { params: params }),
      descriptor);
  }

  public delete<TResult>(route: string, params?: HttpParams, descriptor?: OperationDescriptor) {
    this.setContext(httpMethod.delete, params, route, null, descriptor);
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

export enum httpMethod { get, put, post, delete };

export interface OperationDescriptor {
  failMessage?: string,
  successMessage?: string
}