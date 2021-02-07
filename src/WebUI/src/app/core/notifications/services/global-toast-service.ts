import { Injectable } from '@angular/core';
import { IndividualConfig, ToastrService } from 'ngx-toastr';

@Injectable({
    providedIn: 'root'
  })
export class GlobalToastService {
    
    private static toastConfig: Partial<IndividualConfig> = {
        timeOut: 2000,
        easeTime: 500,
        easing: 'ease-in',
        positionClass: 'toast-bottom-right'
    };
    
    constructor(private toastr: ToastrService) { }

    public showError(message: string, title?: string) {
        this.toastr.error(message, title, GlobalToastService.toastConfig);
    }

    public showSuccess(message: string, title?: string) {
        this.toastr.success(message, title, GlobalToastService.toastConfig);
    }

    public showInfo(message: string, title?: string) {
        this.toastr.info(message, title, GlobalToastService.toastConfig);
    }
}