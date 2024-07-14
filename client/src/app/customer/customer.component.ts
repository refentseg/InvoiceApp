import { Component, inject, OnInit } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Customer, CustomerParams } from '../models/customer';
import { CustomerService } from '../services/customer.service';
import { CommonModule, AsyncPipe } from '@angular/common';
import { ButtonComponent } from '../components/button/button.component';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule,AsyncPipe,ButtonComponent],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.css'
})
export class CustomerComponent implements OnInit {
  customers$!:Observable<Customer[]>;

  customerParams: CustomerParams = {
    orderBy:'',
    searchTerm:'',
  }

  customerService = inject(CustomerService)

  ngOnInit():void {
    this.loadCustomers();
  }

  loadCustomers(){
    this.customers$ = this.customerService.getCustomers(this.customerParams).pipe(
      map(response => {
        return response;
      })
    );;
  }

}
