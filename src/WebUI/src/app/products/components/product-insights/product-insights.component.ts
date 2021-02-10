import { Component, OnInit } from '@angular/core';
import {ProductService} from '../../services/product.service';
import { faSpinner, faTrophy, faBox, faBoxes } from '@fortawesome/free-solid-svg-icons';
import { PagedQuery } from 'app/core/http/models/paged-query';
import { ProductSummary } from 'app/products/models/product-summary';

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

  productMostStocked: ProductSummary;
  productHeaviest: ProductSummary;

  get dataState() { return DataState; }

  mostStockedLoadState: DataState = DataState.Waiting;
  heaviestLoadState: DataState = DataState.Waiting;

  private query = new PagedQuery();

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.getMostStocked();
    this.getHeaviest();
  }

  getMostStocked(forceRefresh = false) {
    this.mostStockedLoadState = DataState.Waiting;

    this.query.pageSize = 1;
    this.query.addSort("numberInStock", true);

    this.productService.getProducts(this.query.toQueryParams(), null, true, forceRefresh).subscribe(
      res => { this.productMostStocked = res.results[0]; this.mostStockedLoadState = DataState.Success; },
      _ => { this.mostStockedLoadState = DataState.Error; });
  }

  getHeaviest(forceRefresh = false) {
    this.heaviestLoadState = DataState.Waiting;

    this.query.pageSize = 1;
    this.query.addSort("massValue", true);

    this.productService.getProducts(this.query.toQueryParams(), null, true, forceRefresh).subscribe(
      res => { this.productHeaviest = res.results[0]; this.heaviestLoadState = DataState.Success; },
      _ => { this.heaviestLoadState = DataState.Error; });
  }
}
