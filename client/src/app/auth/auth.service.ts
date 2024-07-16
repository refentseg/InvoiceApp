import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl: string = 'http://localhost:5000/api'
  private _user : BehaviorSubject<User | null>;
  public user$? :Observable<User |null>;

  constructor(private http: HttpClient) {
    this._user = new BehaviorSubject<User | null>(JSON.parse(localStorage.getItem('user') || 'null'))
    this.user$ = this._user.asObservable();
  }

  public get currentUserValue(): User | null {
    return this._user.value;
  }

  login(userDetails: { username: string; password: string }): Observable<User> {
    return this.http.post<any>('http://localhost:5000/api/account/login', userDetails)
      .pipe(
        map(user => {
          localStorage.setItem('user',JSON.stringify(user));
          this._user.next(user);
          return user;
        })
      );
  }

  logout(): void {
    localStorage.removeItem('user');
    this._user.next(null);
  }

  isAuthenticated(): boolean{
    const user = localStorage.getItem('user');
    return !!user;
  }
}
