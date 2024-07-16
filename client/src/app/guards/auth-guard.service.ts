import { inject, } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';


export const AuthGuardService : CanActivateFn = (route, state) => {

  let isauthenticated = inject(AuthService).isAuthenticated()
 let router = inject(Router)

     if (isauthenticated) {
      return true;
    } else {
      router.navigate(['/login']);
      return false;
    }
}
