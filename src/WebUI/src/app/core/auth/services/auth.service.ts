import {Injectable, OnDestroy} from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {environment} from '../../../../environments/environment';
import { shareReplay } from 'rxjs/operators';
import { tap } from 'rxjs/operators';
import {AuthenticationSuccessData} from '../models/login-data';
import {BehaviorSubject, Observable, Subscription} from 'rxjs';
import { SocialAuthService, GoogleLoginProvider, SocialUser } from 'angularx-social-login';

@Injectable({
  providedIn: 'root'
})

export class AuthService implements OnDestroy {
  
  public signInState: Observable<AuthenticationSuccessData>;
  private _signInState = new BehaviorSubject<AuthenticationSuccessData>(null);
  private socialAuthSub: Subscription;

  constructor(private _http: HttpClient, private _socialAuth: SocialAuthService) {

    this.signInState = this._signInState.asObservable();

    const userData = this.getStoredUserData();
    if (userData != null) {
      this._signInState.next(userData);
    }

    this.socialAuthSub = this._socialAuth.authState.subscribe((user) => 
      this.authenticateExternalSignIn(user));
  }

  ngOnDestroy(): void {
    this.socialAuthSub.unsubscribe();
  }

  public authenticate(username: string, password: string): Observable<HttpResponse<AuthenticationSuccessData>> {
    const loginData = {
      username: username,
      password: password
    };

    return this._http.post<AuthenticationSuccessData>(`${environment.baseHost}/account/login`, loginData, { observe: 'response' })
      .pipe(
        tap(res => this.signIn(res.body)),
        shareReplay()
      );
  }

  private authenticateExternalSignIn(user: SocialUser) {
    if (user == null) {
      this.signOut();
      return;
    }

    const externalTokenData = {
      idToken: user.idToken,
      provider: ExternalAuthenticationProviders[user.provider]
    };

    this._http.post<AuthenticationSuccessData>(`${environment.baseHost}/account/loginExternal`, externalTokenData, { observe: 'response' })
      .subscribe(res => 
        this.signIn(res.body));
  }

  public signInWithGoogle(): Promise<SocialUser> {
    return this._socialAuth.signIn(GoogleLoginProvider.PROVIDER_ID);
  }

  private signIn(data: AuthenticationSuccessData) {
    const expiresAt = new Date();
    expiresAt.setTime(Date.now() + (data.expiresIn * 1000));

    localStorage.setItem('auth_userData', JSON.stringify(data));
    localStorage.setItem('auth_tokenString', `${data.tokenType} ${data.accessToken}`);
    localStorage.setItem('auth_tokenExpiresAt', expiresAt.getTime().toString());
    this._signInState.next(data);
  }

  public signOut() {
    localStorage.removeItem('auth_userData');
    localStorage.removeItem('auth_tokenString');
    localStorage.removeItem('auth_tokenExpiresAt');
    this._socialAuth.signOut().catch(_ => {});
    this._signInState.next(null);
  }

  public isSignedIn() {
    return this._signInState.value != null;
  }

  public getUserToken() {
    return localStorage.getItem('auth_tokenString');
  }

  public getValidityDays() {
    return (+localStorage.getItem('auth_tokenExpiresAt') - Date.now()) / 1000 / (3600 * 24);
  }

  private getStoredUserData(): AuthenticationSuccessData {
    return JSON.parse(localStorage.getItem('auth_userData')) as AuthenticationSuccessData;
  }
}

enum ExternalAuthenticationProviders {
  Google
}