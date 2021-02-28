import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

// Adds Authorization header and token to HTTP requests when a token is available.
@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>,
    next: HttpHandler): Observable<HttpEvent<any>> {

    // Don't add token if URL is not our API
    if (!req.url.includes(environment.baseHost))
      return next.handle(req);

    const token = this.authService.getUserToken();

    if (!token)
      return next.handle(req);

    const cloned = req.clone({
      headers: req.headers.set('Authorization', token)
    });

    return next.handle(cloned);
  }
}
