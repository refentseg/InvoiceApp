import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { UsersComponent } from './users/users.component';
import { PageNotFoundComponent } from './Errors/page-not-found/page-not-found.component';
import { CustomerComponent } from './customer/customer.component';

export const routes: Routes = [
  {
    path:'',
    component: HomeComponent,
    title:'Invoice App | Home Page'
  },
  {
    path:'invoices',
    component: InvoicesComponent,
    title:'Invoice App | Invoices'
  },
  {
    path:'customers',
    component: CustomerComponent,
    title:'Invoice App | Customers'
  },
  { path: '**', component: PageNotFoundComponent }
];
