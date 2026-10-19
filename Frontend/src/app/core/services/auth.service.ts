import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface LoginResponse {
  message: string;
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl = 'http://localhost:5111/api/auth';

  constructor(private http: HttpClient) {}

  customerLogin(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/customer/login`,
      {
        email,
        password
      }
    );
  }

  washerLogin(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/washer/login`,
      {
        email,
        password
      }
    );
  }

  adminLogin(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/admin/login`,
      {
        email,
        password
      }
    );
  }

  registerCustomer(details: {
    name: string;
    email: string;
    phone: string;
    password: string;
    address: string;
  }): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/customer/register`, details);
  }

  registerWasher(details: {
    name: string;
    email: string;
    phone: string;
    password: string;
    address: string;
  }): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/washer/register`, details);
  }

  getCurrentUserId(): string {
    const claims = this.getSessionClaims();
    const idClaim = Object.entries(claims).find(([key]) => {
      const normalizedKey = key.toLowerCase();
      return normalizedKey.endsWith('/nameidentifier') || normalizedKey === 'nameid' || normalizedKey === 'sub';
    });

    return idClaim ? String(idClaim[1]) : '';
  }

  getCurrentName(): string {
    const claims = this.getSessionClaims();
    const name = claims['name'] || claims['unique_name'] || claims['email'];
    return typeof name === 'string' ? name : '';
  }

  clearSession(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
  }

  private getSessionClaims(): Record<string, unknown> {
    const token = localStorage.getItem('token');
    const encodedClaims = token?.split('.')[1];

    if (!encodedClaims) {
      return {};
    }

    try {
      const normalizedClaims = encodedClaims.replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(atob(normalizedClaims)) as Record<string, unknown>;
    } catch {
      return {};
    }
  }
}
