import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_URL = 'http://localhost:5111/api';

export interface CarRecord {
  carId: string;
  brand: string;
  model: string;
  registrationNumber: string;
  color: string;
  carType: string;
}

export interface BookingRecord {
  bookingId: string;
  carId: string;
  washerId?: string | null;
  serviceType: string | number;
  bookingDate: string;
  isScheduled: boolean;
  status: string;
  price: number;
  createdAt?: string;
}

export interface BookingNotificationResult {
  emailSent: boolean;
  emailError?: string | null;
}

export interface CustomerRecord {
  customerId: string;
  name: string;
  email: string;
  phone: string;
  address: string;
}

export interface WasherRecord {
  washerId: string;
  name: string;
  email: string;
  phone: string;
  address: string;
  isAvailable: boolean;
}

export interface NotificationRecord {
  notificationId: string;
  bookingId: string;
  message: string;
  type: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class PlatformApiService {
  constructor(private readonly http: HttpClient) {}

  getCars(): Observable<CarRecord[]> {
    return this.http.get<CarRecord[]>(`${API_URL}/customer/cars`);
  }

  getCustomerProfile(): Observable<CustomerRecord> {
    return this.http.get<CustomerRecord>(`${API_URL}/customer/profile`);
  }

  updateCustomerProfile(profile: Pick<CustomerRecord, 'name' | 'email' | 'phone' | 'address'>): Observable<unknown> {
    return this.http.put(`${API_URL}/customer/profile`, profile);
  }

  addCar(car: Omit<CarRecord, 'carId'>): Observable<CarRecord> {
    return this.http.post<CarRecord>(`${API_URL}/customer/cars`, car);
  }

  updateCar(carId: string, car: Omit<CarRecord, 'carId'>): Observable<unknown> {
    return this.http.put(`${API_URL}/customer/cars/${carId}`, car);
  }

  deleteCar(carId: string): Observable<unknown> {
    return this.http.delete(`${API_URL}/customer/cars/${carId}`);
  }

  getMyBookings(): Observable<BookingRecord[]> {
    return this.http.get<BookingRecord[]>(`${API_URL}/booking/my-bookings`);
  }

  createBooking(booking: {
    carId: string;
    serviceType: number;
    bookingDate: string;
    isScheduled: boolean;
  }): Observable<BookingRecord> {
    return this.http.post<BookingRecord>(`${API_URL}/booking`, booking);
  }

  cancelBooking(bookingId: string): Observable<unknown> {
    return this.http.delete(`${API_URL}/booking/${bookingId}`);
  }

  resendBookingNotification(bookingId: string): Observable<BookingNotificationResult> {
    return this.http.post<BookingNotificationResult>(
      `${API_URL}/booking/${bookingId}/notification/retry`,
      {}
    );
  }

  getNotifications(customerId: string): Observable<NotificationRecord[]> {
    return this.http.get<NotificationRecord[]>(`${API_URL}/notification/customer/${customerId}`);
  }

  getPendingBookings(): Observable<BookingRecord[]> {
    return this.http.get<BookingRecord[]>(`${API_URL}/booking/pending`);
  }

  getMyAssignments(): Observable<BookingRecord[]> {
    return this.http.get<BookingRecord[]>(`${API_URL}/booking/my-assignments`);
  }

  respondToBooking(bookingId: string, accept: boolean): Observable<unknown> {
    return this.http.put(`${API_URL}/booking/${bookingId}/respond`, { accept });
  }

  updateBookingStatus(bookingId: string, status: 'InProgress' | 'Completed'): Observable<unknown> {
    return this.http.put(`${API_URL}/booking/${bookingId}/status`, { status });
  }

  setAvailability(isAvailable: boolean): Observable<unknown> {
    return this.http.put(`${API_URL}/washer/availability`, null, {
      params: { isAvailable }
    });
  }

  getCustomers(): Observable<CustomerRecord[]> {
    return this.http.get<CustomerRecord[]>(`${API_URL}/customer/all`);
  }

  deleteCustomer(customerId: string): Observable<unknown> {
    return this.http.delete(`${API_URL}/customer/${customerId}`);
  }

  getWashers(): Observable<WasherRecord[]> {
    return this.http.get<WasherRecord[]>(`${API_URL}/washer/all`);
  }

  getWasherProfile(): Observable<WasherRecord> {
    return this.http.get<WasherRecord>(`${API_URL}/washer/profile`);
  }

  updateWasherProfile(profile: Pick<WasherRecord, 'name' | 'email' | 'phone' | 'address'>): Observable<unknown> {
    return this.http.put(`${API_URL}/washer/profile`, profile);
  }

  deleteWasher(washerId: string): Observable<unknown> {
    return this.http.delete(`${API_URL}/washer/${washerId}`);
  }

  getAllBookings(): Observable<BookingRecord[]> {
    return this.http.get<BookingRecord[]>(`${API_URL}/booking`);
  }
}