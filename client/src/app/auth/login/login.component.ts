import { Component, inject, OnInit } from '@angular/core';
import { InputComponent } from '../../components/input/input.component';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { z } from 'zod';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { zodValidator } from '../../validation/validator';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,InputComponent,FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  loginForm: FormGroup = new FormGroup({});
  isLoading = false;
  loginError: string | null = null;

  private loginSchema = z.object({
    username: z.string(),
    password: z.string()
  });
  authService = inject(AuthService)

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private toastr: ToastrService
  ) {

  }
  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  get usernameControl(): FormControl {
    return this.loginForm.get('username') as FormControl;
  }

  get passwordControl(): FormControl {
    return this.loginForm.get('password') as FormControl;
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.loginError = null;

      this.authService.login(this.loginForm.value).subscribe({
        next: (success) => {
          this.isLoading = false;
          if (success) {
            this.toastr.success('Login successful!');
            this.router.navigate(['/']);
          } else {
            this.toastr.error('Login failed. Please try again.');
            this.loginError = 'Login failed. Please try again.';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.loginError = 'An error occurred. Please try again later.';
          this.toastr.error('Login failed. Please try again.');
          console.error('Login error:', error);
        }
      });
    }
  }

}
