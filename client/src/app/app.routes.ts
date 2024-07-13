import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { InvoicesComponent } from './invoices/invoices.component';

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
];
