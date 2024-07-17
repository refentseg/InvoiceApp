import { Component } from '@angular/core';
import { Invoice, UpdateInvoice } from '../../../models/invoice';
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { InputComponent } from '../../input/input.component';
import { ToastrService } from 'ngx-toastr';
import { InvoicesService } from '../../../services/invoices.service';
import { currencyFormat } from '../../../util/util';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';


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
  imports: [CommonModule, ReactiveFormsModule,InputComponent,FormsModule,RouterModule],
  templateUrl: './invoice-form.component.html',
  styleUrl: './invoice-form.component.css'
})
export class InvoiceFormComponent {
  mode: 'create' | 'edit' = 'create';
  invoice: CreateInvoice | null = null;
  invoiceForm: FormGroup = new FormGroup({});
  currentInvoiceId: string | null = null;
  invoiceStatuses = ['Pending', 'Approved','Sent','Rejected','DepositPaid', 'Completed', 'Paid', 'Overdue'];
  isSubmitting = false;

  constructor(private fb: FormBuilder, private invoicesService:InvoicesService, private toastr: ToastrService,private router: Router, private route: ActivatedRoute)
  {
    this.invoiceForm = this.fb.group({
      existingCustomer: [true],
      customerId: [''],
      customer: this.fb.group({
        fullName: [''],
        company: [''],
        email: ['', Validators.email],
        phone: ['']
      }),
      items: this.fb.array([]),
      status: ['Pending', Validators.required]
    });
  }

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
      items: this.fb.array([this.createItem()]),
      status: ['Pending']
    });

    this.onExistingCustomerChange();

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.mode = 'edit';
        this.invoicesService.getInvoiceById(id).subscribe(
          invoice => this.setInvoiceForEdit(invoice),
        );
      }
    });
  }


  createItem(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      amount: [0, [Validators.required, Validators.min(0)]]
    });
  }

  getItemAmount(index: number): number {
    return this.items.at(index).get('amount')?.value || 0;
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
  calculateVat(): number {
    const Vat =(this.calculateSubtotal()*0.15)
    return Vat;
  }

  calculateTotal(): number {
      const Total =(this.calculateSubtotal() + this.calculateVat());
      return Total;
  }
getAmount(index: number): FormControl {
    return this.items.at(index).get('amount') as FormControl;
  }
  currencyFormat(amount: number): string {
    return currencyFormat(amount);
  }

  onSubmit() {
    if (this.invoiceForm.valid) {
      this.isSubmitting = true;
      const formValue = this.invoiceForm.value;
      const invoice: CreateInvoice = {
        existingCustomer: formValue.existingCustomer,
        customerId: formValue.existingCustomer ? formValue.customerId : undefined,
        customer: formValue.existingCustomer ? undefined : formValue.customer,
        items: formValue.items
      };
      const updateinvoice: UpdateInvoice = {
        id: this.currentInvoiceId!,  // Include the id for update
        existingCustomer: formValue.existingCustomer,
        customerId: formValue.existingCustomer ? formValue.customerId : undefined,
        customer: formValue.existingCustomer ? undefined : formValue.customer,
        items: formValue.items,
        invoiceStatus: formValue.status
      };

      const action = this.mode === 'create'
      ? this.invoicesService.createInvoice(invoice as CreateInvoice)
      : this.invoicesService.updateInvoice(updateinvoice as UpdateInvoice);

      action.pipe(
        finalize(() => this.isSubmitting = false)
      ).subscribe(
        response => {
          console.log('Invoice response:', response);
          const message = this.mode === 'create' ? 'Invoice created successfully' : 'Invoice updated successfully';
          this.toastr.success(message);
          this.resetForm();
          this.router.navigate(['/invoices']);
        },
        error =>{
          console.error(`Error ${this.mode === 'create' ? 'creating' : 'updating'} invoice:`, error);
        }
      );
    }
  }

  setInvoiceForEdit(invoice: UpdateInvoice) {
    this.mode = 'edit';
    this.currentInvoiceId = invoice.id;
    this.invoiceForm.patchValue({
      existingCustomer: invoice.existingCustomer,
      customerId: invoice.customerId,
      customer: invoice.customer,
      items: invoice.items,
      status:invoice.invoiceStatus
    });
    this.items.clear();
    invoice.items.forEach(item => this.items.push(this.fb.group({
    name: [item.name, Validators.required],
    quantity: [item.quantity, [Validators.required, Validators.min(1)]],
    amount: [item.amount, [Validators.required, Validators.min(0)]]
  })));
  }

  resetForm() {
    this.mode = 'create';
    this.currentInvoiceId = null;
    this.invoiceForm.reset();
  }


}
