import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';
import { HttpErrorNotifierService } from './http-error-notifier.service';
import { AuthService } from 'app/core/auth/services/auth.service';

@Injectable()
export class ServerValidationErrorInterceptor implements HttpInterceptor {

    readonly requestTimeout = 60000;

    constructor(protected httpErrorHandler: HttpErrorNotifierService,
                protected authService: AuthService) {}

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

                        // Little bit of SRP violation here, but I didn't feel like adding a separate interceptor.
                        // Force user to log in again if we get an Unauthorized response (meaning that token is invalid).
                        if (err.status == 401 && this.authService.isSignedIn) {
                            this.authService.signOut();
                        }
                        else {
                            this.httpErrorHandler.handle(err, req);
                        }
                    }

                    return throwError(err);
                })
            );
    }
}