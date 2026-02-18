import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppUserDTO, LoginDTO, Response, UserSession } from '../../models/auth';
import { Observable, map } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private url = '/api/authentication';

  // Holds current user session
  currentUser = signal<UserSession | null>(null);

  constructor(private http: HttpClient, private router: Router) {
    const token = localStorage.getItem('token');

    if (token) {
      try {
        const decoded: any = jwtDecode(token);

        // Check expiration
        if (decoded.exp * 1000 > Date.now()) {
          this.currentUser.set({
            id: decoded.sub || decoded.id || 0,
            name: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
            email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
            role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
            token: token
          });
        } else {
          localStorage.removeItem('token');
          this.currentUser.set(null);
        }

      } catch (e) {
        localStorage.removeItem('token');
        this.currentUser.set(null);
      }
    }
  }

  register(user: AppUserDTO): Observable<Response> {
    return this.http.post<Response>(`${this.url}/register`, user);
  }

  login(user: LoginDTO): Observable<Response> {
    return this.http.post<Response>(`${this.url}/login`, user).pipe(
      map((res) => {
        if (res.flag && res.message) {
          localStorage.setItem('token', res.message);
          const decoded: any = jwtDecode(res.message);
          console.log(res, decoded);
          if (decoded.exp * 1000 > Date.now()) {
            this.currentUser.set({
              id: decoded.sub || decoded.id || 0,
              name: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
              email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
              role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
              token: res.message
            });
            console.log(this.currentUser);
          } else {
            localStorage.removeItem('token');
          }
          console.log(this.currentUser);
          return res;
        }
        return res;
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }
}
