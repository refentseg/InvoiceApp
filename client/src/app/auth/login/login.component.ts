import { Component, OnInit } from '@angular/core';
import { InputComponent } from '../../components/input/input.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { z } from 'zod';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { zodValidator } from '../../validation/validator';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [InputComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  loginForm!: FormGroup;
  isLoading = false;
  loginError: string | null = null;

  private loginSchema = z.object({
    username: z.string(),
    password: z.string()
  });

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loginForm = this.fb.group({
      username: ['', zodValidator(this.loginSchema.shape.username)],
      password: ['', zodValidator(this.loginSchema.shape.password)]
    });
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
            this.loginError = 'Login failed. Please try again.';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.loginError = 'An error occurred. Please try again later.';
          console.error('Login error:', error);
        }
      });
    }
  }
}
