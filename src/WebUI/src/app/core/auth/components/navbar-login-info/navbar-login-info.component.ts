import { Component, OnInit, OnDestroy } from '@angular/core';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar-login-info',
  templateUrl: './navbar-login-info.component.html'
})
export class NavbarLoginInfoComponent implements OnInit, OnDestroy {

  faUser = faUser;

  isLoggedIn: boolean;
  username: string;
  email: string;
  externalLogin: string;
  validityDays: number;

  private sub: Subscription;

  constructor(private as: AuthService, private modalService: NgbModal) { }

  ngOnInit() {
    this.sub = this.as.signInState.subscribe(userData => {
      this.isLoggedIn = userData != null;

      if (this.isLoggedIn) {
        this.username = userData.username;
        this.email = userData.email;
        this.externalLogin = userData.externalAuthenticationProvider;
        this.validityDays = Math.round(this.as.getValidityDays());
      }
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  openModal(content: any) {
    this.modalService.open(content, {
      centered: true,
      keyboard: true,
      size: 'md'
    });
  }

  signOut() {
    this.as.signOut();
  }
}
