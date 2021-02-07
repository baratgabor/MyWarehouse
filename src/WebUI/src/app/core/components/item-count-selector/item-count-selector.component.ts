import {Component, EventEmitter, Output, OnInit } from '@angular/core';

@Component({
  selector: 'app-item-count-selector',
  templateUrl: './item-count-selector.component.html'
})
export class ItemCountSelectorComponent implements OnInit {

  selectedItemCount = 5;
  @Output() itemCountSelected = new EventEmitter<number>();

  itemCountArray = [5, 10, 20, 50];

  constructor() { }

  setItemCount(value: number) {
    this.selectedItemCount = value;
    this.itemCountSelected.emit(value);

    localStorage.setItem('itemsPerPage', value ? value.toString() : '');
  }

  ngOnInit(): void {
    let storedSetting = +localStorage.getItem('itemsPerPage');

    if (storedSetting)
    {
      this.selectedItemCount = storedSetting;
      this.itemCountSelected.emit(storedSetting);
    }
    else
    {
      this.itemCountSelected.emit(this.selectedItemCount);
    }
  }
}
