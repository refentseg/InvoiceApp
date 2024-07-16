import { Component, inject, OnInit } from '@angular/core';
import { InvoicesService } from '../services/invoices.service';
import { Invoice, InvoiceParams } from '../models/invoice';
import { FormControl } from '@angular/forms';
import { AsyncPipe, CommonModule, NgFor } from '@angular/common';
import { map, Observable } from 'rxjs';
import { ButtonComponent } from '../components/button/button.component';
import { MetaData } from '../models/pagination';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptorService } from '../auth/jwt-interceptor.service';
import { AuthService } from '../auth/auth.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule,AsyncPipe,ButtonComponent,RouterModule],
  templateUrl: './invoices.component.html',
  providers:[],
  styleUrl: './invoices.component.css'
})
export class InvoicesComponent implements OnInit{

  invoices$!: Observable<Invoice[]>;
  invoiceParams: InvoiceParams = {
    orderBy: 'orderDate',
    searchTerm: '',
    pageNumber: 1,
    pageSize: 10
  };
  paginationMetaData!: MetaData;


  constructor(
    private invoiceService: InvoicesService,
    private authService: AuthService
  ) {}

  ngOnInit():void {
    const userString = localStorage.getItem('user');
  if (userString) {
    const user = JSON.parse(userString);
    console.log('Token from localStorage:', user.token);
    this.loadInvoices();
  } else {
    console.log('No user found in localStorage');
  }


  }

  loadInvoices() {
    this.invoices$ = this.invoiceService.getInvoices(this.invoiceParams).pipe(
      map(response => {
        this.paginationMetaData = response.metaData;
        return response.items;
      })
    );
  }


  formatInvoiceDate(dateString: string | Date): string {
    const dateObject = dateString instanceof Date ? dateString : new Date(dateString);

    // Check if dateObject is a valid date
    if (isNaN(dateObject.getTime())) {
      // Handle invalid date
      return "Invalid Date";
    }

    const options: Intl.DateTimeFormatOptions = { day: 'numeric', month: 'short', year: 'numeric' };
    const formattedDate = Intl.DateTimeFormat('en-ZA', options).format(dateObject);

    return formattedDate;
  }




}
