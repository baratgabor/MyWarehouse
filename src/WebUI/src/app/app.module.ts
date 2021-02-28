import { BrowserModule } from '@angular/platform-browser';
import {ErrorHandler, NgModule} from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import {CommonModule, CurrencyPipe, DatePipe} from '@angular/common';

// Angular Bootstrap
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

// Angular Font Awesome
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

// Main component
import { AppComponent } from './app.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { IndexComponent } from './_index/index.component';
import { HttpClientModule } from '@angular/common/http';

import { registerLocaleData } from '@angular/common';
import localeHu from '@angular/common/locales/hu';
import localeHuExtra from '@angular/common/locales/extra/hu';
import {CoreModule} from './core/core.module';
import {ToastrModule} from 'ngx-toastr';
import {AppErrorHandler} from './app.error-handler';
import {StockModule } from './stock/stock.module';
import {PartnersModule} from './partners/partners.module';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {TransactionsModule} from './transactions/transactions.module';
import {ProductsModule} from './products/products.module';
import {CanActivate} from '@angular/router';
import {LoggedInCanActivate} from './core/auth/services/logged-in-can-activate.service';

import { SocialLoginModule, SocialAuthServiceConfig } from 'angularx-social-login';
import { GoogleLoginProvider } from 'angularx-social-login';

registerLocaleData(localeHu, 'hu-HU', localeHuExtra);

@NgModule({
  declarations: [
    AppComponent,
    IndexComponent,
  ],
  imports: [
    BrowserModule,
    NgbModule,
    CommonModule,
    FontAwesomeModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    CoreModule,
    StockModule,
    PartnersModule,
    ToastrModule.forRoot(),
    BrowserAnimationsModule,
    TransactionsModule,
    ProductsModule,
    SocialLoginModule,

    AppRoutingModule,
  ],
providers: [
  CurrencyPipe,
  DatePipe,
  //{ provide: ErrorHandler, useClass: AppErrorHandler }
  {
    provide: 'SocialAuthServiceConfig',
    useValue: {
      autoLogin: false,
      providers: [
        {
          id: GoogleLoginProvider.PROVIDER_ID,
          provider: new GoogleLoginProvider(
            '417092478781-v986diqhp87iufde61b2th79p6o9c848.apps.googleusercontent.com'
          )
        }
      ]
    } as SocialAuthServiceConfig,
  }
],
  bootstrap: [AppComponent]
})
export class AppModule { }