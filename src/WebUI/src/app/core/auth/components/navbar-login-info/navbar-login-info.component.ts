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
  validityDays: number;

  private sub: Subscription;

  constructor(private as: AuthService, private modalService: NgbModal) { }

  ngOnInit() {
    this.sub = this.as.loginState.subscribe(newLoginState => {
      this.isLoggedIn = newLoginState;

      if (this.isLoggedIn) {
        this.username = this.as.getUsername();
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

  logout() {
    this.as.logout();
  }
}
