import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../products/services/product.service';
import { faCalculator, faWeightHanging, faMoneyBillWave, faSpinner } from '@fortawesome/free-solid-svg-icons';

enum DataState {
  Waiting,
  Error,
  Success
}

@Component({
  selector: 'app-stock-metrics',
  templateUrl: './stock-metrics.component.html'
})

export class StockMetricsComponent implements OnInit {

  fa = {
    calculator: faCalculator,
    weight: faWeightHanging,
    money: faMoneyBillWave,
    spinner: faSpinner
  };

  errorMessage = 'Error fetching data :(';
  get dataState() { return DataState; }

  stockMass: number;
  stockMassUnit: string;
  stockMassState: DataState = DataState.Waiting;

  stockValue: number;
  stockValueCurrency: string;
  stockValueState: DataState = DataState.Waiting;

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.getMass();
    this.getValue();
  }

  getMass(forceRefresh = false) {
    this.stockMassState = DataState.Waiting;

    this.productService.getAggregateMass(forceRefresh).subscribe(
      res => {
        this.stockMass = res.value;
        this.stockMassUnit = res.unit;
        this.stockMassState = DataState.Success;
      },
      _ => { this.stockMassState = DataState.Error; });
  }

  getValue(forceRefresh = false) {
    this.stockValueState = DataState.Waiting;

    this.productService.getAggregateValue(forceRefresh).subscribe(
      res => {
        this.stockValue = res.amount;
        this.stockValueCurrency = res.currencyCode;
        this.stockValueState = DataState.Success;
      },
      _ => { this.stockValueState = DataState.Error; });
  }
}
