import { Injectable } from '@angular/core';
import { HttpEvent, HttpRequest, HttpResponse, HttpInterceptor, HttpHandler } from '@angular/common/http';

import { tap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { CustomHeaders } from '../models/custom-headers';

const maxAgeMs = 120000;

@Injectable()
export class CachingInterceptor implements HttpInterceptor {

  cache = new Map();

  intercept(request: HttpRequest<any>, next: HttpHandler) {

    if (request.method !== 'GET' || !request.headers.has(CustomHeaders.ClientCached)) {
      return next.handle(request);
    }
    
    let cachedResponse = null;
    
    if (!request.headers.has(CustomHeaders.ForceRefresh)) {
      cachedResponse = this.tryGetCached(request);
    }

    return cachedResponse ? of(cachedResponse) : this.sendRequest(request, next);
  }

  sendRequest(
    request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      tap(res => {
        if (res instanceof HttpResponse) {
          this.saveCached(request, res);
        }}));
  }

  tryGetCached(request: HttpRequest<any>): HttpResponse<any> | undefined {
    const url = request.urlWithParams;
    const cached = this.cache.get(url);

    if (!cached) {
      return undefined;
    }

    const isExpired = cached.lastRead < (Date.now() - maxAgeMs);
    
    if (isExpired) {
      return undefined;
    }

    return cached.response;
  }

  saveCached(request: HttpRequest<any>, response: HttpResponse<any>): void {
    const url = request.urlWithParams;
    const entry = { url, response, lastRead: Date.now() };
    this.cache.set(url, entry);

    const expired = Date.now() - maxAgeMs;
    this.cache.forEach(expiredEntry => {
      if (expiredEntry.lastRead < expired) 
        this.cache.delete(expiredEntry.url);
      });
  }
}
