import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../enviorment';
import { Observable } from 'rxjs';
import {
    CahngeStatus,
  CreatedSampleRequest,
  PagedResult,
  SampledListItem,
} from '../models/model';

@Injectable({providedIn:'root'})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/samples`;

  list(
    page: number,
    pageSize: number,
  ): Observable<PagedResult<SampledListItem>> {
    return this.http.get<PagedResult<SampledListItem>>(this.url, {
      params: { page, pageSize },
    });
  }

  create(sample: any): Observable<any> {
    return this.http.post(this.url, sample);
  }

  changeStatus(id:number, change: CahngeStatus): Observable<any> {
    return this.http.patch(`${this.url}/${id}/status`, change)
  }


}