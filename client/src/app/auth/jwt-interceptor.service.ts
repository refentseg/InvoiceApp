import { inject, Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { HttpErrorResponse, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable()
export class JwtInterceptorService implements HttpInterceptor{

  constructor() {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const user = localStorage.getItem('user');
    let authReq = req;
    if(user){
      const parsedToken = JSON.parse(user);
      authReq = req.clone({
        headers: req.headers.set(
          'Authorization',`Bearer ${parsedToken.token}`
        )
      });
    }
    return next.handle(authReq);

  }
}
