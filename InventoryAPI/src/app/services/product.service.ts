import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = `${environment.apiBaseUrl}/Product`; // API base URL

  constructor(private http: HttpClient) {}

  //Get All Products
  getAllProducts(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetAllProducts`)
    .pipe(catchError(this.handleError));
  }

  // Get Product by ID
  getProduct(id: number): Observable<any> {
    return this.http
      .get<any>(`${this.apiUrl}/GetProduct/${id}`)
      .pipe(catchError(this.handleError));
  }

  // Get Product Analysis by ID
  getProductAnalysis(id: number): Observable<any> {
    return this.http
      .get<any>(`${this.apiUrl}/ProductAnalysis/${id}`)
      .pipe(catchError(this.handleError));
  }

  // Get Products by Category
  getMainFilteredProducts(category: string): Observable<any> {
    return this.http
      .get<any>(`${this.apiUrl}/Get_Main_Filtered/${category}`)
      .pipe(catchError(this.handleError));
  }

  // Create Product
  createProduct(product: any): Observable<any> {
    return this.http
      .post<any>(`${this.apiUrl}/Create`, product)
      .pipe(catchError(this.handleError));
  }

  // Edit Product by ID
  editProduct(id: number, product: any): Observable<any> {
    return this.http
      .put<any>(`${this.apiUrl}/Edit/${id}`, product)
      .pipe(catchError(this.handleError));
  }

  // Delete Product by ID
  deleteProduct(id: number): Observable<any> {
    return this.http
      .delete<any>(`${this.apiUrl}/Delete/${id}`)
      .pipe(catchError(this.handleError));
  }

  // Error handling
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Server returned code: ${error.status}, error message is: ${error.message}`;
    }
    return throwError(errorMessage);
  }


  getAllProductLots(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetAllProductLots`).pipe(catchError(this.handleError));
  }
}
