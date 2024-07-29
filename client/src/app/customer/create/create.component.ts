import { Component } from '@angular/core';
import { CustomerFormComponent } from '../../components/Forms/customer-form/customer-form.component';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CustomerFormComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.css'
})
export class CustomerCreateComponent {

}
