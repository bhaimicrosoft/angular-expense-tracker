import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { inject } from '@angular/core';
import { NotificationService } from '@src/app/core/services/notification.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);
  const notificationService = inject(NotificationService);

  if (authService.isAuthenticated()) {
    return true;
  }

  notificationService.error('You are not authenticated, login first!');
  return router.createUrlTree(['login'], { queryParams: { returnUrl: state.url } });
};
