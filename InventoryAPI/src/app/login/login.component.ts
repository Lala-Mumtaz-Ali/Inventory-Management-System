// src/app/login/login.component.ts
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth-service.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls:['./login.component.css'],
  })
export class LoginComponent implements OnInit{
  loginForm: FormGroup;
  isSubmitted: Boolean = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private service: AuthService
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    })
  }

  ngOnInit(): void {
    if(this.service.isLoggedIn())
      this.router.navigateByUrl('/dashboard')
  }

  hasDisplayableError(controlName: string): Boolean {
    const control = this.loginForm.get(controlName);
    return (
      (Boolean(control?.invalid) && (Boolean(this.isSubmitted) || Boolean(control?.touched) || Boolean(control?.dirty))))
      ;
  }

  onSubmit() {
    this.isSubmitted = true;
    console.log(this.loginForm.value);
  
    if (this.loginForm.valid) {
      this.service.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          if (response.token) {
            console.log('Form Submitted', this.loginForm.value);
            this.service.saveToken(response.token);
            console.log('Navigation to dashboard initiated');
            this.router.navigateByUrl('dashboard');
          }
        },
        error: (error: any) => {
          console.error('Error occurred:', error); // Log detailed error
          this.errorMessage = 'Invalid username or password'; // General error for the user
        }
      });
    } else {
      this.errorMessage = 'Please fill in both fields correctly';
    }
  }

  
}
