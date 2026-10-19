import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  imports: [FormsModule, RouterLink],
  selector: 'app-register',
  styleUrl: './register.css',
  templateUrl: './register.html',
})
export class Register {
  role: 'Customer' | 'Washer' = 'Customer';
  name = '';
  email = '';
  phone = '';
  password = '';
  address = '';
  errorMessage = '';
  successMessage = '';
  isLoading = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  register(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;

    const details = {
      name: this.name,
      email: this.email,
      phone: this.phone,
      password: this.password,
      address: this.address
    };
    const request = this.role === 'Customer'
      ? this.authService.registerCustomer(details)
      : this.authService.registerWasher(details);

    request.subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Account created. You can sign in now.';
        setTimeout(() => this.router.navigate(['/login']), 900);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.detail || error.error?.message || 'Registration could not be completed.';
      }
    });
  }
}
