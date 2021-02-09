import { HttpErrorResponse } from "@angular/common/http";
import { Injectable, OnDestroy } from "@angular/core";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { HttpContext, httpMethod } from "app/core/http/api-service";
import { Subscription } from "rxjs";
import { HttpErrorNotificationComponent } from "../components/http-error-notification/http-error-notification.component";

@Injectable({
    providedIn: 'root'  
})
export class HttpErrorNotifierService implements OnDestroy {

    private activeModals: NgbModalRef[] = [];
    private sub: Subscription;

    constructor(private modalService: NgbModal, private context: HttpContext) {
        this.sub = this.modalService.activeInstances // Why the fuck isn't this just a normal array???
        .subscribe(list => this.activeModals = list);
    }

    ngOnDestroy(): void {
        this.sub?.unsubscribe();
    }

    public handle(httpError: HttpErrorResponse) {

        if (httpError.status == 400 && this.context.LastRequest != httpMethod.post) {
            // We don't want an error notification for e.g. GET errors; only if we sent a form.
            // Filtering for POST is crude, but currently it works fine.
            // TODO: Extend context by explicitly declaring if request was a form submission (or to find a more elegant solution).
            return;
        }

        if (httpError.status == 401) {
            return; // Authentication issues are handled by auth services.
        }

        if (this.activeModals.some((x: NgbModalRef) =>
            x.componentInstance instanceof HttpErrorNotificationComponent 
            && x.componentInstance.httpCode == httpError.status)) {
            return; // At any given time show only one notification of a given type.
        }

        this.showErrorModal(httpError);
    }

    private showErrorModal(httpError: HttpErrorResponse) {
        
        const modalRef = this.modalService.open(
          HttpErrorNotificationComponent,
          {
              centered: true,
              scrollable: true
          });

          modalRef.componentInstance.modalRef = modalRef;
          modalRef.componentInstance.error = httpError;
      }

}  