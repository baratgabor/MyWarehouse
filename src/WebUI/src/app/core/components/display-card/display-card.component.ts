import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { IconDefinition } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-display-card',
  templateUrl: './display-card.component.html'
})

export class DisplayCardComponent implements OnInit {

  @Input() title = 'Title';
  @Input() content = 'Body text.';
  @Input() linkText = '';
  @Input() faIcon: IconDefinition;

  @Output() linkClicked = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  delegateClick() {
    this.linkClicked.emit();
  }
}
