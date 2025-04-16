import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Factory de guard qui vérifie si l'utilisateur a un rôle spécifique
 * @param role Le rôle requis pour accéder à la route
 * @returns Un guard qui vérifie si l'utilisateur a le rôle spécifié
 */
export function hasRoleGuard(role: string): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.isAuthenticated() && authService.hasRole(role)) {
      return true;
    }

    // Si l'utilisateur est authentifié mais n'a pas le rôle requis
    if (authService.isAuthenticated()) {
      router.navigate(['/unauthorized']);
      return false;
    }

    // Si l'utilisateur n'est pas authentifié
    router.navigate(['/login']);
    return false;
  };
}

// Guards prédéfinis pour les rôles communs
export const adminGuard: CanActivateFn = hasRoleGuard('Admin');
export const userGuard: CanActivateFn = hasRoleGuard('User');
