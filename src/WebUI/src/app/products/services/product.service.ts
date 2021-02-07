import {Injectable} from '@angular/core';
import {HttpParams, HttpResponse} from '@angular/common/http';
import {PagedState} from '../../core/http/paged-state';
import {Product} from '../models/product';
import {Observable} from 'rxjs';
import {Money} from '../../core/models/Money';
import {ProductSummary} from '../models/product-summary';
import {Mass} from '../../core/models/Mass';
import {ProductStockCount} from '../models/product-stock-count';
import {ProductCreate} from '../models/product-create';
import {ApiService} from 'app/core/http/api-service';

@Injectable({
  providedIn: 'root'
})

export class ProductService {

  constructor(private apiService: ApiService) { }

  getProducts(params: HttpParams, onlyStocked: boolean = false, ) : Observable<PagedState<ProductSummary>>
  {
    if (onlyStocked)
      params = params.append('status','stocked');

    return this.apiService.get<PagedState<ProductSummary>>('products', params);
  }

  getProduct(id: number)
  {
    return this.apiService.get<Product>(`products/${id}`);
  }

  updateProduct(id: number, product: Product): Observable<number>
  {
    return this.apiService.put<Product, number>(`products/${id}`, product, null,
    {
      successMessage: 'Changes saved.',
      failMessage: 'Saving changes failed.'
    });
  }

  createProduct(product: ProductCreate): Observable<number>
  {
    return this.apiService.post<ProductCreate, number>(`products`, product, null,
    {
      successMessage: 'New product added.',
      failMessage: 'Product creation failed.'
    });
  }

  deleteProduct(id: number) : Observable<HttpResponse<Product>>
  {
    return this.apiService.delete<HttpResponse<Product>>(`products/${id}`, null,
    {
      successMessage: 'Product deleted.',
      failMessage: 'Product deletion failed.'
    });
  }

  getAggregateMass(): Observable<Mass> {
    return this.apiService.get<Mass>(`products/totalMass`);
  }

  getAggregateValue(): Observable<Money> {
    return this.apiService.get<Money>(`products/totalValue`);
  }

  getStockCount(): Observable<ProductStockCount> {
    return this.apiService.get<ProductStockCount>(`products/stockCount`);
  }

  getMostStocked(): Observable<Product>
  {
    return this.apiService.get<Product>(`products/MostStocked`);
  }

  getHeaviest(): Observable<Product>
  {
    return this.apiService.get<Product>(`products/Heaviest`);
  }
}
