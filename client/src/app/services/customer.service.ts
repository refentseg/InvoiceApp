import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Customer, CustomerParams } from '../models/customer';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private baseUrl: string = 'http://localhost:5000/api'
  constructor(private http: HttpClient) { }

  getCustomers(customerParams:CustomerParams):Observable<Customer[]>{
    let params = new HttpParams()
    .set('OrderBy', customerParams.orderBy)

    if (customerParams.searchTerm) {
      params = params.set('SearchTerm', customerParams.searchTerm);
    }
    return this.http.get<Customer[]>(`${this.baseUrl}/customer`, { observe: 'response', params })
      .pipe(
        map(response =>{
          return response.body || []
        })
      )
  }
}
