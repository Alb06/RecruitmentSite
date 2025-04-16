import { inject } from '@angular/core';
import { CanActivateFn, CanActivateChildFn, CanMatchFn, Route, Router, UrlSegment, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Vérifie si l'utilisateur est authentifié et possède les rôles requis
 */
const checkAuth = (
  url: string,
  route: ActivatedRouteSnapshot | null
): boolean => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    // Vérifier si la route requiert un rôle spécifique
    const requiredRoles = route?.data?.['roles'] as string[];

    if (requiredRoles) {
      const hasRequiredRole = requiredRoles.some(role =>
        authService.hasRole(role)
      );

      if (!hasRequiredRole) {
        router.navigate(['/unauthorized']);
        return false;
      }
    }

    return true;
  }

  // Stocker l'URL actuelle pour y revenir après connexion
  router.navigate(['/login'], { queryParams: { returnUrl: url } });
  return false;
};

/**
 * Guard pour protéger les routes
 */
export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  return checkAuth(state.url, route);
};

/**
 * Guard pour protéger les routes enfants
 */
export const authGuardChild: CanActivateChildFn = (
  childRoute: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  return checkAuth(state.url, childRoute);
};

/**
 * Guard pour le chargement paresseux
 */
export const authGuardMatch: CanMatchFn = (
  route: Route,
  segments: UrlSegment[]
) => {
  const url = segments.map(s => `/${s.path}`).join('');
  return checkAuth(url, null);
};
