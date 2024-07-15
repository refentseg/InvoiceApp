import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class JwtInterceptorService implements HttpInterceptor{

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const currentUser = this.authService.currentUserValue;

    if(currentUser){
      const authReq = req.clone({
        setHeaders:{
          Authorization: `Bearer ${currentUser.token}`
        }
      });
      return next.handle(authReq);
    }else{
      return next.handle(req)
    }
  }
}
