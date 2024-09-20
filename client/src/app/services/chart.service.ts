import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private baseUrl: string = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getTotalSum(): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/total-sum`);
  }

  getTotalSumPerMonth(): Observable<{ [key: string]: number }> {
    return this.http.get<{ [key: string]: number }>(`${this.baseUrl}/charts/total-sum-per-month`);
  }

  getSumByStatus(): Observable<{ [key: string]: number }> {
    return this.http.get<{ [key: string]: number }>(`${this.baseUrl}/charts/sum-by-status`);
  }
}
