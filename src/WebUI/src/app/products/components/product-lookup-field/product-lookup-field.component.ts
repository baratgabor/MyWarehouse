import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Observable, of} from 'rxjs';
import {catchError, debounceTime, distinctUntilChanged, map, tap, switchMap, finalize} from 'rxjs/operators';
import {PagedState} from 'app/core/http/models/paged-state';
import {HttpParams} from '@angular/common/http';
import {Product} from '../../models/product';
import {ProductService} from '../../services/product.service';
import { faBook, faCircleNotch } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-product-lookup-field',
  templateUrl: './product-lookup-field.component.html'
})
export class ProductLookupFieldComponent implements OnInit {

  _model: Product;
  get model(): Product { return this._model; }
  set model(value: Product) {
    if (value.name == undefined)
      return; // Don't save typed in strings as model

    this._model = value;
    this.productSelected.emit(value);

    if (this.resetAfterSelection)
      this.reset();
  }

  get searching() { return this.state == 'searching'; }
  get searchFailed() { return this.state == 'searchFailed'; }
  get noResults() { return this.state == 'noResults'; }
  
  spinnerIcon = faCircleNotch;
  readyIcon = faBook;
  inputValue: string;

  query: PagedState<Product> = new PagedState<Product>();
  private state: ('ready' | 'searching' | 'searchFailed' | 'noResults') = 'ready';

  @Input() disabled: boolean = false;
  @Input() drawFocus: boolean = false;
  @Input() resetAfterSelection: boolean = false;
  @Input() set resultCount(count: number) { this.query.pageSize = count; }
  @Input() filterOnlyStocked: boolean = false;
  @Output() productSelected = new EventEmitter<Product>();

  ngOnInit() {
  }

  constructor(
    private _service: ProductService) {}

  public reset = () => {
    setTimeout(() => {
      this.inputValue = '';
      this._model = null;
    }, 10);
  };

  search = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => {
        this.state = 'searching';
      }),
      switchMap(term =>
        this._service.getProducts(this.getParams(term), this.filterOnlyStocked).pipe(
          map(r => r.results),
          tap((r) => {
            this.state = 'ready';
            if (!r || r.length == 0)
              this.state = 'noResults';
          }),
          catchError(() => {
            this.state = 'searchFailed';
            return of([]);
          })
      )),
    )

  private getParams(term: string) : HttpParams {
    this.query.addFilter("Name", term, false);
    return this.query.toQueryParams();
  }

  formatter = (x: Product) => x.name;
}
