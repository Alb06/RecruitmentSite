import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { hasRoleGuard, adminGuard, userGuard } from './role.guard';
import { AuthService } from '../services/auth.service';

describe('Role Guards', () => {
  let authServiceMock: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(() => {
    const authSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'hasRole']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authSpy },
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    authServiceMock = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
  });

  describe('hasRoleGuard', () => {
    it('should allow access when user has the required role', () => {
      authServiceMock.isAuthenticated.and.returnValue(true);
      authServiceMock.hasRole.and.returnValue(true);

      TestBed.runInInjectionContext(() => {
        const guard = hasRoleGuard('Admin');
        const result = guard({} as any, {} as any);
        expect(result).toBeTrue();
        expect(authServiceMock.hasRole).toHaveBeenCalledWith('Admin');
      });
    });
    // ... autres tests
  });

  describe('adminGuard', () => {
    it('should check for Admin role', () => {
      authServiceMock.isAuthenticated.and.returnValue(true);
      authServiceMock.hasRole.and.returnValue(true);

      TestBed.runInInjectionContext(() => {
        const result = adminGuard({} as any, {} as any);
        expect(result).toBeTrue();
        expect(authServiceMock.hasRole).toHaveBeenCalledWith('Admin');
      });
    });
  });

  describe('userGuard', () => {
    it('should check for User role', () => {
      authServiceMock.isAuthenticated.and.returnValue(true);
      authServiceMock.hasRole.and.returnValue(true);

      TestBed.runInInjectionContext(() => {
        const result = userGuard({} as any, {} as any);
        expect(result).toBeTrue();
        expect(authServiceMock.hasRole).toHaveBeenCalledWith('User');
      });
    });
  });
});
