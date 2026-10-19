import { Component } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { apiErrorMessage } from '../../core/services/api-error-message';
import {
  BookingRecord,
  CarRecord,
  CustomerRecord,
  NotificationRecord,
  PlatformApiService
} from '../../core/services/platform-api.service';

@Component({
  imports: [CurrencyPipe, DatePipe, FormsModule, RouterLink],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard {
  activeSection = 'Overview';
  customerName = 'Customer';
  profileForm: Pick<CustomerRecord, 'name' | 'email' | 'phone' | 'address'> = {
    name: '',
    email: '',
    phone: '',
    address: ''
  };
  cars: CarRecord[] = [];
  bookings: BookingRecord[] = [];
  notifications: NotificationRecord[] = [];
  errorMessage = '';
  successMessage = '';
  isLoading = false;
  editingCarId = '';
  resendingBookingId = '';
  carForm = this.emptyCarForm();
  bookingCarId = '';
  serviceType = '0';
  bookingDate = this.defaultBookingDate();
  minimumBookingDate = this.minimumDateTime();
  cancellingBookingId = '';

  constructor(
    private readonly api: PlatformApiService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.customerName = authService.getCurrentName() || 'Customer';
    this.loadProfile();
    this.loadDashboard();
  }

  loadProfile(): void {
    this.api.getCustomerProfile().subscribe({
      next: (profile) => {
        this.profileForm = {
          name: profile.name,
          email: profile.email,
          phone: profile.phone,
          address: profile.address
        };
        this.customerName = profile.name;
      },
      error: (error) => this.showError(error, 'Could not load your profile.')
    });
  }

  saveProfile(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;
    this.api.updateCustomerProfile(this.profileForm).subscribe({
      next: () => {
        this.isLoading = false;
        this.customerName = this.profileForm.name;
        this.successMessage = 'Profile updated. Booking emails will go to the email address shown here.';
      },
      error: (error) => {
        this.isLoading = false;
        this.showError(error, 'Could not update your profile.');
      }
    });
  }

  loadDashboard(): void {
    this.api.getCars().subscribe({
      next: (cars) => {
        this.cars = cars;
        if (!this.bookingCarId && cars.length) this.bookingCarId = cars[0].carId;
      },
      error: (error) => this.showError(error, 'Could not load your cars.')
    });
    this.loadBookings();

    const customerId = this.authService.getCurrentUserId();
    if (customerId) {
      this.api.getNotifications(customerId).subscribe({
        next: (notifications) => this.notifications = notifications,
        error: (error) => this.showError(error, 'Could not load notifications.')
      });
    }
  }

  loadBookings(): void {
    this.api.getMyBookings().subscribe({
      next: (bookings) => this.bookings = bookings,
      error: (error) => this.showError(error, 'Could not load your bookings.')
    });
  }

  setSection(section: string): void {
    this.activeSection = section;
    this.errorMessage = '';
    this.successMessage = '';
  }

  saveCar(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;
    const request = this.editingCarId
      ? this.api.updateCar(this.editingCarId, this.carForm)
      : this.api.addCar(this.carForm);

    request.subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = this.editingCarId ? 'Vehicle details updated.' : 'Vehicle added to your garage.';
        this.resetCarForm();
        this.api.getCars().subscribe({ next: (cars) => this.cars = cars });
      },
      error: (error) => {
        this.isLoading = false;
        this.showError(error, 'Could not save this vehicle.');
      }
    });
  }

  editCar(car: CarRecord): void {
    this.editingCarId = car.carId;
    this.carForm = {
      brand: car.brand,
      model: car.model,
      registrationNumber: car.registrationNumber,
      color: car.color,
      carType: car.carType
    };
    this.setSection('Garage');
  }

  deleteCar(car: CarRecord): void {
    if (!window.confirm(`Remove ${car.brand} ${car.model} from your garage?`)) return;
    this.api.deleteCar(car.carId).subscribe({
      next: () => {
        this.cars = this.cars.filter((item) => item.carId !== car.carId);
        this.successMessage = 'Vehicle removed.';
      },
      error: (error) => this.showError(error, 'Could not remove this vehicle.')
    });
  }

  bookWash(): void {
    this.errorMessage = '';
    this.successMessage = '';
    if (!this.bookingCarId) {
      this.errorMessage = 'Add a vehicle before booking a wash.';
      return;
    }

    this.isLoading = true;
    this.api.createBooking({
      carId: this.bookingCarId,
      serviceType: Number(this.serviceType),
      bookingDate: new Date(this.bookingDate).toISOString(),
      isScheduled: true
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Wash request sent. You can follow its status in Bookings.';
        this.loadBookings();
        this.setSection('Bookings');
      },
      error: (error) => {
        this.isLoading = false;
        this.showError(error, 'Could not create this booking.');
      }
    });
  }

  cancelBooking(booking: BookingRecord): void {
    if (!window.confirm('Cancel this wash request?')) return;
    this.cancellingBookingId = booking.bookingId;
    this.api.cancelBooking(booking.bookingId).subscribe({
      next: () => {
        this.cancellingBookingId = '';
        this.successMessage = 'Booking cancelled.';
        this.loadBookings();
      },
      error: (error) => {
        this.cancellingBookingId = '';
        this.showError(error, 'Could not cancel this booking.');
      }
    });
  }

  resendBookingNotification(booking: BookingRecord): void {
    this.resendingBookingId = booking.bookingId;
    this.errorMessage = '';
    this.successMessage = '';
    this.api.resendBookingNotification(booking.bookingId).subscribe({
      next: (result) => {
        this.resendingBookingId = '';
        if (result.emailSent) {
          this.successMessage = 'Booking confirmation email sent.';
        } else {
          this.errorMessage = result.emailError || 'The confirmation email could not be sent.';
        }
      },
      error: (error) => {
        this.resendingBookingId = '';
        this.showError(error, 'Could not resend the booking confirmation.');
      }
    });
  }

  carLabel(carId: string): string {
    const car = this.cars.find((item) => item.carId === carId);
    return car ? `${car.brand} ${car.model}` : 'Vehicle';
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

  resetCarForm(): void {
    this.editingCarId = '';
    this.carForm = this.emptyCarForm();
  }

  private emptyCarForm(): Omit<CarRecord, 'carId'> {
    return { brand: '', model: '', registrationNumber: '', color: '', carType: 'Sedan' };
  }

  private defaultBookingDate(): string {
    const date = new Date();
    date.setHours(date.getHours() + 1, 0, 0, 0);
    return this.localDateTimeValue(date);
  }

  private minimumDateTime(): string {
    const date = new Date();
    date.setMinutes(date.getMinutes() + 1, 0, 0);
    return this.localDateTimeValue(date);
  }

  private localDateTimeValue(date: Date): string {
    const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return localDate.toISOString().slice(0, 16);
  }

  private showError(error: unknown, fallback: string): void {
    this.errorMessage = apiErrorMessage(error, fallback);
  }
}
