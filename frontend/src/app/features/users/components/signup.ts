import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { NotificationService } from '@src/app/core/services/notification.service';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';

@Component({
  selector: 'app-signup',
  imports: [FormsModule, HlmInputImports, HlmButtonImports, RouterLink],
  template: `<section
    class="mx-auto grid min-h-[calc(100vh-9rem)] max-w-6xl items-center gap-8 lg:grid-cols-[0.95fr_1.05fr]"
  >
    <form
      class="rounded-[2rem] border bg-card p-6 shadow-2xl shadow-slate-950/10 dark:shadow-black/30 sm:p-8"
      (ngSubmit)="signup()"
    >
      <h1 class="text-3xl font-black">Create your account</h1>
      <p class="mt-2 text-muted-foreground">Start tracking expenses like a finance creator.</p>
      <div class="mt-8 space-y-4">
        <label class="grid gap-2 text-sm font-semibold">
          Full name
          <input
            hlmInput
            name="fullName"
            [ngModel]="form().fullName"
            (ngModelChange)="patchForm({ fullName: $event })"
            required
            class="h-12 rounded-2xl px-4"
            placeholder="Indra Kumar"
          />
        </label>
        <label class="grid gap-2 text-sm font-semibold">
          Email
          <input
            hlmInput
            name="email"
            type="email"
            [ngModel]="form().email"
            (ngModelChange)="patchForm({ email: $event })"
            required
            class="h-12 rounded-2xl px-4"
            placeholder="you@example.com"
          />
        </label>
        <label class="grid gap-2 text-sm font-semibold">
          Password
          <input
            hlmInput
            name="password"
            type="password"
            [ngModel]="form().password"
            (ngModelChange)="patchForm({ password: $event })"
            required
            class="h-12 rounded-2xl px-4"
            placeholder="At least 8 chars, 1 uppercase, 1 number"
          />
        </label>
        <button hlmBtn class="h-12 w-full rounded-2xl font-bold" type="submit">
          Create account
        </button>
      </div>
      <p class="mt-6 text-center text-sm text-muted-foreground">
        Already have an account?
        <a routerLink="/login" class="font-bold text-primary">Sign in</a>
      </p>
    </form>

    <div class="relative overflow-hidden rounded-[2rem] border bg-card p-8">
      <div
        class="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,--theme(--color-emerald-400/0.18),transparent_18rem),radial-gradient(circle_at_85%_35%,--theme(--color-sky-400/0.16),transparent_22rem)]"
      ></div>
      <div class="relative">
        <p class="text-sm font-semibold uppercase tracking-[0.35em] text-muted-foreground">
          Creator-grade UI
        </p>
        <h2 class="mt-4 text-5xl font-black tracking-tight">
          Dashboards that make budgets feel premium.
        </h2>
        <p class="mt-4 text-lg text-muted-foreground">
          Authenticated routes, API-backed forms, responsive cards, and a theme system built on
          Slate tokens.
        </p>
      </div>
    </div>
  </section>`,
})
export class Signup {
  private readonly authService = inject(AuthService);
  private readonly notifications = inject(NotificationService);
  private readonly router = inject(Router);

  readonly form = signal({
    fullName: '',
    email: '',
    password: '',
  });

  async signup(): Promise<void> {
    try {
      await this.authService.register(this.form());
      this.notifications.success('Account created. Please sign in.');
      void this.router.navigateByUrl('/login');
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to create account.'));
    }
  }

  patchForm(value: Partial<{ fullName: string; email: string; password: string }>): void {
    this.form.update((form) => ({ ...form, ...value }));
  }
}
