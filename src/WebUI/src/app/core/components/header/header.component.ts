import { Component, OnInit, OnDestroy } from '@angular/core';
import {AuthService} from '../../auth/services/auth.service';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {

  isCollapsed = true;
  isLoggedIn = false;

  private sub: Subscription;

  constructor(private auth: AuthService) { }

  ngOnInit() {
    this.sub = this.auth.signInState.subscribe(
      userData => this.isLoggedIn = userData != null
    );
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
