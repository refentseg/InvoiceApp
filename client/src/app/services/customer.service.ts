import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Customer, CustomerParams } from '../models/customer';
import { map, Observable } from 'rxjs';
import { MetaData, Paginatedresponse } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private baseUrl: string = 'http://localhost:5000/api'
  constructor(private http: HttpClient) { }

  getCustomers(customerParams:CustomerParams):Observable<Paginatedresponse<Customer[]>>{
    let params = new HttpParams()
      .set('OrderBy', customerParams.orderBy)
      .set('PageNumber', customerParams.pageNumber.toString())
      .set('PageSize', customerParams.pageSize.toString());

    if (customerParams.searchTerm) {
      params = params.set('SearchTerm', customerParams.searchTerm);
    }
    return this.http.get<Customer[]>(`${this.baseUrl}/customer`, { observe: 'response', params })
      .pipe(
        map(response =>{
          const paginationMetaData: MetaData = JSON.parse(response.headers.get('Pagination')!);
          return new Paginatedresponse<Customer[]>(response.body!, paginationMetaData);
        })
      )
  }
  getCustomerById(id: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/customer/${id}`);
  }
  createCustomer(customer:Customer):Observable<any>{
    return this.http.post<any>(`${this.baseUrl}/customer`,customer);
  }
  updateCustomer(customer:Customer):Observable<any>{
    return this.http.put<any>(`${this.baseUrl}/customer/${customer.id}`,customer)
  }
  deleteCustomer(id:string){
    return this.http.delete<any>(`${this.baseUrl}/customer/${id}`)
  }
}
