import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    CommonModule
  ]
})
export class LoginComponent implements OnInit, AfterViewInit {
  @ViewChild('emailInput') emailInput!: ElementRef;

  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  returnUrl = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    if (this.authService.isAuthenticated()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  ngAfterViewInit(): void {
    if (this.error && this.emailInput) {
      this.emailInput.nativeElement.focus();
    }
  }

  get f() { return this.loginForm.controls; }

  onSubmit(): void {
    this.submitted = true;
    this.error = '';

    if (this.loginForm.invalid) {
      this.focusFirstErrorField();
      return;
    }

    this.loading = true;

    this.authService.login({
      email: this.f['email'].value,
      password: this.f['password'].value
    }).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: () => {
        this.router.navigate([this.returnUrl]);
      },
      error: error => {
        this.error = error.message || 'Échec de connexion. Vérifiez vos identifiants.';
        this.focusFirstErrorField();
      }
    });
  }

  private focusFirstErrorField(): void {
    setTimeout(() => {
      const firstErrorField = document.querySelector('.form-control--invalid') as HTMLElement;
      if (firstErrorField) {
        firstErrorField.focus();
      }
    }, 100);
  }
}
