import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { z } from 'zod';
import { AuthService } from '../auth.service';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { InputComponent } from '../../components/input/input.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,InputComponent,FormsModule,RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{
  registerForm: FormGroup = new FormGroup({});
  isLoading = false;
  registerError: string | null = null;
  isRegisterMode = true;
  @Input() mode!: string;
  @Output() toggleMode = new EventEmitter<string>();

  onToggleMode() {
    this.toggleMode.emit('login');
  }

  private registerSchema = z.object({
    firstName: z.string().min(1, 'First name is required'),
    lastName: z.string().min(1, 'Last name is required'),
    username: z.string().min(1, 'Username is required'),
    email: z.string().email('Invalid email format'),
    company: z.string().min(1, 'Company is required'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
    confirmPassword: z.string().min(1, 'Confirm password is required')
  });

  authService = inject(AuthService);

  firstNameSchema = this.registerSchema.shape.firstName;
  lastNameSchema = this.registerSchema.shape.lastName;
  usernameSchema = this.registerSchema.shape.username;
  emailSchema = this.registerSchema.shape.email;
  companySchema = this.registerSchema.shape.company;
  passwordSchema = this.registerSchema.shape.password;
  confirmPasswordSchema = this.registerSchema.shape.confirmPassword;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      firstName: [''],
      lastName: [''],
      username: [''],
      email: [''],
      company: [''],
      password: [''],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : {'mismatch': true};
  }

  get firstNameControl(): FormControl {
    return this.registerForm.get('firstName') as FormControl;
  }

  get lastNameControl(): FormControl {
    return this.registerForm.get('lastName') as FormControl;
  }

  get usernameControl(): FormControl {
    return this.registerForm.get('username') as FormControl;
  }

  get emailControl(): FormControl {
    return this.registerForm.get('email') as FormControl;
  }

  get companyControl(): FormControl {
    return this.registerForm.get('company') as FormControl;
  }

  get passwordControl(): FormControl {
    return this.registerForm.get('password') as FormControl;
  }

  get confirmPasswordControl(): FormControl {
    return this.registerForm.get('confirmPassword') as FormControl;
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.registerError = null;

      const registerData = this.registerSchema.parse(this.registerForm.value);
      this.authService.register(registerData).subscribe({
        next: (success) => {
          this.isLoading = false;
          if (success) {
            this.toastr.success('Registration successful!');
            this.router.navigate(['/']);
          } else {
            this.toastr.error('Registration failed. Please try again.');
            this.registerError = 'Registration failed. Please try again.';
          }
        },
        error: (error) => {
          this.isLoading = false;
          if (error.error && error.error.errors) {
            const errorMessages = Object.values(error.error.errors).flat();
            this.registerError = errorMessages.join(' ');
          } else {
            this.registerError = 'An error occurred. Please try again later.';
          }
          this.toastr.error(this.registerError);
          console.error('Registration error:', error);
        }
      });
    }
  }
}
