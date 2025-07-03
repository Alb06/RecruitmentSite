import { Routes } from '@angular/router';
import { adminGuard } from './auth/guards/role.guard';

export const routes: Routes = [
  // Routes d'authentification
  {
    path: 'login',
    loadComponent: () => import('./auth/components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./auth/components/register/register.component').then(m => m.RegisterComponent)
  },

  // Route non autorisée
  {
    path: 'unauthorized',
    loadComponent: () => import('./shared/components/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent)
  },

  // Routes protégées (exemples à adapter selon vos besoins futurs)
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./admin/admin.component').then(m => m.AdminComponent) // À créer si nécessaire
  },

  // Redirections
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
