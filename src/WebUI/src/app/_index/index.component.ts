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
    this.sub = this.auth.signInState.subscribe(
      (userData) => this.isLoggedIn = userData != null
    )
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
