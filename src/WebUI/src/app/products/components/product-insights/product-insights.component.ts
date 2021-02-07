import { Component, OnInit } from '@angular/core';
import {Product} from '../../models/product';
import {ProductService} from '../../services/product.service';
import { faSpinner, faTrophy, faBox, faBoxes } from '@fortawesome/free-solid-svg-icons';

enum DataState {
  Waiting,
  Error,
  Success
}

@Component({
  selector: 'app-stock-insights',
  templateUrl: './product-insights.component.html'
})

export class ProductInsightsComponent implements OnInit {

  fa = {
    spinner: faSpinner,
    trophy: faTrophy,
    box: faBox,
    boxes: faBoxes
  };

  productMostStocked: Product;
  productHeaviest: Product;

  get dataState() { return DataState; }

  mostStockedLoadState: DataState = DataState.Waiting;
  heaviestLoadState: DataState = DataState.Waiting;

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.getMostStocked();
    this.getHeaviest();
  }

  getMostStocked() {
    this.mostStockedLoadState = DataState.Waiting;

    this.productService.getMostStocked().subscribe(
      res => { this.productMostStocked = res; this.mostStockedLoadState = DataState.Success; },
      _ => { this.mostStockedLoadState = DataState.Error; });
  }

  getHeaviest() {
    this.heaviestLoadState = DataState.Waiting;

    this.productService.getHeaviest().subscribe(
      res => { this.productHeaviest = res; this.heaviestLoadState = DataState.Success; },
      _ => { this.heaviestLoadState = DataState.Error; });
  }
}
