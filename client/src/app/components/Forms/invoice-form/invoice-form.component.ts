import { Component, OnInit } from '@angular/core';
import { Invoice, InvoiceItem, UpdateInvoice } from '../../../models/invoice';
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { InputComponent } from '../../input/input.component';
import { ToastrService } from 'ngx-toastr';
import { InvoicesService } from '../../../services/invoices.service';
import { currencyFormat } from '../../../util/util';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged, finalize, map, Observable, Subject, switchMap } from 'rxjs';
import { Customer, CustomerParams } from '../../../models/customer';
import { CustomerService } from '../../../services/customer.service';
import { MetaData } from '../../../models/pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgOptionHighlightModule } from '@ng-select/ng-option-highlight';


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

interface InvoiceItemForm {
  name: FormControl<string | null>;
  quantity: FormControl<number | null>;
  amount: FormControl<number | null>;
  id?: FormControl<number | null>;  // Make id optional
}

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,InputComponent,FormsModule,RouterModule,NgSelectModule,NgOptionHighlightModule],
  templateUrl: './invoice-form.component.html',
  styleUrl: './invoice-form.component.css'
})
export class InvoiceFormComponent implements OnInit{
  mode: 'create' | 'edit' = 'create';
  invoice: CreateInvoice | null = null;
  invoiceForm: FormGroup = new FormGroup({});
  currentInvoiceId: string | null = null;
  isSubmitting = false;
  loading = false;

  customers$!: Observable<Customer[]>;
  customerInput$ = new Subject<string>();
  customerParams: CustomerParams = {
    orderBy: 'name',
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  };
  paginationMetaData!: MetaData;

  constructor(private fb: FormBuilder, private invoicesService:InvoicesService,private customerService: CustomerService, private toastr: ToastrService,private router: Router, private route: ActivatedRoute)
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
      status: ['']
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
      status: ['']
    });

    this.onExistingCustomerChange();

    this.loadCustomers();

    this.customerInput$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => {
        this.customerParams.searchTerm = term;
        this.customerParams.pageNumber = 1;
        return this.loadCustomers();
      })
    ).subscribe();

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

  //Finding Customer
  loadCustomers() {
    this.loading = true;
    this.customers$ = this.customerService.getCustomers(this.customerParams).pipe(
      map(response => {
        this.paginationMetaData = response.metaData;
        this.loading = false;
        return response.items;
      })
    );
    return this.customers$;
  }

  onScrollToEnd() {
    this.customerParams.pageNumber++;
    this.loadCustomers().subscribe();
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
        status: formValue.status
      };

      const action = this.mode === 'create'
      ? this.invoicesService.createInvoice(invoice as CreateInvoice)
      : this.invoicesService.updateInvoice(updateinvoice as UpdateInvoice);

      action.pipe(
        finalize(() => this.isSubmitting = false)
      ).subscribe(
        (response) => {
          console.log('Invoice response:', response.invoiceId);
          const message = this.mode === 'create' ? 'Invoice created successfully' : 'Invoice updated successfully';
          this.toastr.success(message);
          this.resetForm();
          this.router.navigate(['/invoices']);
        },
        error =>{
          console.error(`Error ${this.mode === 'create' ? 'creating' : 'updating'} invoice:`, error);

          let errorMessage = `An error occurred while ${this.mode === 'create' ? 'creating' : 'updating'} the invoice.`;

          if (error.status === 400) {
            if (error.error && error.error.title === "Invalid invoice status") {
              errorMessage = 'The invoice status you provided is invalid. Please select a valid status and try again.';
              // Optionally, you could highlight the status field in the form
              this.invoiceForm.get('status')?.setErrors({'invalidStatus': true});
            } else {
              errorMessage = 'Invalid data submitted. Please check your inputs and try again.';
            }
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
      );
    }
  }

  createInvoiceItemFormGroup(item: Partial<InvoiceItem>): FormGroup<InvoiceItemForm> {
    const group = this.fb.group<InvoiceItemForm>({
      name: this.fb.control(item.name ?? '', Validators.required),
      quantity: this.fb.control(item.quantity ?? 0, [Validators.required, Validators.min(1)]),
      amount: this.fb.control(item.amount ?? 0, [Validators.required, Validators.min(0)])
    });

    if (item?.id) {
      group.addControl('id', this.fb.control(item.id));
    }

    return group;
  }

  setInvoiceForEdit(invoice: UpdateInvoice) {
    this.mode = 'edit';
    this.currentInvoiceId = invoice.id;
    this.invoiceForm.patchValue({
      existingCustomer: invoice.existingCustomer,
      customerId: invoice.customerId,
      customer: invoice.customer,
      items: invoice.items,
      status:invoice.status
    });

    const itemsFormArray = this.fb.array(
      invoice.items.map(item => this.createInvoiceItemFormGroup(item))
    );

    this.invoiceForm.setControl('items', itemsFormArray);
  }

  resetForm() {
    this.mode = 'create';
    this.currentInvoiceId = null;
    this.invoiceForm.reset();
  }


}
