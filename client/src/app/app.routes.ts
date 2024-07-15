import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { PageNotFoundComponent } from './Errors/page-not-found/page-not-found.component';
import { CustomerComponent } from './customer/customer.component';
import { LoginComponent } from './auth/login/login.component';
import { AuthGuardService } from './guards/auth-guard.service';

export const routes: Routes = [
  {
    path:'',
    component: HomeComponent,
    title:'Invoice App | Home Page',
    canActivate: [AuthGuardService]
  },
  {
    path:'invoices',
    component: InvoicesComponent,
    title:'Invoice App | Invoices',
    canActivate: [AuthGuardService]
  },
  {
    path:'customers',
    component: CustomerComponent,
    title:'Invoice App | Customers',
    canActivate: [AuthGuardService]
  },
  {
    path:'login',
    component: LoginComponent,
    title:'Invoice App | Login'
  },
  {
    path:'register',
    component: LoginComponent,
    title:'Invoice App | Register'
  },
  { path: '**', component: PageNotFoundComponent }
];
