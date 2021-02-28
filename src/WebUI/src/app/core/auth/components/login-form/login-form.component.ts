import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { faSpinner } from '@fortawesome/free-solid-svg-icons';
import { Subscription, timer } from 'rxjs';
import { AuthenticationSuccessData } from '../../models/login-data';

enum LocalLoginState {
  None,
  Waiting,
  Success,
  ErrorWrongData,
  ErrorOther
}

enum ExternalLoginState {
  None,
  Waiting,
  Success,
  Error
}

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html'
})
export class LoginFormComponent implements OnInit, OnDestroy {

  faSpinner = faSpinner;

  isLoggedIn: boolean;
  loggedInUsername: string;
  validityDays: number;

  form: FormGroup;
  formSubmitAttempt: boolean;

  localLoginState = LocalLoginState.None;
  get localLoginStates() { return LocalLoginState; }

  externalLoginState = ExternalLoginState.None;
  get externalLoginStates() { return ExternalLoginState; }

  // convenience getter for easy access to form fields
  get f() { return this.form.controls; }

  private sub: Subscription;

  constructor(private as: AuthService) {
  }

  ngOnInit() {
    this.sub = this.as.signInState.subscribe(userData => {
      this.isLoggedIn = userData != null;
      this.updateUserData(userData);

      if (!this.isLoggedIn && !this.form)
        this.createLoginForm();
    });
  }

  private createLoginForm() {
    this.form = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      grant_type: new FormControl('password'),
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  signInWithGoogle() {
    this.externalLoginState = ExternalLoginState.Waiting;
    this.as.signInWithGoogle()
      .catch(_ => this.externalLoginState = ExternalLoginState.Error);
  }

  onSubmit() {

    this.formSubmitAttempt = true;
    if (this.form.invalid) {
      return;
    }

    this.localLoginState = LocalLoginState.Waiting;
    this.form.disable();

    this.as.authenticate(this.form.value.username, this.form.value.password).subscribe(
      _ => {
        this.localLoginState = LocalLoginState.Success;
        timer(5000).subscribe(() => this.localLoginState = LocalLoginState.None); // In case user logs out without navigating elsewhere; the 'success' would still be visible.
        this.form.enable();
      },
      err => {

        this.form.enable();

        if (err.status == 401)
          this.localLoginState = LocalLoginState.ErrorWrongData;
        else
          this.localLoginState = LocalLoginState.ErrorOther;
      }
    );
  }

  signOut() {
    this.as.signOut();
  }

  updateUserData(userData: AuthenticationSuccessData) {
    if (this.isLoggedIn) {
      this.loggedInUsername = userData.username;
      this.validityDays = Math.round(this.as.getValidityDays());
    } else {
      this.loggedInUsername = '';
      this.validityDays = 0;
    }
  }

}
