import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { EmployeeService } from '../services/employee.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-employee',
  standalone: true,
  templateUrl: './edit-employee.component.html',
  styleUrls: ['./edit-employee.component.css'],
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
})
export class EditEmployeeComponent implements OnInit {
  @Input() selectedOperation: string = ''; // Receive operation type from parent

  operations: string[] = ['Create', 'Update', 'Delete', 'Get All Employees', 'Get By Username'];
  employees: any[] = [];
  employeeData: any = {
    user_name: '',
    password: '',
    email: '',
    contact_no: '',
    role: '',
    name: '',
  };
  employeeFormVisible: boolean = false;
  fetchedEmployee: any = null; // For storing result of Get By Username

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    if (this.selectedOperation === 'Get All Employees') {
      this.getAllEmployees();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedOperation'] && changes['selectedOperation'].currentValue) {
      this.fetchedEmployee = null; // Reset fetched employee data
      if (this.selectedOperation === 'Get All Employees') {
        this.getAllEmployees();
      } else {
        this.employeeData = {}; // Clear previous data
      }
    }
  }

  getAllEmployees(): void {
    this.employeeService.getAllEmployees().subscribe((data: any[]) => {
      this.employees = data;
    });
  }

  getEmployeeByUsername(): void {
    if (this.employeeData.user_name) {
      this.employeeService.getEmployeeByUsername(this.employeeData.user_name).subscribe((data: any) => {
        this.fetchedEmployee = data;
      });
    }
  }

  createEmployee(): void {
    this.employeeService.createEmployee(this.employeeData).subscribe(() => {
      this.clearForm();
      this.getAllEmployees(); // Refresh the employee list
    });
  }

  updateEmployee(): void {
    this.employeeService.updateEmployee(this.employeeData).subscribe(() => {
      this.clearForm();
      this.getAllEmployees(); // Refresh the employee list
    });
  }

  deleteEmployee(): void {
    this.employeeService.deleteEmployee(this.employeeData.user_name).subscribe(() => {
      this.clearForm();
      this.getAllEmployees(); // Refresh the employee list
    });
  }

  clearForm(): void {
    this.employeeData = {};
    this.selectedOperation = '';
    this.employeeFormVisible = false;
    this.fetchedEmployee = null; // Reset fetched data
  }
}
