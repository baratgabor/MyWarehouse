import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Product} from '../../models/product';
import {ProductService} from '../../services/product.service';

@Component({
  selector: 'app-product-new',
  templateUrl: './product-new.component.html'
})

export class ProductNewComponent implements OnInit {

  @Input() showInCard = true;
  @Output() productCreated = new EventEmitter<Product>();

  product: Product = {
    priceCurrencyCode: "USD",
    massUnitSymbol: "kg",
    numberInStock: 0
  } as Product;

  constructor(private ps: ProductService) {
    this.product.id = 0;
  }

  ngOnInit() {
  }

  onCreate(product: Product) {
    this.ps.createProduct(product).subscribe(
      _res => this.productCreated.emit(product)
    );
  }
}
