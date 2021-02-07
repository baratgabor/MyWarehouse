import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Router} from '@angular/router';
import { faBoxes, faCoins, faWeightHanging, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import {Product} from '../../models/product';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html'
})
export class ProductCardComponent implements OnInit {

  fa = {
    boxes: faBoxes,
    weight: faWeightHanging,
    coins: faCoins
  };

  private productsUrl = '/products';

  @Input() faIcon: IconDefinition;
  @Input() cardTitle: string;
  @Input() product: Product;

  @Output() refreshRequested = new EventEmitter();

  constructor(private router: Router) { }

  ngOnInit() { }

  navigateToProductEdit() {
    this.router.navigate([this.productsUrl, {edit: this.product.id}]);
  }
}
