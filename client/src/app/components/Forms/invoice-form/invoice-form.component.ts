import { Component } from '@angular/core';
import { Invoice } from '../../../models/invoice';
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { InputComponent } from '../../input/input.component';
import { ToastrService } from 'ngx-toastr';

interface CreateInvoice {
  existingCustomer: boolean;
  customerId?: string;
  customer?: {
    fullName: string;
    company: string;
    email: string;
    phone: string;
  };
  items: {
    name: string;
    quantity: number;
    amount: number;
  }[];
}

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,InputComponent,FormsModule],
  templateUrl: './invoice-form.component.html',
  styleUrl: './invoice-form.component.css'
})
export class InvoiceFormComponent {
  mode: 'create' | 'edit' = 'create';
  invoice: CreateInvoice | null = null;
  invoiceForm: FormGroup = new FormGroup({});

  constructor(private fb: FormBuilder, private http: HttpClient, private toastr: ToastrService) {}

  onExistingCustomerChange() {
    const existingCustomer = this.invoiceForm.get('existingCustomer')!.value;
    if (existingCustomer) {
      this.invoiceForm.get('customerId')!.setValidators(Validators.required);
      this.invoiceForm.get('customer')!.disable();
    } else {
      this.invoiceForm.get('customerId')!.clearValidators();
      this.invoiceForm.get('customer')!.enable();
    }
    this.invoiceForm.get('customerId')!.updateValueAndValidity();
  }

  ngOnInit() {
    this.invoiceForm = this.fb.group({
      existingCustomer: [false],
      customerId: [''],
      customer: this.fb.group({
        fullName: ['', Validators.required],
        company: [''],
        email: ['', [Validators.required, Validators.email]],
        phone: ['', Validators.required]
      }),
      items: this.fb.array([this.createItem()])
    });

    this.onExistingCustomerChange();
  }

  createItem(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      amount: [0, [Validators.required, Validators.min(0)]]
    });
  }

  get items() {
    return this.invoiceForm.get('items') as FormArray;
  }

  addItem() {
    this.items.push(this.createItem());
  }

  removeItem(index: number) {
    this.items.removeAt(index);
  }
  calculateSubtotal(): number {
    return this.items.controls
      .reduce((sum, item) => sum + (item.get('quantity')?.value * item.get('amount')?.value), 0);
  }
getAmount(index: number): FormControl {
    return this.items.at(index).get('amount') as FormControl;
  }

  onSubmit() {
    if (this.invoiceForm.valid) {
      const formValue = this.invoiceForm.value;
      const invoice: CreateInvoice = {
        existingCustomer: formValue.existingCustomer,
        customerId: formValue.existingCustomer ? formValue.customerId : undefined,
        customer: formValue.existingCustomer ? undefined : formValue.customer,
        items: formValue.items
      };

      this.http.post<any>('http://localhost:5000/api/invoices', invoice).subscribe(
        response => {
          this.toastr.success('Invoice created successfully');
          console.log(response)
        },
        error => {
          this.toastr.error('Couldnt create invoice');
          console.error(error);

        }
      );
    }
  }


}
