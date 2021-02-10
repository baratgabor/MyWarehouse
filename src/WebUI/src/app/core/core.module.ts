import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreRoutingModule } from './core-routing.module';
import { ConfirmationDialogComponent } from './notifications/services/confirmation-dialog/confirmation-dialog.component';
import { DisplayCardComponent } from './components/display-card/display-card.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthInterceptor } from "./auth/interceptors/auth-interceptor";
import { NavbarLoginInfoComponent } from './auth/components/navbar-login-info/navbar-login-info.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ServerStatusWidgetComponent } from './serverStatus/server-status-widget/server-status-widget.component';
import { LoginFormComponent } from './auth/components/login-form/login-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CurrencySelectorComponent } from './exchangeRates/components/currency-selector/currency-selector.component';
import { ItemCountSelectorComponent } from './components/item-count-selector/item-count-selector.component';
import { ServerValidationErrorInterceptor } from './errorhandling/http-errors/services/http-error.interceptor';
import { ShowValidationErrorsComponent } from './errorhandling/form-validation/components/show-validation-errors/show-validation-errors.component';
import { HttpErrorNotificationComponent } from './errorhandling/http-errors/components/http-error-notification/http-error-notification.component';
import { CachingInterceptor } from './http/services/caching-interceptor';

@NgModule({
  entryComponents:[
    ConfirmationDialogComponent
  ],
  declarations: [
    ConfirmationDialogComponent,
    DisplayCardComponent,
    HeaderComponent,
    FooterComponent,
    NotFoundComponent,
    NavbarLoginInfoComponent,
    ServerStatusWidgetComponent,
    LoginFormComponent,
    CurrencySelectorComponent,
    ItemCountSelectorComponent,
    ShowValidationErrorsComponent,
    HttpErrorNotificationComponent
    ],
  imports: [
    CommonModule,
    CoreRoutingModule,
    FontAwesomeModule,
    NgbModule,
    ReactiveFormsModule,
    RouterModule
  ],
  exports: [
    ConfirmationDialogComponent,
    DisplayCardComponent,
    HeaderComponent,
    FooterComponent,
    NavbarLoginInfoComponent,
    ServerStatusWidgetComponent,
    LoginFormComponent,
    CurrencySelectorComponent,
    ItemCountSelectorComponent,
    ShowValidationErrorsComponent
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ServerValidationErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CachingInterceptor,
      multi: true
    }
  ]
})
export class CoreModule { }
