import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-stock',
  templateUrl: './stock-index.component.html'
})
export class StockIndexComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  navigateToStockView() {
    this.router.navigate(['/products', { stocked : 'yes' }]);
  }

}
