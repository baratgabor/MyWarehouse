import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';
import { HttpErrorNotifierService } from './http-error-notifier.service';

@Injectable()
export class ServerValidationErrorInterceptor implements HttpInterceptor {

    readonly requestTimeout = 20000;

    constructor(protected httpErrorHandler: HttpErrorNotifierService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return next.handle(req)
            .pipe(
                // We must make an explicit timeout to catch one.
                timeout(this.requestTimeout),
                catchError(err => {

                    // Timeout doesn't return a HttpErrorResponse, so make one.
                    if (err.name == 'TimeoutError') {
                        err = new HttpErrorResponse({
                            status: 408,
                            error: 'Request timed out.'
                        });
                    }

                    if (err instanceof HttpErrorResponse) {
                        this.httpErrorHandler.handle(err);
                    }

                    return throwError(err);
                })
            );
    }
}