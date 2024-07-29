import { Component } from '@angular/core';
import { CustomerFormComponent } from '../../components/Forms/customer-form/customer-form.component';

@Component({
  selector: 'app-edit',
  standalone: true,
  imports: [CustomerFormComponent],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.css'
})
export class CustomerEditComponent {

}
