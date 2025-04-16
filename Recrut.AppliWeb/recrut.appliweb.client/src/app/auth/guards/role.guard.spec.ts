import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { hasRoleGuard, adminGuard, userGuard } from './role.guard';
import { AuthService } from '../services/auth.service';

describe('Role Guards', () => {
  let authServiceMock: jasmine.SpyObj<AuthService>;
  let routerMock: jasmine.SpyObj<Router>;

  beforeEach(() => {
    authServiceMock = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'hasRole']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });
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

    it('should redirect to unauthorized when user does not have the required role', () => {
      authServiceMock.isAuthenticated.and.returnValue(true);
      authServiceMock.hasRole.and.returnValue(false);

      TestBed.runInInjectionContext(() => {
        const guard = hasRoleGuard('Admin');
        const result = guard({} as any, {} as any);
        expect(result).toBeFalse();
        expect(routerMock.navigate).toHaveBeenCalledWith(['/unauthorized']);
      });
    });

    it('should redirect to login when user is not authenticated', () => {
      authServiceMock.isAuthenticated.and.returnValue(false);

      TestBed.runInInjectionContext(() => {
        const guard = hasRoleGuard('Admin');
        const result = guard({} as any, {} as any);
        expect(result).toBeFalse();
        expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
      });
    });
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
