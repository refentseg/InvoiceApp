import { Component, inject, OnInit } from '@angular/core';
import { InvoicesService } from '../services/invoices.service';
import { Invoice, InvoiceParams } from '../models/invoice';
import { FormControl } from '@angular/forms';
import { AsyncPipe, CommonModule, NgFor } from '@angular/common';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule,AsyncPipe],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.css'
})
export class InvoicesComponent implements OnInit{

  invoices$!: Observable<Invoice[]>;
  totalCount: number = 0;
  currentPage: number = 1;
  pageSize: number = 10;
  searchControl:string = '';

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  invoicesService = inject(InvoicesService)

  ngOnInit():void {
    this.invoices$ =this.invoicesService.getInvoices();
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


  // loadInvoices() {
  //   const params: InvoiceParams = {
  //     pageNumber: this.currentPage,
  //     pageSize: this.pageSize,
  //     orderBy: 'orderDate',
  //     searchTerm: this.searchControl
  //   };

  //   this.invoicesService.getInvoices(params);
  // }

  // loadPage(page: number) {
  //   if (page >= 1 && page <= this.totalPages) {
  //     this.currentPage = page;
  //     this.loadInvoices();
  //   }
  // }

  // getPages(): number[] {
  //   const totalPages = this.totalPages;
  //   const currentPage = this.currentPage;
  //   const pages: number[] = [];

  //   for (let i = Math.max(1, currentPage - 2); i <= Math.min(totalPages, currentPage + 2); i++) {
  //     pages.push(i);
  //   }

  //   return pages;
  // }

}
