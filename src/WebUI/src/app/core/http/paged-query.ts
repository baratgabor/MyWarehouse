import {HttpParams} from '@angular/common/http';

export class PagedQuery {
  private sortList = new Map<string, SortState>();
  private filterList = new Array<SortRecord>();

  pageIndex: number = 1;
  pageSize: number = 10;

  public addFilter(propertyPath: string, value: string, exactMatch = false, addToExisting = false) {
    if (!addToExisting)
      this.filterList.length = 0;

    this.filterList.push({ prop: propertyPath, value, exactMatch });
  }

  public clearFilters() {
    this.filterList.length = 0;
  }

  public addSort(propertyPath: string, descending = false, addToExisting = false) {
    if (!addToExisting)
      this.sortList.clear();

    this.sortList.set(propertyPath, descending ? SortState.Desc : SortState.Asc);
  }

  public clearSorts() {
    this.sortList.clear();
  }

  public getSortStateFor(propertyPath: string) : SortState {
    return this.sortList.has(propertyPath)
      ? this.sortList.get(propertyPath)
      : SortState.Off;
  }

  public isFilterSet(propertyPath: string, value: string, exactMatch: boolean) {
    return this.filterList.indexOf({ prop: propertyPath, value, exactMatch }) > -1;
  }

  // Convert state into params for new HTTP requests
  public toQueryParams() : HttpParams {
    let res = new HttpParams();

    res = res.append('pageIndex', this.pageIndex.toString());
    res = res.append('pageSize', this.pageSize.toString());

    let orderBy = "";
    this.sortList.forEach((sort, prop) => orderBy += `${prop} ${sort}, `);
    orderBy = orderBy.slice(0, -2);

    let filter = this.filterList.reduce((acc, filter, i) => acc
      + (i > 0 ? " and " : "")
      + (filter.exactMatch ? `${filter.prop} eq ${filter.value}` : `substringof('${filter.value}', ${filter.prop})`), "");

    if (orderBy)
      res = res.append('orderBy', orderBy);

    if (filter)
      res = res.append('filter', filter);

    return res;
  }
}

export enum SortState {
  Off = "-", 
  Asc = "asc",
  Desc = "desc"
}

interface SortRecord {
  prop: string; value: string; exactMatch: boolean;
}
