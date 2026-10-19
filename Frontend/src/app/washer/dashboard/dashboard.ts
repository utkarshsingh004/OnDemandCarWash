import { Component } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { apiErrorMessage } from '../../core/services/api-error-message';
import { BookingRecord, PlatformApiService, WasherRecord } from '../../core/services/platform-api.service';

@Component({
  imports: [CurrencyPipe, DatePipe, FormsModule, RouterLink],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard {
  activeSection = 'Overview';
  washerName = 'Washer';
  profileForm = { name: '', email: '', phone: '', address: '' };
  isSavingProfile = false;
  isAvailable = false;
  pendingBookings: BookingRecord[] = [];
  assignedBookings: BookingRecord[] = [];
  busyBookingId = '';
  errorMessage = '';
  successMessage = '';

  constructor(
    private readonly api: PlatformApiService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.washerName = authService.getCurrentName() || 'Washer';
    this.loadProfile();
    this.loadDashboard();
  }

  loadProfile(): void {
    this.api.getWasherProfile().subscribe({
      next: (profile) => {
        this.profileForm = {
          name: profile.name,
          email: profile.email,
          phone: profile.phone,
          address: profile.address
        };
        this.washerName = profile.name;
      },
      error: (error) => this.showError(error, 'Could not load your profile.')
    });
  }

  saveProfile(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isSavingProfile = true;
    this.api.updateWasherProfile(this.profileForm).subscribe({
      next: () => {
        this.isSavingProfile = false;
        this.washerName = this.profileForm.name;
        this.successMessage = 'Profile updated. Sign in next time with your updated email address.';
      },
      error: (error) => {
        this.isSavingProfile = false;
        this.showError(error, 'Could not update your profile.');
      }
    });
  }

  loadDashboard(): void {
    this.api.getPendingBookings().subscribe({
      next: (bookings) => this.pendingBookings = bookings,
      error: (error) => this.showError(error, 'Could not load pending requests.')
    });
    this.api.getMyAssignments().subscribe({
      next: (bookings) => {
        const washerId = this.authService.getCurrentUserId();
        this.assignedBookings = bookings.filter((booking) => booking.washerId === washerId);
      },
      error: (error) => this.showError(error, 'Could not load your assigned washes.')
    });
    this.api.getWasherProfile().subscribe({
      next: (washer: WasherRecord) => {
        this.isAvailable = washer.isAvailable;
      },
      error: (error) => this.showError(error, 'Could not load your availability.')
    });
  }

  setSection(section: string): void {
    this.activeSection = section;
    this.errorMessage = '';
    this.successMessage = '';
  }

  updateAvailability(isAvailable: boolean): void {
    this.api.setAvailability(isAvailable).subscribe({
      next: () => {
        this.isAvailable = isAvailable;
        this.successMessage = isAvailable ? 'You are now available for requests.' : 'You are now unavailable.';
      },
      error: (error) => this.showError(error, 'Could not update your availability.')
    });
  }

  respond(booking: BookingRecord, accept: boolean): void {
    this.busyBookingId = booking.bookingId;
    this.errorMessage = '';
    this.api.respondToBooking(booking.bookingId, accept).subscribe({
      next: () => {
        this.busyBookingId = '';
        this.successMessage = accept ? 'Wash request accepted.' : 'Wash request declined.';
        this.loadDashboard();
      },
      error: (error) => {
        this.busyBookingId = '';
        this.showError(error, 'Could not respond to this request.');
      }
    });
  }

  updateBookingStatus(booking: BookingRecord, status: 'InProgress' | 'Completed'): void {
    this.busyBookingId = booking.bookingId;
    this.errorMessage = '';
    this.api.updateBookingStatus(booking.bookingId, status).subscribe({
      next: () => {
        this.busyBookingId = '';
        this.successMessage = status === 'InProgress'
          ? 'Wash marked as started.'
          : 'Wash marked as completed.';
        this.loadDashboard();
      },
      error: (error) => {
        this.busyBookingId = '';
        this.showError(error, 'Could not update this wash.');
      }
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
