import {PagedQuery} from './paged-query';

export class PagedState<T> extends PagedQuery {

  pageCount: number;
  rowCount: number;

  firstRowOnPage: number;
  lastRowOnPage: number;

  results: T[];
}
