import { Component, inject, OnInit } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Customer, CustomerParams } from '../models/customer';
import { CustomerService } from '../services/customer.service';
import { CommonModule, AsyncPipe } from '@angular/common';
import { ButtonComponent } from '../components/button/button.component';
import { MetaData } from '../models/pagination';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule,AsyncPipe,ButtonComponent,RouterModule,FormsModule,LucideAngularModule],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.css'
})
export class CustomerComponent implements OnInit {
  customers$!:Observable<Customer[]>;
  customerParams: CustomerParams = {
    orderBy:'id',
    searchTerm:'',
    pageNumber: 1,
    pageSize: 5
  }
  paginationMetaData!: MetaData;

  constructor(private customerService:CustomerService,private router : Router){}

  ngOnInit():void {
    this.loadCustomers();
  }

  loadCustomers(){
    this.customers$ = this.customerService.getCustomers(this.customerParams).pipe(
      map(response => {
        this.paginationMetaData = response.metaData;
        return response.items;
      })
    );
  }

  searchCustomer(){
    this.customerParams.pageNumber=1;
    this.loadCustomers();
  }

  onCustomerPageChange(pageNumber: number) {
    this.customerParams.pageNumber = pageNumber;
    this.loadCustomers();
  }

  getPageNumbers(): number[] {
    const totalPages = this.paginationMetaData.totalPages;
    const pageNumbers: number[] = [];
    for (let i = 1; i <= totalPages; i++) {
      pageNumbers.push(i);
    }
    return pageNumbers;
  }

  editCustomer(id: number) {
    this.router.navigate(['/customer', id, 'edit']);
  }

}
