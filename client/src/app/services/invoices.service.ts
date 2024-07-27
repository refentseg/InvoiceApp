import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateInvoice, Invoice, InvoiceParams, UpdateInvoice } from '../models/invoice';
import { map, Observable } from 'rxjs';
import { MetaData, Paginatedresponse } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {
  private baseUrl: string = 'http://localhost:5000/api'

  constructor(private http: HttpClient) { }

  //Getting Invoices with Params
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
  getInvoiceById(id: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/invoices/${id}`);
  }
  createInvoice(invoice:CreateInvoice):Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/invoices`,invoice);
  }
  updateInvoice(invoice: UpdateInvoice): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/invoices/${invoice.id}`, invoice);
  }

  deleteInvoice(id:string){
    return this.http.delete<any>(`${this.baseUrl}/invoices/${id}`)
  }

}
