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

  private apiUrl = 'http://localhost:5279/api/Auth';

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
}