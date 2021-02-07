import { Component, OnInit, OnDestroy } from '@angular/core';
import {ServerStatusService} from '../server-status.service';
import {Subscription} from 'rxjs';
import { faCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-server-status-widget',
  templateUrl: './server-status-widget.component.html'
})
export class ServerStatusWidgetComponent implements OnInit, OnDestroy {

  faCircle = faCircle;

  isOnline: boolean;

  private sub: Subscription;

  constructor(
    private serverStat: ServerStatusService) { }

  ngOnInit() {
    this.isOnline = this.serverStat.serverAvailable;

    this.sub = this.serverStat.serverAvailabilityChanged.subscribe(
      status => this.isOnline = status
    )
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
