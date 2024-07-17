import { Customer } from "./customer";

export interface InvoiceItem{
  productId: number;
  name:number;
  price:number;
  quantity:number;
}

export interface Invoice{
  id:string;
  customerId:number;
  customer:Customer;
  salesRep:string;
  orderDate:string;
  items:InvoiceItem[];
  subtotal:number;
  vat:number;
  total:number;
  invoiceStatus:string;
}

export interface InvoiceParams{
  orderBy:string;
  searchTerm?:string;
  pageNumber:number;
  pageSize:number;
}

export interface CreateInvoice {
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

export interface UpdateInvoice {
  id:string;
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
  invoiceStatus:string;
}
