import { HttpErrorResponse, HttpRequest } from "@angular/common/http";
import { Injectable, OnDestroy } from "@angular/core";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { Subscription } from "rxjs";
import { HttpErrorNotificationComponent } from "../components/http-error-notification/http-error-notification.component";

@Injectable({
    providedIn: 'root'  
})
export class HttpErrorNotifierService implements OnDestroy {

    private activeModals: NgbModalRef[] = [];
    private sub: Subscription;

    constructor(private modalService: NgbModal) {
        this.sub = this.modalService.activeInstances
        .subscribe(list => this.activeModals = list);
    }

    ngOnDestroy(): void {
        this.sub?.unsubscribe();
    }

    public handle(httpError: HttpErrorResponse, httpRequest: HttpRequest<any>) {

        if (httpError.status == 400 && httpRequest.method !== 'POST') {
            // TODO: Make 400/validation notification filtering depend on an explicit specification of whether the request was a form submission we should be acting on.
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