import {ErrorHandler, NgZone} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

export class AppErrorHandler implements ErrorHandler {

  constructor(
    private toastr: ToastrService,
    private ngZone: NgZone) {}

  handleError(error: any): void {
    this.ngZone.run(() => {
      console.log(error);
    });
  }
}
