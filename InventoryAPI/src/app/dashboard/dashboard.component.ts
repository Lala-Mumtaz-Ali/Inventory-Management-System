import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth-service.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EditEmployeeComponent } from '../edit-employee/edit-employee.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  imports: [EditEmployeeComponent, CommonModule, RouterLink],
})
export class DashboardComponent implements OnInit{
  showEditEmployee: boolean = false;
  selectedOperation: string = '';
  

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    
  }

  logout() {
    this.authService.removeToken();
    console.log('User logged out');
    this.router.navigateByUrl('/login');
  }

  onEditEmployeesClick() {
    this.showEditEmployee = !this.showEditEmployee;
    this.selectedOperation = '';  // Reset operation
  }

  onOperationSelect(operation: string) {
    console.log('Selected operation:', operation);
    this.selectedOperation = operation;
    this.showEditEmployee = true;
  }
  
}
