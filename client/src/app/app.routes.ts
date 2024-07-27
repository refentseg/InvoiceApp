import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { PageNotFoundComponent } from './Errors/page-not-found/page-not-found.component';
import { CustomerComponent } from './customer/customer.component';
import { LoginComponent } from './auth/login/login.component';
import { AuthGuardService } from './guards/auth-guard.service';
import { InvoiceCreateComponent } from './invoices/create/create.component';
import { InvoiceEditComponent } from './invoices/edit/edit.component';
import { CustomerCreateComponent } from './customer/create/create.component';
import { CustomerEditComponent } from './customer/edit/edit.component';

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
    path:'customer/create',
    component: CustomerCreateComponent,
    title:'Invoice App | Create Customers',
    canActivate: [AuthGuardService]
  },
  {
    path:'customer/:id/edit',
    component: CustomerEditComponent,
    title:'Invoice App | Edit Customers',
    canActivate: [AuthGuardService]
  },
  {
    path:'invoice/create',
    component: InvoiceCreateComponent,
    title:'Invoice App | Create Invoice',
    canActivate: [AuthGuardService]
  },
  {
    path:'invoices/:id/edit',
    component: InvoiceEditComponent,
    title:'Invoice App | Edit Invoice',
    canActivate: [AuthGuardService]
  },
  {
    path:'login',
    component: LoginComponent,
    title:'Invoice App | Login', outlet: 'auth'
  },
  {
    path:'register',
    component: LoginComponent,
    title:'Invoice App | Register'
  },
  { path: '**', component: PageNotFoundComponent, outlet: 'auth' }
];
