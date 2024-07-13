import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Invoice, InvoiceParams } from '../models/invoice';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {

  constructor(private http: HttpClient) { }

  private baseUrl: string = 'http://localhost:5000/api/'

  getInvoices(): Observable<Invoice[]> {
    // let httpParams = new HttpParams()
    //     .set('pageNumber', params.pageNumber.toString())
    //     .set('pageSize', params.pageSize.toString())
    //     .set('orderBy', params.orderBy)

    //     if (params.searchTerm) {
    //       httpParams = httpParams.set('searchTerm', params.searchTerm);
    //     }

    return this.http.get<Invoice[]>(`${this.baseUrl}invoices`,

    );
  }
}
