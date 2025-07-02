import { TestBed } from '@angular/core/testing';
import { JwtService } from './jwt.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

describe('JwtService', () => {
  let service: JwtService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        JwtService,
        { provide: AuthService, useValue: jasmine.createSpyObj('AuthService', ['logout']) },
        { provide: Router, useValue: jasmine.createSpyObj('Router', ['navigate']) },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(JwtService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
