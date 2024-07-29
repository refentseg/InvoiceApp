import { Invoice } from "./invoice"

export interface Customer{
    id?:number;
    fullName:string;
    invoices?:Invoice[];
    company:string;
    email:string;
    phone:string;
}

export interface CustomerParams{
    orderBy:string;
    searchTerm?:string;
    pageNumber:number;
    pageSize:number;
}
