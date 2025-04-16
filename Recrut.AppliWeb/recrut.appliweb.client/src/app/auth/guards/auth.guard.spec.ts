import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';

describe('authGuard', () => {
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

  it('should allow access when user is authenticated', () => {
    authServiceMock.isAuthenticated.and.returnValue(true);

    const routeMock = {
      data: {}
    } as unknown as ActivatedRouteSnapshot;

    const stateMock = {
      url: '/protected'
    } as RouterStateSnapshot;

    TestBed.runInInjectionContext(() => {
      const result = authGuard(routeMock, stateMock);
      expect(result).toBeTrue();
      expect(authServiceMock.isAuthenticated).toHaveBeenCalled();
    });
  });

  it('should redirect to login when user is not authenticated', () => {
    authServiceMock.isAuthenticated.and.returnValue(false);

    const routeMock = {
      data: {}
    } as unknown as ActivatedRouteSnapshot;

    const stateMock = {
      url: '/protected'
    } as RouterStateSnapshot;

    TestBed.runInInjectionContext(() => {
      const result = authGuard(routeMock, stateMock);
      expect(result).toBeFalse();
      expect(routerMock.navigate).toHaveBeenCalledWith(
        ['/login'],
        { queryParams: { returnUrl: '/protected' } }
      );
    });
  });

  it('should check for required roles when specified', () => {
    authServiceMock.isAuthenticated.and.returnValue(true);
    authServiceMock.hasRole.and.returnValue(true);

    const routeMock = {
      data: { roles: ['Admin'] }
    } as unknown as ActivatedRouteSnapshot;

    const stateMock = {
      url: '/admin'
    } as RouterStateSnapshot;

    TestBed.runInInjectionContext(() => {
      const result = authGuard(routeMock, stateMock);
      expect(result).toBeTrue();
      expect(authServiceMock.hasRole).toHaveBeenCalledWith('Admin');
    });
  });

  it('should redirect to unauthorized when user does not have required role', () => {
    authServiceMock.isAuthenticated.and.returnValue(true);
    authServiceMock.hasRole.and.returnValue(false);

    const routeMock = {
      data: { roles: ['Admin'] }
    } as unknown as ActivatedRouteSnapshot;

    const stateMock = {
      url: '/admin'
    } as RouterStateSnapshot;

    TestBed.runInInjectionContext(() => {
      const result = authGuard(routeMock, stateMock);
      expect(result).toBeFalse();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/unauthorized']);
    });
  });
});
