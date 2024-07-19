import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private baseUrl: string = 'http://localhost:5000/api/charts'
  constructor(private http: HttpClient) { }

  getTotalSum(): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/total-sum`);
  }

  getTotalSumPerMonth(): Observable<{ [key: string]: number }> {
    return this.http.get<{ [key: string]: number }>(`${this.baseUrl}/total-sum-per-month`);
  }

  getSumByStatus(): Observable<{ [key: string]: number }> {
    return this.http.get<{ [key: string]: number }>(`${this.baseUrl}/sum-by-status`);
  }
}
