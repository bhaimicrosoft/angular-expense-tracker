import { computed, inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  RegisterResponse,
  UpdateCurrentUserRequest,
  UserResponse,
} from '@src/app/core/models/models';
import { firstValueFrom, map } from 'rxjs';
import { environment } from '@src/environments/environment.development';

const tokenKey: string = 'expense-tracker-token';

@Injectable({
  providedIn: 'root',
})
// http://localhost:5004/api/users/auth/login
export class AuthService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly router: Router = inject(Router);
  private readonly tokenSignal: WritableSignal<string | null> = signal<string | null>(
    localStorage.getItem(tokenKey),
  );
  private readonly currentUserSignal = signal<UserResponse | null>(null);

 /* constructor() {
    localStorage.setItem(
      tokenKey,
      'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwMGIwMTVmZC1mMDRmLTQ3OTMtOGFmMC01MTgxMDU4Yzc1MTciLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJleHAiOjE3ODg3OTg0MzgsImlzcyI6IkV4cGVuc2VUcmFja2VyQVBJIiwiYXVkIjoiRXhwZW5zZVRyYWNrZXJDbGllbnQifQ.uuSL1PG7YiVe_w6qpJqSHglWKxn_fqiEffdhpXoNPno',
    );
  }*/

  readonly currentUser = computed(() => {
    const token = this.tokenSignal();
    if(!token || this.isTokenExpired(token)) {
      return null;
    }

    return this.currentUserSignal() ?? this.readUserFromToken(token);
  })

  readonly token = this.tokenSignal.asReadonly();
  readonly isAuthenticated: Signal<boolean> = computed(() => {
    const token = this.tokenSignal();
    return !!token && !this.isTokenExpired(token);
  });

  private isTokenExpired(token: string): boolean {
    const payload = this.decodeToken(token);
    const exp = payload?.['exp'];

    return typeof exp === 'number' ? Date.now() >= exp * 1000 : false;
  }

  private decodeToken(token: string): Record<string, unknown> | null {
    const parts = token.split('.');

    if (parts.length < 2) {
      return null; // token invalid
    }

    try {
      const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64)
          .split('')
          .map((char) => `%${char.charCodeAt(0).toString(16).padStart(2, '0')}`)
          .join(''),
      );

      const parsed = JSON.parse(json);

      return parsed && typeof parsed === 'object' && !Array.isArray(parsed)
        ? (parsed as Record<string, unknown>)
        : null;
    } catch {
      return null;
    }
  }

  private setToken(token: string): void {
    localStorage.setItem(tokenKey, token);
    this.tokenSignal.set(token);
    this.currentUserSignal.set(null);
  }

  async login(request: LoginRequest): Promise<string> {
    const token = await firstValueFrom(
      this.http.post<AuthResponse>(`${environment.serverUrl}/auth/login`, request).pipe(
        map((response: AuthResponse) => {
          const token = response.token ?? response.token;
          if (!token) {
            throw new Error('Authentication response has no token!');
          }

          return token;
        }),
      ),
    );

    this.setToken(token);
    return token;
  }

  async register(request: RegisterRequest): Promise<RegisterResponse> {
    return firstValueFrom(
      this.http.post<RegisterResponse>(`${environment.serverUrl}/auth/register`, request),
    );
  }

  async loadCurrentUser(): Promise<UserResponse> {
    const user = await firstValueFrom(this.http.get<UserResponse>(`${environment.serverUrl}/me`));

    this.currentUserSignal.set(user);
    return user;
  }

  async updateCurrentUser(request: UpdateCurrentUserRequest): Promise<UserResponse> {
    await firstValueFrom(this.http.put<void>(`${environment.serverUrl}/me`, request));
    return this.loadCurrentUser();
  }

  async deleteCurrentUser(): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${environment.serverUrl}/me}`));
    // clear the token
    this.clearToken();
    await this.router.navigateByUrl('/login');
  }

  logout(): void {
    this.clearToken();
    this.router.navigateByUrl('/login').then();
  }

  private clearToken(): void {
    localStorage.removeItem(tokenKey);
    this.tokenSignal.set(null);
    this.currentUserSignal.set(null);
  }

  private readClaim(payload: Record<string, unknown>, keys: string[]): string | null {
    for (const key of keys) {
      const value = payload[key];
      if (typeof value === 'string' && value.trim()) {
        return value;
      }
    }

    return null;
  }

  private readUserFromToken(token: string): UserResponse | null {
    const payload = this.decodeToken(token);
    if (!payload) {
      return null;
    }

    const id = this.readClaim(payload, [
      'sub',
      'nameid',
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
    ]);
    const email = this.readClaim(payload, [
      'email',
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
    ]);
    const fullName = this.readClaim(payload, [
      'name',
      'unique_name',
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
    ]);

    if (!id) {
      return null;
    }

    return {
      id,
      email: email ?? 'signed-in-user@local',
      fullName: fullName ?? email ?? 'Expense Pro',
    };
  }
}
