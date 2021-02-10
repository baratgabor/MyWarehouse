import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../products/services/product.service';
import { faCubes, faGlobe, faListOl, faSpinner } from '@fortawesome/free-solid-svg-icons';

enum DataState {
  Waiting,
  Error,
  Success
}

@Component({
  selector: 'app-stock-summary',
  templateUrl: './stock-summary.component.html'
})
export class StockSummaryComponent implements OnInit {

  fa = {
    spinner: faSpinner,
    globe: faGlobe,
    list: faListOl,
    cubes: faCubes
  };

  errorMessage = 'Error fetching data :(';
  get dataState() { return DataState; }

  loadState: DataState = DataState.Waiting;

  productCount: number;
  stockCount: number;

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData(forceRefresh = false) {
    this.loadState = DataState.Waiting;

    this.productService.getStockCount(forceRefresh).subscribe(
      res => {
        this.productCount = res.productCount;
        this.stockCount = res.totalStock;
        this.loadState = DataState.Success;
      },
      _ => { this.loadState = DataState.Error; });
  }
}
