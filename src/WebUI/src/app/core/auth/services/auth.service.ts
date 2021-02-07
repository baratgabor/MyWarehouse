import {EventEmitter, Injectable} from '@angular/core';
import {HttpClient, HttpParams, HttpResponse} from '@angular/common/http';
import {environment} from '../../../../environments/environment';
import { shareReplay } from 'rxjs/operators';
import { tap } from 'rxjs/operators';
import {AuthenticationSuccessData} from '../models/login-data';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class AuthService {

  private previousLoginState = false;
  public loginState = new BehaviorSubject<boolean>(this.isLoggedIn());

  constructor(private http: HttpClient) {

    setInterval(() => {
      if (this.isLoggedIn() !== this.previousLoginState) {
        this.previousLoginState = !this.previousLoginState;
        this.loginState.next(this.previousLoginState);
      }}, 1000);
  }

  authorize(username: string, password: string): any {
    const loginData = {
      username: username,
      password: password
    };

    return this.http.post<AuthenticationSuccessData>(`${environment.baseHost}/account/login`, loginData, { observe: 'response' })
      .pipe(
        tap(res => this.setSession(res)),
        shareReplay()
      );
  }

  private setSession(authResult: HttpResponse<AuthenticationSuccessData>) {
    const expiresAt = new Date();
    expiresAt.setTime(Date.now() + (authResult.body.expires_in * 1000));

    localStorage.setItem('userName', authResult.body.username);
    localStorage.setItem('access_token', authResult.body.access_token);
    localStorage.setItem('token_type', authResult.body.token_type);
    localStorage.setItem('expires_at', expiresAt.getTime().toString());
  }

  logout() {
    localStorage.removeItem('userName');
    localStorage.removeItem('access_token');
    localStorage.removeItem('token_type');
    localStorage.removeItem('expires_at');
  }

  public isLoggedIn() {
    return  +localStorage.getItem('expires_at') > Date.now();
  }

  public getUsername() {
    return localStorage.getItem('userName');
  }

  public getUserToken() {
    return `${localStorage.getItem('token_type')} ${localStorage.getItem('access_token')}`;
  }

  public getValidityDays() {
    return (+localStorage.getItem('expires_at') - Date.now()) / 1000 / (3600 * 24);
  }
}
