import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/components/login/login.component';
import { RegisterComponent } from './auth/components/register/register.component';
import { authGuard } from './auth/guards/auth.guard';
import { adminGuard } from './auth/guards/role.guard';

// Composant pour la page non autorisée
import { Component } from '@angular/core';

@Component({
  template: `
    <div class="container mt-5">
      <div class="alert alert-danger">
        <h4>Accès non autorisé</h4>
        <p>Vous n'avez pas les droits nécessaires pour accéder à cette page.</p>
        <button class="btn btn-primary" (click)="goBack()">Retour</button>
      </div>
    </div>
  `,
  standalone: true
})
export class UnauthorizedComponent {
  goBack() {
    window.history.back();
  }
}

const routes: Routes = [
  // Routes d'authentification
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },

  // Redirection vers login par défaut
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
