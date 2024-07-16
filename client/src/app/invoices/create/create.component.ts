import { Component } from '@angular/core';
import { Route } from '@angular/router';
import { InvoiceFormComponent } from '../../components/Forms/invoice-form/invoice-form.component';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [InvoiceFormComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.css'
})
export class InvoiceCreateComponent {

  constructor() {}
  onInvoiceCreated(invoiceId: string) {
    console.log('Invoice created with ID:', invoiceId);
    // Navigate to the invoice list or the newly created invoice details
    //this.router.navigate(['/invoices', invoiceId]);
  }
}
