import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    RouterLink,
    FormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  email = '';
  password = '';
  role = 'Customer';

  errorMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login() {

    this.errorMessage = '';
    this.isLoading = true;

    let loginRequest;

    if (this.role === 'Customer') {

      loginRequest =
        this.authService.customerLogin(
          this.email,
          this.password
        );

    } else if (this.role === 'Washer') {

      loginRequest =
        this.authService.washerLogin(
          this.email,
          this.password
        );

    } else {

      loginRequest =
        this.authService.adminLogin(
          this.email,
          this.password
        );
    }

    loginRequest.subscribe({

      next: (response) => {

        console.log('Login successful');

        localStorage.setItem(
          'token',
          response.token
        );

        localStorage.setItem(
          'role',
          this.role
        );

        this.isLoading = false;

        // Temporary dashboard routing
        if (this.role === 'Customer') {
          this.router.navigate(['/customer']);
        }
        else if (this.role === 'Washer') {
          this.router.navigate(['/washer']);
        }
        else {
          this.router.navigate(['/admin']);
        }
      },

      error: (error) => {

        console.error(error);

        this.errorMessage =
          error.error?.detail ||
          error.error?.message ||
          'Invalid email or password';

        this.isLoading = false;
      }

    });
  }
}