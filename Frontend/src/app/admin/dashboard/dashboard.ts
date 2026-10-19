import { Component } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { apiErrorMessage } from '../../core/services/api-error-message';
import {
  BookingRecord,
  CustomerRecord,
  PlatformApiService,
  WasherRecord
} from '../../core/services/platform-api.service';

@Component({
  imports: [CurrencyPipe, DatePipe, FormsModule, RouterLink],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard {
  activeSection = 'Overview';
  adminName = 'Administrator';
  customers: CustomerRecord[] = [];
  washers: WasherRecord[] = [];
  bookings: BookingRecord[] = [];
  searchTerm = '';
  errorMessage = '';
  successMessage = '';

  constructor(
    private readonly api: PlatformApiService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.adminName = authService.getCurrentName() || 'Administrator';
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.api.getCustomers().subscribe({
      next: (customers) => this.customers = customers,
      error: (error) => this.showError(error, 'Could not load customers.')
    });
    this.api.getWashers().subscribe({
      next: (washers) => this.washers = washers,
      error: (error) => this.showError(error, 'Could not load washers.')
    });
    this.api.getAllBookings().subscribe({
      next: (bookings) => this.bookings = bookings,
      error: (error) => this.showError(error, 'Could not load bookings.')
    });
  }

  setSection(section: string): void {
    this.activeSection = section;
    this.searchTerm = '';
    this.errorMessage = '';
    this.successMessage = '';
  }

  get filteredCustomers(): CustomerRecord[] {
    const search = this.searchTerm.trim().toLowerCase();
    return this.customers.filter((customer) =>
      `${customer.name} ${customer.email} ${customer.phone}`.toLowerCase().includes(search)
    );
  }

  get filteredWashers(): WasherRecord[] {
    const search = this.searchTerm.trim().toLowerCase();
    return this.washers.filter((washer) =>
      `${washer.name} ${washer.email} ${washer.phone}`.toLowerCase().includes(search)
    );
  }

  get filteredBookings(): BookingRecord[] {
    const search = this.searchTerm.trim().toLowerCase();
    return this.bookings.filter((booking) =>
      `${booking.bookingId} ${booking.status} ${booking.serviceType} ${booking.carId}`.toLowerCase().includes(search)
    );
  }

  removeCustomer(customer: CustomerRecord): void {
    if (!window.confirm(`Delete the customer account for ${customer.name}?`)) return;
    this.api.deleteCustomer(customer.customerId).subscribe({
      next: () => {
        this.customers = this.customers.filter((item) => item.customerId !== customer.customerId);
        this.successMessage = `${customer.name} was removed.`;
      },
      error: (error) => this.showError(error, 'Could not delete this customer.')
    });
  }

  removeWasher(washer: WasherRecord): void {
    if (!window.confirm(`Delete the washer account for ${washer.name}?`)) return;
    this.api.deleteWasher(washer.washerId).subscribe({
      next: () => {
        this.washers = this.washers.filter((item) => item.washerId !== washer.washerId);
        this.successMessage = `${washer.name} was removed.`;
      },
      error: (error) => this.showError(error, 'Could not delete this washer.')
    });
  }

  serviceLabel(serviceType: string | number): string {
    return serviceType === 0 || serviceType === '0' ? 'Basic wash' : 'Premium wash';
  }

  statusClass(status: string): string {
    return status.toLowerCase().replace(/\s+/g, '-');
  }

  logout(): void {
    this.authService.clearSession();
    this.router.navigate(['/login']);
  }

  private showError(error: unknown, fallback: string): void {
    this.errorMessage = apiErrorMessage(error, fallback);
  }
}
