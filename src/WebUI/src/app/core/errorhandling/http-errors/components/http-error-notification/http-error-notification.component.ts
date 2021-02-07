import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { faBug, faClock, faEraser, faFire, faHourglassEnd, faLock, faPowerOff, faServer } from '@fortawesome/free-solid-svg-icons';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ServerValidationErrors } from '../../models/server-validation-errors';

@Component({
  selector: 'app-http-error-notification',
  templateUrl: './http-error-notification.component.html'
})
export class HttpErrorNotificationComponent implements OnInit {

  @Input() error: HttpErrorResponse;
  @Input() modalRef: NgbModalRef;

  httpCode: number = 404;
  validationErrors: ServerValidationErrors = <ServerValidationErrors> {};

  icons = {
    bug: faBug,
    fire: faFire,
    lock: faLock,
    clock: faClock,
    server: faServer,
    eraser: faEraser,
    powerOff: faPowerOff,
    hourglass: faHourglassEnd,
  };

  constructor() { }

  ngOnInit(): void {
    this.processHttpError(this.error);
  }

  private processHttpError(error: HttpErrorResponse) {

    this.httpCode = error.status;
    this.validationErrors = error.error;
  }
}
