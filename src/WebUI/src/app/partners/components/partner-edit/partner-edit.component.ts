import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ConfirmationDialogService} from '../../../core/notifications/services/confirmation-dialog/confirmation-dialog.service';
import {PartnerListing} from '../../models/partner-listing';
import {PartnerService} from '../../services/partner.service';
import { faSpinner } from '@fortawesome/free-solid-svg-icons';
import { PartnerUpdate } from 'app/partners/models/partner-update';

enum LoadState {
  Loading,
  Success,
  Error
}

@Component({
  selector: 'app-partner-edit',
  templateUrl: './partner-edit.component.html'
})
export class PartnerEditComponent implements OnInit {

  @Input() showInCard = true;

  private _partnerId: number;
  @Input()
  set partnerId(id: number) {
    this._partnerId = id;
    if (id > 0)
      this.loadEntity();
  }
  get partnerId() {
    return this._partnerId;
  }

  @Output() closeRequested = new EventEmitter(); //TODO
  @Output() partnerUpdated = new EventEmitter<PartnerUpdate>();
  @Output() partnerDeleted = new EventEmitter<PartnerUpdate>();

  faSpinner = faSpinner;

  partner: PartnerUpdate;

  loadState = LoadState.Loading;
  get state() { return LoadState; }

  constructor(private service: PartnerService,
              private confirmDialog: ConfirmationDialogService) { }

  ngOnInit() {
  }

  loadEntity() {
    this.loadState = LoadState.Loading;
    this.service.get(this.partnerId).subscribe(
      res => { this.partner = res; this.loadState = LoadState.Success; },
      err => this.loadState = LoadState.Error);
  }

  onSaveChanges(partner: PartnerUpdate) {
    this.service.update(this.partnerId, partner).subscribe(
      _res => this.partnerUpdated.emit(partner)
    );
  }

  onDeleteEntity(partner: PartnerListing) {
    this.confirmDialog.confirm(
      'Confirm deletion',
      'Are you sure you delete this partner?',
      'Delete',
      'Cancel').then(confirmed => {
      if (confirmed)
        this.deleteEntity()});
  }

  deleteEntity() {
    this.service.delete(this.partnerId).subscribe(
      _res => {
        this.partnerDeleted.emit(this.partner);
        this.partner = null;
      }
    );
  }
}
