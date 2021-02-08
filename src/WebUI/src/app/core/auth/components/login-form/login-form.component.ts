import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { faSpinner } from '@fortawesome/free-solid-svg-icons';
import { Subscription, timer } from 'rxjs';

enum RequestState {
  None,
  Waiting,
  Success,
  ErrorWrongData,
  ErrorOther
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

  loadState = RequestState.None;
  get state() { return RequestState; }

  // convenience getter for easy access to form fields
  get f() { return this.form.controls; }

  private sub: Subscription;

  constructor(private as: AuthService) {
  }

  ngOnInit() {
    this.sub = this.as.loginState.subscribe(newState => {
      this.isLoggedIn = newState;
      this.updateUserData();

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

  onSubmit() {

    this.formSubmitAttempt = true;
    if (this.form.invalid) {
      return;
    }

    this.loadState = RequestState.Waiting;
    this.form.disable();

    this.as.authorize(this.form.value.username, this.form.value.password).subscribe(
      _ => {
        this.loadState = RequestState.Success;
        timer(5000).subscribe(() => this.loadState = RequestState.None); // In case user logs out without navigating elsewhere; the 'success' would still be visible.
        this.form.enable();
      },
      err => {

        this.form.enable();

        if (err.status == 401)
          this.loadState = RequestState.ErrorWrongData;
        else
          this.loadState = RequestState.ErrorOther;
      }
    );
  }

  doLogout() {
    this.as.logout();
  }

  updateUserData() {
    if (this.isLoggedIn) {
      this.loggedInUsername = this.as.getUsername();
      this.validityDays = Math.round(this.as.getValidityDays());
    } else {
      this.loggedInUsername = '';
      this.validityDays = 0;
    }
  }

}
