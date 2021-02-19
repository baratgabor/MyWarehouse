import {Component, OnInit, ViewChild} from '@angular/core';
import {PagedState} from 'app/core/http/models/paged-state';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ActivatedRoute} from '@angular/router';
import {PartnerListing} from '../../models/partner-listing';
import {PartnerService} from '../../services/partner.service';
import { SortState } from '../../../core/http/models/paged-query';
import { faBarcode, faBuilding, faGlobe, faIdCard, faMapPin, faPencilAlt, faPlus, faSort, faSortDown, faSortUp, faSpinner, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { PartnerUpdate } from 'app/partners/models/partner-update';

enum LoadingState {
  Default,
  Waiting,
  Error,
  Success
}

@Component({
  selector: 'app-partners',
  templateUrl: './partner-index.component.html',
  styles: ['.p-3 { padding: 0.5rem 1rem!important; }']
})

export class PartnerIndexComponent implements OnInit {

  fa = {
    spinner: faSpinner,
    trash: faTrashAlt,
    plus: faPlus,
    idCard: faIdCard,
    globe: faGlobe,
    barCode: faBarcode,
    mapPin: faMapPin,
    building: faBuilding,
    pencil: faPencilAlt,
    sort: faSort,
    sortUp: faSortUp,
    sortDown: faSortDown
  };

  get ls() { return LoadingState; }
  loadingState = LoadingState.Default;

  state: PagedState<PartnerListing> = new PagedState<PartnerListing>();

  selectedEntityId: number;
  selectedSortProperty: string;
  selectedSortIsAscending: boolean = true;
  isFiltered: boolean = false;

  private modalRef: NgbModalRef;


  @ViewChild('editPartnerModal', {static: true}) editModalContent;

  constructor(private partnerService: PartnerService,
              private modalService: NgbModal,
              private route: ActivatedRoute) {}

  ngOnInit() {

    // URL param for opening an entity for editing
    let editParam = this.route.snapshot.params['edit'];
    if (!isNaN(editParam)) {
      this.selectedEntityId = editParam;
      this.openModal(this.editModalContent);
    }
  }

  loadEntities() {
    this.loadingState = LoadingState.Waiting;
    this.partnerService.getAll(this.state.toQueryParams())
      .subscribe(
      res => {
        Object.assign(this.state, res);
        this.loadingState = LoadingState.Success;
      },
      res => {
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

  removeEntityFromDataSet(entity: PartnerListing)
  {
    // Refactored to simple reload; removing entries was buggy
    this.loadEntities();
  }

  onEntityCreated(entity: PartnerListing)
  {
    this.loadEntities();
  }

  updateEntityInDateSet(entity: PartnerUpdate)
  {
    let index = this.state.results.findIndex(p => p.id == entity.id);
    Object.assign(this.state.results[index], entity);
    Object.assign(this.state.results[index], entity.address);
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

    // Switching sort resets page
    this.state.pageIndex = 1;
    this.loadEntities();
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

    this.loadEntities();
  }

  setPageSize(pageSize: number) {
    if (pageSize <= 0)
      return;

    this.state.pageSize = pageSize;
    this.state.pageIndex = Math.trunc(this.state.firstRowOnPage / pageSize) + 1 || 1;
    this.loadEntities();
  }
}
