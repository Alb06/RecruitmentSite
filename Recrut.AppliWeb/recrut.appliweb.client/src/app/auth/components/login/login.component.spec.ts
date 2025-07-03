import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';
import { of, throwError } from 'rxjs';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceMock: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['login', 'isAuthenticated']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent, ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authSpy },
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    authServiceMock = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should apply correct CSS classes from design system', () => {
    const compiled = fixture.nativeElement;

    expect(compiled.querySelector('.form-container')).toBeTruthy();
    expect(compiled.querySelector('.btn--primary')).toBeTruthy();
    expect(compiled.querySelector('.form-control')).toBeTruthy();

    // Vérifier qu'aucune classe Bootstrap n'est présente
    expect(compiled.querySelector('.card')).toBeFalsy();
    expect(compiled.querySelector('.btn-primary')).toBeFalsy();
  });

  it('should have proper accessibility attributes', () => {
    const emailInput = fixture.nativeElement.querySelector('#email');
    const passwordInput = fixture.nativeElement.querySelector('#password');

    expect(emailInput.getAttribute('aria-describedby')).toContain('email-error');
    expect(passwordInput.getAttribute('aria-describedby')).toContain('password-error');
  });

  it('should show validation errors with design system classes', () => {
    component.onSubmit();
    fixture.detectChanges();

    const emailInput = fixture.nativeElement.querySelector('#email');
    const errorMessage = fixture.nativeElement.querySelector('.validation-message--error');

    expect(emailInput.classList.contains('form-control--invalid')).toBeTrue();
    expect(errorMessage).toBeTruthy();
  });

  it('should call login service on valid form submission', () => {
    authServiceMock.login.and.returnValue(of({ token: 'fake-token' }));

    component.loginForm.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });

    component.onSubmit();

    expect(authServiceMock.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123'
    });
  });

  it('should display error message with alert--error class', () => {
    authServiceMock.login.and.returnValue(throwError(() => ({ message: 'Invalid credentials' })));

    component.loginForm.patchValue({
      email: 'test@example.com',
      password: 'wrongpassword'
    });

    component.onSubmit();
    fixture.detectChanges();

    const errorAlert = fixture.nativeElement.querySelector('.alert--error');
    expect(errorAlert).toBeTruthy();
    expect(errorAlert.textContent).toContain('Invalid credentials');
  });

  it('should focus first error field on validation failure', async () => {
    spyOn(component as any, 'focusFirstErrorField');

    component.onSubmit();

    expect((component as any).focusFirstErrorField).toHaveBeenCalled();
  });
});
