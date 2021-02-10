import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Observable, of} from 'rxjs';
import {catchError, debounceTime, distinctUntilChanged, map, tap, switchMap, finalize} from 'rxjs/operators';
import {PartnerService} from '../../services/partner.service';
import {PartnerListing} from '../../models/partner-listing';
import {HttpParams} from '@angular/common/http';
import { PagedQuery } from 'app/core/http/models/paged-query';
import { faBook, faCircleNotch } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-partner-lookup-field',
  templateUrl: './partner-lookup-field.component.html'
})

export class PartnerLookupFieldComponent implements OnInit {

  _model: PartnerListing;
  get model(): PartnerListing { return this._model; }
  set model(value: PartnerListing) {
    if (value.name == undefined)
      return; // Don't save typed in strings as model

    this._model = value;
    this.partnerSelected.emit(value);

    if (this.resetAfterSelection)
      this.reset();
  }

  get searching() { return this.state == 'searching'; }
  get searchFailed() { return this.state == 'searchFailed'; }
  get noResults() { return this.state == 'noResults'; }
  
  spinnerIcon = faCircleNotch;
  readyIcon = faBook;
  inputValue: string;

  @Input() disabled: boolean = false;
  @Input() drawFocus: boolean = false;
  @Input() resetAfterSelection: boolean = false;
  @Input() set resultCount(count: number) { this.query.pageSize = count; }
  @Output() partnerSelected = new EventEmitter<PartnerListing>();

  private query: PagedQuery = new PagedQuery();
  private state: ('ready' | 'searching' | 'searchFailed' | 'noResults') = 'ready';

  ngOnInit() {
  }

  constructor(
    private _service: PartnerService) {}

  public reset = () => {
    setTimeout(() => {
      this.inputValue = '';
      this._model = null;
      this.state = 'ready';
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
        this._service.getAll(this.getParams(term)).pipe(
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

  formatter = (x: {name: string}) => x.name;
}
