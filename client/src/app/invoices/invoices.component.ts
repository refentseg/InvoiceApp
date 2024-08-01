import { Component, inject, OnInit } from '@angular/core';
import { InvoicesService } from '../services/invoices.service';
import { Invoice, InvoiceItem, InvoiceParams } from '../models/invoice';
import { FormControl, FormsModule } from '@angular/forms';
import { AsyncPipe, CommonModule, NgFor } from '@angular/common';
import { map, Observable } from 'rxjs';
import { ButtonComponent } from '../components/button/button.component';
import { MetaData } from '../models/pagination';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptorService } from '../auth/jwt-interceptor.service';
import { AuthService } from '../auth/auth.service';
import { Router, RouterModule } from '@angular/router';
import { currencyFormat } from '../util/util';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { LucideAngularModule } from 'lucide-angular';

declare module 'jspdf' {
  interface jsPDF {
    autoTable: (options: any) => void;
    lastAutoTable: { finalY: number };
  }
}
@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule,AsyncPipe,ButtonComponent,RouterModule,FormsModule,LucideAngularModule],
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
    pageSize: 5
  };
  paginationMetaData!: MetaData;


  constructor(
    private invoiceService: InvoicesService,
    private router : Router,
    private dialog: MatDialog,
    private toastr: ToastrService
  ) {}

  ngOnInit():void {
    const userString = localStorage.getItem('user');
  if (userString) {
    const user = JSON.parse(userString);
    // console.log('Token from localStorage:', user.token);
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

  searchInvoices() {
    // Reset pagination to first page when searching
    this.invoiceParams.pageNumber = 1;
    this.loadInvoices();
  }

  onSearchTermChange(term: string): void {
    this.invoiceParams.searchTerm = term;
    this.loadInvoices();
  }

  onPageChange(pageNumber: number) {
    this.invoiceParams.pageNumber = pageNumber;
    this.loadInvoices();
  }

  getPageNumbers(): number[] {
    const totalPages = this.paginationMetaData.totalPages;
    const pageNumbers: number[] = [];
    for (let i = 1; i <= totalPages; i++) {
      pageNumbers.push(i);
    }
    return pageNumbers;
  }

  editInvoice(id: string) {
    this.router.navigate(['/invoices', id, 'edit']);
  }

  deleteInvoice(id:string){
    const dialogRef = this.dialog.open(ConfirmDialogComponent);
    dialogRef.afterClosed().subscribe(result =>{
      if(result){
        this.invoiceService.deleteInvoice(id).subscribe(
          () =>{
            this.toastr.success('Deletion successfull')
          },
          error =>{
            this.toastr.error('Cant delete invoice')
            console.log(error);
          }
        )
      }
    })

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

  currencyFormat(amount: number): string {
    return currencyFormat(amount);
  }

  generatePDF(invoice: Invoice) {
    const doc = new jsPDF();

    const pageWidth = doc.internal.pageSize.width;

    // Company Name
    // doc.addImage(headerImageData, 'PNG', pageWidth - 10, 0, 60, 30);

    // Invoice Header
    doc.setFontSize(18);
    doc.text(`Invoice`, pageWidth / 2, 20, { align: 'center' });

    // Invoice Details
    doc.setFontSize(11);
    doc.text(`# ${invoice.id}`, 10, 30);
    doc.text(`Date: ${this.formatInvoiceDate(invoice.orderDate)}`, pageWidth - 10, 30, { align: 'right' });
    doc.text(`Customer: ${invoice.customer.fullName}`, 10, 40);
    doc.text(`Company: ${invoice.customer.company}`, 10, 45);
    doc.text(`Email: ${invoice.customer.email}`, 10, 50);
    doc.text(`Phone: ${invoice.customer.phone}`, 10, 55);
    // doc.text(`Address: ${invoice.customer.address}`, 10, 45);

    // Invoice Items Table
    doc.autoTable({
      startY: 65,
      head: [['Item', 'Quantity', 'Amount']],
      body: invoice.items.map((item: InvoiceItem) => [
        item.name,
        item.quantity.toString(),
        currencyFormat(item.amount)
      ]),
      styles: { fontSize: 11 },
      headStyles: { fillColor: [103, 58, 183], textColor: [255, 255, 255], fontStyle: 'bold' },
      columnStyles: {
        0: { cellWidth: 'auto' },
        1: { cellWidth: 40 },
        2: { cellWidth: 45 }
      },
      margin: { left: 10, right: 10 },
      tableWidth: 'auto'
    });

    // Invoice Totals
    const finalY = (doc as any).lastAutoTable.finalY + 10;
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');

    const rightAlign = pageWidth - 10;
    doc.text('Subtotal:', rightAlign - 50, finalY);
    doc.text(currencyFormat(invoice.subtotal), rightAlign, finalY, { align: 'right' });

    doc.text('VAT:', rightAlign - 50, finalY + 10);
    doc.text(currencyFormat(invoice.vat), rightAlign, finalY + 10, { align: 'right' });

    doc.setFontSize(12);
    doc.text('Total:', rightAlign - 50, finalY + 20);
    doc.text(currencyFormat(invoice.total), rightAlign, finalY + 20, { align: 'right' });

    // Save the PDF with dynamic filename
    const filename = `#${invoice.id}.pdf`;
    doc.save(filename);
  }

}
