import { Component } from '@angular/core';
import { Customer } from '../../../models/customer';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CustomerService } from '../../../services/customer.service';
import { CommonModule } from '@angular/common';
import { InputComponent } from '../../input/input.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,InputComponent,FormsModule,RouterModule],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.css'
})
export class CustomerFormComponent {
  mode: 'create' | 'edit' = 'create';
  customer: Customer | null = null;
  customerForm: FormGroup = new FormGroup({});
  currentCustomerId:number | null= null;
  isSubmitting = false;

  constructor(private fb: FormBuilder, private customerService:CustomerService, private toastr: ToastrService,private router: Router, private route: ActivatedRoute) {
    this.customerForm = this.fb.group({
      fullName: [''],
        company: [''],
        email: ['', Validators.email],
        phone: ['']
    })
  }

  setCustomerForEdit(customer:Customer){
    this.mode = 'edit';
    this.currentCustomerId = customer.id!;
    this.customerForm.patchValue({
      fullName:customer.fullName,
      company:customer.company,
      email:customer.email,
      phone:customer.phone
    })
  }

  resetForm() {
    this.mode = 'create';
    this.currentCustomerId = null;
    this.customerForm.reset();
  }

  ngOnInit(){
    this.customerForm = this.fb.group({
      fullName: [''],
        company: [''],
        email: ['', Validators.email],
        phone: ['']
    });

    this.route.paramMap.subscribe(params =>{
      const id = params.get('id')
      if(id){
        this.mode = 'edit';
        this.customerService.getCustomerById(id).subscribe(
          customer => this.setCustomerForEdit(customer)
        );
      }
    });
  }

  onSubmit(){
    if(this.customerForm.valid){
      this.isSubmitting = true;

      const formValue = this.customerForm.value;

      const createCustomer:Customer ={
        fullName:formValue.fullName,
        company:formValue.company,
        email:formValue.email,
        phone:formValue.phone
      };

      const updateCustomer:Customer ={
        id: this.currentCustomerId!,
        fullName:formValue.fullName,
        company:formValue.company,
        email:formValue.email,
        phone:formValue.phone
      };

      const action = this.mode === 'create'
      ? this.customerService.createCustomer(createCustomer)
      : this.customerService.updateCustomer(updateCustomer)

      action.pipe(
        finalize(() => this.isSubmitting = false)
      ).subscribe(
        (response) => {
          console.log('Customer response:', response.customerId);
          const message = this.mode === 'create' ? 'Customer created successfully' : 'Customer updated successfully';
          this.toastr.success(message);
          this.resetForm();
          this.router.navigate(['/customers']);
        },
        error =>{
          console.error(`Error ${this.mode === 'create' ? 'creating' : 'updating'} invoice:`, error);

          let errorMessage = `An error occurred while ${this.mode === 'create' ? 'creating' : 'updating'} the customer.`;

          if (error.status === 400) {
              errorMessage = 'Invalid data submitted. Please check your inputs and try again.';
          } else if (error.status === 401) {
            errorMessage = 'You are not authorized to perform this action. Please log in and try again.';
            this.router.navigate(['/login']);
          } else if (error.status === 403) {
            errorMessage = 'You do not have permission to perform this action.';
          } else if (error.status === 404) {
            errorMessage = 'The requested resource was not found. Please refresh and try again.';
          } else if (error.status === 500) {
            errorMessage = 'A server error occurred. Please try again later or contact support.';
          }

          this.toastr.error(errorMessage);
        }
      )

    }
  }
}
