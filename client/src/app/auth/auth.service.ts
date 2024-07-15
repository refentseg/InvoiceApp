import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl: string = 'http://localhost:5000/api'



  isLoggedIn: boolean = false;
  private _user : BehaviorSubject<User | null>;
  public user$? :Observable<User |null>;

  constructor(private http: HttpClient) {
    this._user = new BehaviorSubject<User | null>(JSON.parse(localStorage.getItem('user') || 'null'))
    this.user$ = this._user.asObservable();
  }

  public get currentUserValue(): User | null {
    return this._user.value;
  }

  login(userDetails: { username: string; password: string }): Observable<boolean> {
    return this.http.post<any>('http://localhost:5000/api/account/login', userDetails)
      .pipe(
        map(user => {
          localStorage.setItem('user',JSON.stringify(user));
          this._user.next(user);
          this.isLoggedIn =true
          return true;
        }),
        catchError(error => {
          console.log(error);
          this.isLoggedIn = false;
          return of(false);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('user');
    this._user.next(null);
    this.isLoggedIn = false;
  }

  isAuthenticated(): boolean {
    return this.isLoggedIn;
  }
}
