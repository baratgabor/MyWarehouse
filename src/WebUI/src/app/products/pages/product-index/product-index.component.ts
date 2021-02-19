import {Component, OnInit, ViewChild} from '@angular/core';
import {ProductService} from '../../services/product.service';
import {PagedState} from 'app/core/http/models/paged-state';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ActivatedRoute} from '@angular/router';
import {ProductSummary} from '../../models/product-summary';
import {Product} from '../../models/product';
import { SortState } from '../../../core/http/models/paged-query';
import { faToggleOn, faToggleOff, faSpinner, faIdCard, faCoins, 
  faWeightHanging, faBoxes, faPencilAlt, faSort, faSortUp,
  faSortDown, 
  faPlus,
  faTrashAlt} from '@fortawesome/free-solid-svg-icons';
import { SelectedCurrency } from 'app/core/exchangeRates/components/currency-selector/currency-selector.component';

enum LoadingState {
  Default,
  Waiting,
  Error,
  Success
}

@Component({
  selector: 'app-products',
  templateUrl: './product-index.component.html',
  styles: ['.p-3 { padding: 0.5rem 1rem!important; }'],
})

export class ProductIndexComponent implements OnInit {

  fa = {
    toggleOn: faToggleOn,
    toggleOff: faToggleOff,
    spinner: faSpinner,
    idCard: faIdCard,
    coins: faCoins,
    weightHanging: faWeightHanging,
    boxes: faBoxes,
    pencil: faPencilAlt,
    sort: faSort,
    sortUp: faSortUp,
    sortDown: faSortDown,
    plus: faPlus,
    trash: faTrashAlt
  };
  
  selectedForeignCurrency: string;
  selectedForeignCurrencyRate: number;

  get ls() { return LoadingState; }
  loadingState = LoadingState.Default;

  state: PagedState<ProductSummary> = new PagedState<ProductSummary>();

  selectedProductId: number;
  selectedSortProperty: string;
  selectedSortIsAscending: boolean = true;
  isFiltered: boolean = false;

  private modalRef: NgbModalRef;

  private _showOnlyStocked: boolean;
  get showOnlyStocked() {
    return this._showOnlyStocked;
  }
  set showOnlyStocked(value: boolean) {
    if (this.showOnlyStocked != value)
    {
      this._showOnlyStocked = value;
      this.loadProducts(); // Refresh list if filter changed
    }
  }

  @ViewChild('editProductModal', {static: true}) editModalContent;

  constructor(private productService: ProductService,
              private modalService: NgbModal,
              private route: ActivatedRoute) {}

  ngOnInit() {

    // URL param for filtering to stocked products
    if (this.route.snapshot.params['stocked'] != null)
      this.showOnlyStocked = true;

    // URL param for opening a product for editing
    let editParam = this.route.snapshot.params['edit'];
    if (!isNaN(editParam)) {
      this.selectedProductId = editParam;
      this.openModal(this.editModalContent);
    }
  }

  loadProducts() {
    this.loadingState = LoadingState.Waiting;
    this.productService.getProducts(this.state.toQueryParams(), this.showOnlyStocked)
      .subscribe(
      res => {
        Object.assign(this.state, res);
        this.loadingState = LoadingState.Success;
      },
      (err) => {
        this.loadingState = LoadingState.Error;
      });
  }

  setForeignCurrency(currency: SelectedCurrency)
  {
    this.selectedForeignCurrency = currency.code;
    this.selectedForeignCurrencyRate = currency.rate;
  }

  setPage(pageIndex: number)
  {
    if(pageIndex == this.state.pageIndex
      || pageIndex > this.state.pageCount
      || pageIndex < 1)
      return;

    this.state.pageIndex = pageIndex;
    this.loadProducts();
  }

  onProductCreated()
  {
    this.loadProducts();
  }

  // Update changed product in data set.
  onProductUpdated(product: Product)
  {
    let index = this.state.results.findIndex(p => p.id == product.id);
    this.state.results[index] = product;
  }

  // Remove deleted product from data set.
  onProductDeleted()
  {
    // Refactored to simple reload; removing entries was error-prone
    this.loadProducts();
  }

  openModal(modalContent)
  {
    this.modalRef = this.modalService.open(modalContent, {
      centered: true,
      keyboard: false,
      backdrop: 'static'
    });
  }

  closeModal()
  {
    this.modalRef.close();
  }

  toggleSort(property: string)
  {
    let sortState = this.state.getSortStateFor(property);

    switch (sortState) {
      case null:
        this.state.clearSorts();
      case SortState.Off:
        this.state.addSort(property);
        this.selectedSortIsAscending = true;
        this.selectedSortProperty = property;
        break;
      case SortState.Asc:
        this.state.addSort(property, true);
        this.selectedSortIsAscending = false;
        break;
      case SortState.Desc:
        this.state.clearSorts();
        this.selectedSortProperty = null;
        break;
    }

    // Switching sort resets page
    this.state.pageIndex = 1;
    this.loadProducts();
  }

  search(value: string) {

    let searchProp = "Name";
    let exactMatch = false;

    if (value == null || this.state.isFilterSet(searchProp, value, exactMatch))
      return;

    if (value == '') {
      this.state.clearFilters();
      this.isFiltered = false;
    } else {
      this.state.addFilter(searchProp, value, exactMatch);
      this.isFiltered = true;
    }

    this.loadProducts();
  }

  setPageSize(pageSize: number) {
    if (pageSize <= 0)
      return;

    this.state.pageSize = pageSize;
    this.state.pageIndex = Math.trunc(this.state.firstRowOnPage / pageSize) + 1 || 1;
    this.loadProducts();
  }
}
