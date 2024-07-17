import { Component } from '@angular/core';
import { InvoiceFormComponent } from '../../components/Forms/invoice-form/invoice-form.component';

@Component({
  selector: 'app-edit',
  standalone: true,
  imports: [InvoiceFormComponent],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.css'
})
export class InvoiceEditComponent {

}
