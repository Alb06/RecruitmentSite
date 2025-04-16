import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';

import { AuthResultDto } from '../models/auth-result.model';
import { LoginDto } from '../models/login.model';
import { UserCreateDto } from '../models/register.model';
import { User } from '../models/user.model';
import { OperationResult } from '../../shared/models/operation-result.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private readonly TOKEN_KEY = 'auth_token';
  private readonly API_URL = '/api';

  constructor(private http: HttpClient) {
    this.loadUserFromStorage();
  }

  /**
   * Authentifie un utilisateur et stocke son token
   */
  login(credentials: LoginDto): Observable<AuthResultDto> {
    return this.http.post<AuthResultDto>(`${this.API_URL}/auth/login`, credentials)
      .pipe(
        tap(result => this.handleAuthentication(result.token)),
        catchError(error => {
          console.error('Login error', error);
          return throwError(() => new Error(error.error?.message || 'Échec de connexion'));
        })
      );
  }

  /**
   * Enregistre un nouvel utilisateur
   */
  register(userData: UserCreateDto): Observable<OperationResult> {
    // L'API attend un tableau d'utilisateurs
    return this.http.post<OperationResult>(`${this.API_URL}/users/createUsers`, [userData])
      .pipe(
        catchError(error => {
          console.error('Registration error', error);
          return throwError(() => new Error(error.error?.message || 'Échec de l\'inscription'));
        })
      );
  }

  /**
   * Déconnecte l'utilisateur
   */
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUserSubject.next(null);
  }

  /**
   * Vérifie si l'utilisateur est authentifié
   */
  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  }

  /**
   * Récupère le token d'authentification
   */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Vérifie si l'utilisateur a un rôle spécifique
   */
  hasRole(role: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.roles?.includes(role) || false;
  }

  /**
   * Gère l'authentification en stockant le token et les informations utilisateur
   */
  private handleAuthentication(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    const user = this.decodeToken(token);
    this.currentUserSubject.next(user);
  }

  /**
   * Charge l'utilisateur à partir du localStorage au démarrage
   */
  private loadUserFromStorage(): void {
    const token = this.getToken();
    if (token && !this.isTokenExpired(token)) {
      const user = this.decodeToken(token);
      this.currentUserSubject.next(user);
    }
  }

  /**
   * Décode le token JWT pour extraire les informations utilisateur
   */
  private decodeToken(token: string): User | null {
    try {
      const decodedToken: any = jwtDecode(token);

      return {
        id: parseInt(decodedToken.sub),
        name: decodedToken.name || '', // Si disponible dans le token
        email: decodedToken.email,
        roles: Array.isArray(decodedToken.role)
          ? decodedToken.role
          : decodedToken.role
            ? [decodedToken.role]
            : []
      };
    } catch (error) {
      console.error('Token decode error', error);
      return null;
    }
  }

  /**
   * Vérifie si le token est expiré
   */
  private isTokenExpired(token: string): boolean {
    try {
      const decodedToken: any = jwtDecode(token);
      // exp est en secondes, Date.now() est en millisecondes
      return decodedToken.exp * 1000 < Date.now();
    } catch (error) {
      return true;
    }
  }
}
