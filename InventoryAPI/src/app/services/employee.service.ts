import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  private apiUrl = environment.apiBaseUrl + '/employees';

  constructor(private http: HttpClient) {}

  // Fetch all employees
  getAllEmployees(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}`);
  }

  // Get employee by username
  getEmployeeByUsername(username: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Get_By_Username/${username}`);
  }

  // Create employee
  createEmployee(employee: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, employee);
  }

  // Update employee
  updateEmployee(employee: any): Observable<any> {
    return this.http.patch<any>(this.apiUrl, employee);
  }

  // Delete employee by username
  deleteEmployee(username: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/Delete/${username}`);
  }
}
