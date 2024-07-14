import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Invoice, InvoiceParams } from '../models/invoice';
import { map, Observable } from 'rxjs';
import { MetaData, Paginatedresponse } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {
  private baseUrl: string = 'http://localhost:5000/api'

  constructor(private http: HttpClient) { }

  getInvoices(invoiceParams: InvoiceParams): Observable<Paginatedresponse<Invoice[]>> {

    let params = new HttpParams()
      .set('OrderBy', invoiceParams.orderBy)
      .set('PageNumber', invoiceParams.pageNumber.toString())
      .set('PageSize', invoiceParams.pageSize.toString());

    if (invoiceParams.searchTerm) {
      params = params.set('SearchTerm', invoiceParams.searchTerm);
    }

    return this.http.get<Invoice[]>(`${this.baseUrl}/invoices`, { observe: 'response', params })
      .pipe(
        map(response => {
          const paginationMetaData: MetaData = JSON.parse(response.headers.get('Pagination')!);
          return new Paginatedresponse<Invoice[]>(response.body!, paginationMetaData);
        })
      );
  }
  trackByInvoiceId(index: number, invoice: Invoice): string {
    return invoice.id;
  }
}
