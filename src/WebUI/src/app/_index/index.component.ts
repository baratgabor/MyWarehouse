import { Component, OnInit, OnDestroy } from '@angular/core';
import {AuthService} from '../core/auth/services/auth.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html'
})
export class IndexComponent implements OnInit, OnDestroy {

  isLoggedIn: boolean;

  private sub;

  constructor(private auth : AuthService) {}

  ngOnInit() {
    this.sub = this.auth.loginState.subscribe(
      (status) => this.isLoggedIn = status
    )
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
