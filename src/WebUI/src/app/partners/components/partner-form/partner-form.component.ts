import { Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { FormBuilder, FormGroup, Validators, NgForm } from '@angular/forms';
import { faAddressCard } from '@fortawesome/free-solid-svg-icons';
import { PartnerCreate } from 'app/partners/models/partner-create';
import { PartnerUpdate } from 'app/partners/models/partner-update';
import {PartnerListing} from '../../models/partner-listing';

@Component({
  selector: 'app-partner-form',
  templateUrl: './partner-form.component.html'
})

// Reusable transaction form for multiple operations (update/new)
export class PartnerFormComponent implements OnInit {

  @Input() showInCard = true;
  @Input() formTitle = 'Partner';
  @Input() submitText = 'Submit';
  @Input() secondaryBtnText = '';

  // Product model to load into the form
  private _partner: PartnerUpdate;
  @Input()
  set partner(partner: PartnerUpdate) {
    this._partner = partner;

    if(partner)
      this.initForm();
    else
      this.makeEmptyDisabledForm();
  }
  get partner() {
    return this._partner;
  }

  // Events emitted
  @Output() submitClicked = new EventEmitter<PartnerUpdate>();
  @Output() secondaryBtnClicked = new EventEmitter<PartnerUpdate>();
  @Output() closeBtnClicked = new EventEmitter<PartnerUpdate>();

  partnerForm: FormGroup;
  partnerAddressForm: FormGroup;
  submitAttempted: boolean;
  faAddress = faAddressCard;

  // Convenience getter for easy access to form fields
  get f() { return this.partnerForm.controls; }
  get fa() { return this.partnerAddressForm.controls; }

  constructor(
    private formBuilder: FormBuilder) {}

  ngOnInit() {}

  private initForm() {

    if (this.partnerForm)
      this.partnerForm.enable();

    this.partnerAddressForm = this.formBuilder.group({
        country:        [this.partner.address.country, [Validators.required, Validators.maxLength(100)]],
        zipCode:        [this.partner.address.zipCode, [Validators.required, Validators.maxLength(100)]],
        city:           [this.partner.address.city, [Validators.required, Validators.maxLength(100)]],
        street:         [this.partner.address.street, [Validators.required, Validators.maxLength(100)]],
    })

    this.partnerForm = this.formBuilder.group({
      id:             [this.partner.id],
      name:           [this.partner.name, [Validators.required, Validators.maxLength(100)]],
      address:        this.partnerAddressForm
    });
  }

  private makeEmptyDisabledForm() {

    this.partnerForm = this.formBuilder.group({
      id:             '',
      name:           '',
      address:        this.formBuilder.group({
        country:        '',
        zipCode:        '',
        city:           '',
        street:         '',
      })
    });
    this.partnerForm.disable();
  }

  onSubmit() {
    if (!this.validate())
      return;

    this.partner = this.partnerForm.value;
    this.submitClicked.emit(this.partner);
  }

  onSecondary() {
    if (!this.validate())
      return;

    this.partner = this.partnerForm.value;
    this.secondaryBtnClicked.emit(this.partner);
  }

  validate(): boolean {
    this.submitAttempted = true;
    return !this.partnerForm.invalid;
  }
}
