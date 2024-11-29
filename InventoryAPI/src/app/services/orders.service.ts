import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable , throwError, catchError } from 'rxjs';
import {Order, OrderDispatchDTO, Product, OrderItem, PurchaseOrder, Supplier } from '../models/DTOs';
@Injectable({
  providedIn: 'root'
})
export class OrdersService {

  private apiUrl = `${environment.apiBaseUrl}/order`;

  constructor(private http: HttpClient) { }

  // Get products by category
  getProductsByCategory(category: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/RetrieveProducts/${category}`);
  }

  // Get pending customer orders
  getPendingCustomerOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/PendingCustomerOrders`);
  }

  // Dispatch orders
  dispatchOrders(orders: OrderDispatchDTO[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/Dispatch`, orders);
  }


  reorderLowStockProducts(): Observable<any> {
    return this.http.get(`${this.apiUrl}/StockAllCurrentProducts`).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(error);
      })
    );
  }

  reorderMissingProducts(): Observable<any> {
    return this.http.get(`${this.apiUrl}/StockAllMissingProducts`).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(error);
      })
    );
  }

  // Receive purchase orders
  receivePurchaseOrders(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/RecievePurchaseOrders`, {}).pipe(
        catchError((error: HttpErrorResponse) => {
            console.error('Error receiving purchase orders:', error);
            return throwError(error);
        })
    );
  }



  // Get all orders
  getAllOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/AllOrders`).pipe(
      catchError(this.handleError)
    );
  }

  // Get all order items
  getAllOrderItems(): Observable<OrderItem[]> {
    return this.http.get<OrderItem[]>(`${this.apiUrl}/AllOrderItems`).pipe(
      catchError(this.handleError)
    );
  }

  // Get all suppliers
  getAllSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.apiUrl}/AllSuppliers`).pipe(
      catchError(this.handleError)
    );
  }

  // Get all purchase orders
  getAllPurchaseOrders(): Observable<PurchaseOrder[]> {
    return this.http.get<PurchaseOrder[]>(`${this.apiUrl}/AllPurchaseOrders`).pipe(
      catchError(this.handleError)
    );
  }

  // Handle errors
  private handleError(error: HttpErrorResponse) {
    console.error('An error occurred:', error);
    return throwError(error.message || 'Server Error');
  }
}
