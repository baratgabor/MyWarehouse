import {Component, OnInit, ViewChild } from '@angular/core';
import {PagedState} from 'app/core/http/models/paged-state';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ActivatedRoute} from '@angular/router';
import {TransactionDetails} from '../../../viewExisting/models/transaction-details';
import {TransactionService} from '../../../../common/services/transaction.service';
import { TransactionType } from '../../../../common/models/transaction-type';
import { SortState } from '../../../../../core/http/models/paged-query';
import { faCalendarAlt, faCoins, faIdCard, faMinus, faPlus, faSearch, faSort, faSortDown, faSortUp, faSpinner, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { SelectedCurrency } from 'app/core/exchangeRates/components/currency-selector/currency-selector.component';

enum LoadingState {
  Default,
  Waiting,
  Error,
  Success
}

@Component({
  selector: 'app-transaction-index',
  templateUrl: './transaction-index.component.html'
})
export class TransactionIndexComponent implements OnInit {

  selectedForeignCurrency : string;
  selectedForeignCurrencyRate : number;

  get ls() { return LoadingState; }
  get tt() { return TransactionType; }

  fa = {
    spinner: faSpinner,
    plus: faPlus,
    minus: faMinus,
    trash: faTrashAlt,
    calendar: faCalendarAlt,
    idCard: faIdCard,
    coins: faCoins,
    sort: faSort,
    sortUp: faSortUp,
    sortDown: faSortDown,
    search: faSearch
  };

  showOnlyProcurement = false;
  showOnlySales = false;
  showAllEntries = true;

  loadingState = LoadingState.Default;

  state: PagedState<TransactionDetails> = new PagedState<TransactionDetails>();

  selectedEntityId: number;
  selectedSortProperty: string;
  selectedSortIsAscending: boolean = true;
  isFiltered: boolean = false;

  private modalRef: NgbModalRef;

  @ViewChild('viewTransactionModal', {static: true}) editModalContent;

  constructor(private transactionService: TransactionService,
              private modalService: NgbModal,
              private route: ActivatedRoute) {}

  ngOnInit() {

    // URL param for opening an entity
    let viewParam = this.route.snapshot.params['view'];
    if (!isNaN(viewParam)) {
      this.selectedEntityId = viewParam;
      this.openModal(this.editModalContent);
    }
  }

  loadEntities() {
    
    this.loadingState = LoadingState.Waiting;
    this.transactionService.getAll(this.state.toQueryParams(),
      this.showOnlyProcurement ? TransactionType.Procurement : (this.showOnlySales ? TransactionType.Sale : null))
      .subscribe(
        res => {
          Object.assign(this.state, res);
          this.loadingState = LoadingState.Success;
        },
        () => {
          this.loadingState = LoadingState.Error;
        });
  }

  setPage(pageIndex: number)
  {
    if(pageIndex == this.state.pageIndex
      || pageIndex > this.state.pageCount
      || pageIndex < 1)
      return;

    this.state.pageIndex = pageIndex;
    this.loadEntities();
  }

  setForeignCurrency(currency: SelectedCurrency)
  {
    this.selectedForeignCurrency = currency.code;
    this.selectedForeignCurrencyRate = currency.rate;  
  }

  removeEntityFromDataSet()
  {
    // Refactored to simple reload; removing entries was buggy
    this.loadEntities();
  }

  onEntityCreated()
  {
    this.loadEntities();
  }

  updateEntityInDateSet(entity: TransactionDetails)
  {
    let index = this.state.results.findIndex(p => p.id == entity.id);
    this.state.results[index] = entity;
  }

  openModal(modalContent)
  {
    this.modalRef = this.modalService.open(modalContent, {
      keyboard: false,
      scrollable: true,
      backdrop: 'static',
      size: 'xl'
    });
  }

  toggleSort(property: string) {
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

    this.state.pageIndex = 1;
    this.loadEntities();
  }

  search(value: string) {

    let searchProp = "PartnerName";
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

    this.state.pageIndex = 1;
    this.loadEntities();
  }

  setPageSize(pageSize: number) {
    if (pageSize <= 0)
      return;
    
    this.state.pageSize = pageSize;
    this.state.pageIndex = Math.trunc(this.state.firstRowOnPage / pageSize) + 1 || 1;
    this.loadEntities();
  }

  setViewType(value: 'showOnlyProcurement' | 'showOnlySales' | 'showAllEntries')
  {
    if (this[value] == true)
      return;

    this.showOnlyProcurement = this.showOnlySales = this.showAllEntries = false;
    this[value] = true;

    this.state.pageIndex = 1;
    this.loadEntities();
  }
}
