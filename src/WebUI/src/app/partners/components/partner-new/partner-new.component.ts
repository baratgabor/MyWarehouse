import { Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {PartnerService} from '../../services/partner.service';
import { PartnerCreate } from 'app/partners/models/partner-create';

@Component({
  selector: 'app-partner-new',
  templateUrl: './partner-new.component.html'
})
export class PartnerNewComponent implements OnInit {

  @Input() showInCard = true;
  @Output() partnerCreated = new EventEmitter<PartnerCreate>();

  partner: PartnerCreate = { address: {} } as PartnerCreate;

  constructor(private service: PartnerService) 
  {}
  
  ngOnInit() {
  }

  onCreate(partner: PartnerCreate) {
    this.service.create(partner).subscribe(
      _res => this.partnerCreated.emit(partner)
    );
  }
}
