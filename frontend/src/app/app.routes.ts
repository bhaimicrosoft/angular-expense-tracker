import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { loginGuard } from './core/guards/login.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: 'login',
    loadComponent: () => import('./features/users/components/login').then((m) => m.Login),
    canActivate: [loginGuard]
  },
  {
    path: 'signup',
    loadComponent: () => import('./features/users/components/signup').then((m) => m.Signup),
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
    canActivate: [authGuard],
  },
  {
    path: 'profile',
    loadComponent: () => import('./features/users/components/user').then((m) => m.User),
    canActivate: [authGuard],
  },
  {
    path: 'expenses',
    loadComponent: () =>
      import('./features/expenses/components/expense').then((module) => module.Expense),
    canActivate: [authGuard],
  },
  {
    path: 'income',
    loadComponent: () =>
      import('./features/incomes/components/income').then((module) => module.Income),
    canActivate: [authGuard],
  },
  {
    path: 'budgets',
    loadComponent: () =>
      import('./features/budgets/components/budget').then((module) => module.Budget),
    canActivate: [authGuard],
  },
  {
    path: 'categories',
    loadComponent: () =>
      import('./features/categories/components/category').then((module) => module.Category),
    canActivate: [authGuard],
  },
  {
    path: '**', redirectTo: 'login'
  }
];
