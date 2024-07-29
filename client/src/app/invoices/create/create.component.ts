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
}
