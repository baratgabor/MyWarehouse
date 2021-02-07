import { Component, OnInit, OnDestroy } from '@angular/core';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar-login-info',
  templateUrl: './navbar-login-info.component.html'
})
export class NavbarLoginInfoComponent implements OnInit, OnDestroy {

  faUser = faUser;

  loggedIn: boolean;
  username: string;

  private sub: Subscription;

  constructor(private as: AuthService) { }

  ngOnInit() {
    this.sub = this.as.loginState.subscribe(newLoginState => {
      this.loggedIn = newLoginState;

      if (this.loggedIn) {
        this.username = this.as.getUsername();
      }
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
