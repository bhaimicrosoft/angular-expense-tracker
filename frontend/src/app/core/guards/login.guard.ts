import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { inject } from '@angular/core';

export const loginGuard: CanActivateFn = () => {
    const authService: AuthService =inject(AuthService);
    const router:Router = inject(Router);

    return authService.isAuthenticated() ? router.createUrlTree(['/dashboard']) : true;
};
