import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { NotificationService } from '@src/app/core/services/notification.service';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';

@Component({
  selector: 'app-login',
  imports: [FormsModule, HlmButtonImports, HlmInputImports, RouterLink],
  template: `<section
    class="mx-auto grid min-h-[calc(100vh-9rem)] max-w-6xl items-center gap-8 lg:grid-cols-[1.05fr_0.95fr]"
  >
    <div class="space-y-6">
      <p class="text-sm font-semibold uppercase tracking-[0.35em] text-muted-foreground">
        Expense Tracker Pro
      </p>
      <h1 class="text-5xl font-black tracking-tight sm:text-7xl">
        Your money, beautifully under control.
      </h1>
      <p class="max-w-xl text-lg text-muted-foreground">
        Sign in to unlock cinematci dashboards, clean budget guardrails, and expense stories your
        customer will remember.
      </p>
      <div class="grid gap-3 sm:grid-cols-3">
        <div class="rounded-3xl border bg-card p-5">
          <p class="text-2xl font-black">Live</p>
          <p class="text-sm text-muted-foreground">API backed</p>
        </div>
        <div class="rounded-3xl border bg-card p-5">
          <p class="text-2xl font-black">Slate</p>
          <p class="text-sm text-muted-foreground">Spartan Theme</p>
        </div>
        <div class="rounded-3xl border bg-card p-5">
          <p class="text-2xl font-black">Dark</p>
          <p class="text-sm text-muted-foreground">Ready mode</p>
        </div>
      </div>
    </div>

    <form
      class="rounded-[2rem] border bg-card p-6 shadow-2xl shadow-slate-950/10 dark:shadow-black/30 sm:p-8"
      (ngSubmit)="login()"
    >
      <h2 class="text-3xl font-black">Welcome back</h2>
      <p class="mt-2 text-muted-foreground">Enter your credentials to continue.</p>
      <div class="mt-8 space-y-4">
        <label for="email" class="grid gap-2 text-sm font-semibold">
          Email
          <input
            hlmInput
            type="email"
            name="email"
            id="email"
            [ngModel]="form().email"
            (ngModelChange)="patchForm({ email: $event })"
            class="h-12 rounded-2xl px-4"
            placeholder="you@example.com"
          />
        </label>
        <label for="password" class="grid gap-2 text-sm font-semibold">
          Password
          <input
            hlmInput
            type="password"
            name="password"
            id="password"
            [ngModel]="form().password"
            (ngModelChange)="patchForm({ password: $event })"
            class="h-12 rounded-2xl px-4"
            placeholder="************"
          />
        </label>
        <button hlmBtn class="h-12 w-full rounded-2xl font-bold" type="submit">Sign in</button>
      </div>

      <p class="mt-6 text-center text-sm text-muted-foreground">
        New here?
        <a routerLink="/signup" class="font-bold text-primary">Create your account</a>
      </p>
    </form>
  </section>`,
})
export class Login implements OnInit {
  private readonly authService: AuthService = inject(AuthService);
  private readonly notifications: NotificationService = inject(NotificationService);
  private readonly route: ActivatedRoute = inject(ActivatedRoute);
  private readonly router: Router = inject(Router);

  readonly form = signal({
    email: '',
    password: '',
  });

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigateByUrl('/dashboard').then();
    }
  }

  async login(): Promise<void> {
    if (this.authService.isAuthenticated()) {
      this.router.navigateByUrl('/dashboard').then();
      return;
    }

    try {
      await this.authService.login(this.form());
      this.notifications.success('Signed in successfully!');
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/dashboard';
      this.router.navigateByUrl(returnUrl).then();
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Invalid email or password!'));
    }
  }

  patchForm(value: Partial<{ email: string; password: string }>): void {
    this.form.update((formValue) => ({ ...formValue, ...value }));
  }
}
