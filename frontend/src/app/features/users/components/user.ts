import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  lucideBadgeCheck,
  lucideMail,
  lucideSave,
  lucideShieldAlert,
  lucideShieldCheck,
  lucideTrash2,
  lucideUserRound,
  lucideX,
} from '@ng-icons/lucide';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { NotificationService } from '@src/app/core/services/notification.service';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';
import { AuthService } from '@src/app/features/users/services/auth.service';

@Component({
  selector: 'app-user',
  imports: [FormsModule, RouterLink, NgIcon, HlmButtonImports, HlmInputImports],
  providers: [
    provideIcons({
      lucideBadgeCheck,
      lucideMail,
      lucideSave,
      lucideShieldAlert,
      lucideShieldCheck,
      lucideTrash2,
      lucideUserRound,
      lucideX,
    }),
  ],
  template: `
    <section class="mx-auto grid max-w-6xl gap-6 lg:grid-cols-[0.9fr_1.1fr]">
      <div
        class="relative overflow-hidden rounded-[2rem] border bg-card p-6 shadow-2xl shadow-slate-950/10 dark:shadow-black/30 sm:p-8"
      >
        <div
          class="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,--theme(--color-primary/0.20),transparent_18rem),radial-gradient(circle_at_85%_30%,--theme(--color-emerald-400/0.16),transparent_20rem)]"
        ></div>
        <div class="relative">
          <p
            class="inline-flex items-center gap-2 text-sm font-black uppercase tracking-[0.3em] text-muted-foreground"
          >
            <ng-icon name="lucideBadgeCheck" />
            Profile
          </p>
          <div
            class="mt-8 grid size-28 place-items-center rounded-[2rem] bg-primary text-4xl font-black text-primary-foreground shadow-xl"
          >
            {{ initials() }}
          </div>
          <h1 class="mt-6 text-4xl font-black tracking-tight">
            {{ authService.currentUser()?.fullName ?? 'Your profile' }}
          </h1>
          <p class="mt-2 flex items-center gap-2 text-muted-foreground">
            <ng-icon name="lucideMail" />
            {{ authService.currentUser()?.email ?? 'Update your contact details' }}
          </p>

          <div class="mt-8 rounded-3xl border bg-background/70 p-5">
            <p class="flex items-center gap-2 font-bold">
              <ng-icon name="lucideShieldCheck" class="text-emerald-500" />
              Authenticated current-user profile
            </p>
            <p class="mt-2 text-sm text-muted-foreground">
              This page reads and updates the signed-in user through the protected
              <span class="font-mono">/me</span> endpoint.
            </p>
          </div>
        </div>
      </div>

      <form
        class="rounded-[2rem] border bg-card p-6 shadow-2xl shadow-slate-950/10 dark:shadow-black/30 sm:p-8"
        (ngSubmit)="saveProfile()"
      >
        <div class="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p
              class="inline-flex items-center gap-2 text-sm font-black uppercase tracking-[0.3em] text-muted-foreground"
            >
              <ng-icon name="lucideUserRound" />
              Account details
            </p>
            <h2 class="mt-2 text-3xl font-black">Update your profile</h2>
            <p class="mt-2 text-muted-foreground">
              Keep your display name fresh. Your email is locked after account creation.
            </p>
          </div>
          <a routerLink="/dashboard" hlmBtn variant="outline" class="rounded-2xl"
            >Back to dashboard</a
          >
        </div>

        <div class="mt-8 grid gap-5">
          <label class="grid gap-2 text-sm font-semibold">
            Full name
            <input
              hlmInput
              name="fullName"
              [ngModel]="form().fullName"
              (ngModelChange)="patchForm({ fullName: stringValue($event) })"
              required
              class="h-12 rounded-2xl px-4"
              placeholder="Your full name"
            />
          </label>

          <div class="grid gap-2 text-sm font-semibold">
            Email
            <div
              class="flex min-h-12 items-center gap-3 rounded-2xl border bg-muted/60 px-4 text-muted-foreground"
            >
              <ng-icon name="lucideMail" />
              <span class="font-bold">{{ form().email || 'Email unavailable' }}</span>
              <span
                class="ml-auto rounded-full bg-background px-3 py-1 text-xs font-black uppercase tracking-[0.2em]"
                >Locked</span
              >
            </div>
            <p class="text-xs text-muted-foreground">
              Email addresses are unique identifiers and cannot be changed after signup.
            </p>
          </div>

          <button hlmBtn class="h-12 rounded-2xl font-bold" type="submit" [disabled]="isSaving()">
            <ng-icon name="lucideSave" />
            {{ isSaving() ? 'Saving profile...' : 'Save profile' }}
          </button>

          <div class="rounded-3xl border border-destructive/30 bg-destructive/5 p-5">
            <p class="flex items-center gap-2 font-black text-destructive">
              <ng-icon name="lucideShieldAlert" />
              Danger zone
            </p>
            <p class="mt-2 text-sm text-muted-foreground">
              Permanently delete your account and all data linked to it. This action cannot be
              undone.
            </p>
            <button
              hlmBtn
              variant="destructive"
              class="mt-4 rounded-2xl font-bold"
              type="button"
              (click)="openDeleteDialog()"
            >
              <ng-icon name="lucideTrash2" />
              DELETE account
            </button>
          </div>
        </div>
      </form>
    </section>

    @if (isDeleteDialogOpen()) {
      <div
        class="fixed inset-0 z-50 grid place-items-center bg-black/60 p-4 backdrop-blur-sm"
        role="dialog"
        aria-modal="true"
      >
        <div
          class="w-full max-w-md rounded-[2rem] border border-destructive/30 bg-card p-6 shadow-2xl"
        >
          <div class="flex items-start justify-between gap-4">
            <div>
              <p
                class="inline-flex items-center gap-2 text-sm font-black uppercase tracking-[0.3em] text-destructive"
              >
                <ng-icon name="lucideShieldAlert" />
                Confirm delete
              </p>
              <h2 class="mt-3 text-2xl font-black">Delete your account?</h2>
            </div>
            <button
              hlmBtn
              variant="ghost"
              size="icon"
              type="button"
              (click)="closeDeleteDialog()"
              [disabled]="isDeleting()"
            >
              <ng-icon name="lucideX" />
            </button>
          </div>

          <p class="mt-4 text-sm text-muted-foreground">
            This will permanently delete your account. You will be signed out immediately after
            deletion.
          </p>

          <div class="mt-6 grid gap-3 sm:grid-cols-2">
            <button
              hlmBtn
              variant="outline"
              class="h-11 rounded-2xl font-bold"
              type="button"
              (click)="closeDeleteDialog()"
              [disabled]="isDeleting()"
            >
              Cancel
            </button>
            <button
              hlmBtn
              variant="destructive"
              class="h-11 rounded-2xl bg-red-600 font-black text-white hover:bg-red-700"
              type="button"
              (click)="confirmDeleteAccount()"
              [disabled]="isDeleting()"
            >
              {{ isDeleting() ? 'Deleting...' : 'Confirm DELETE' }}
            </button>
          </div>
        </div>
      </div>
    }
  `,
})
export class User implements OnInit {
  readonly authService = inject(AuthService);
  private readonly notifications = inject(NotificationService);

  readonly isSaving = signal(false);
  readonly isDeleting = signal(false);
  readonly isDeleteDialogOpen = signal(false);
  readonly form = signal({
    fullName: '',
    email: '',
  });
  readonly initials = computed(() => {
    const name = this.authService.currentUser()?.fullName ?? this.form().fullName;
    return (
      name
        .split(' ')
        .filter(Boolean)
        .slice(0, 2)
        .map((part) => part[0]?.toUpperCase() ?? '')
        .join('') || '₹'
    );
  });

  async ngOnInit(): Promise<void> {
    this.patchFromUser(this.authService.currentUser());

    try {
      const user = await this.authService.loadCurrentUser();
      this.patchFromUser(user);
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to load profile.'));
    }
  }

  async saveProfile(): Promise<void> {
    const currentForm = this.form();
    const email = this.authService.currentUser()?.email ?? currentForm.email;
    if (!currentForm.fullName.trim()) {
      this.notifications.error('Full name is required.');
      return;
    }

    if (!email.trim()) {
      this.notifications.error('Profile email is unavailable. Refresh and try again.');
      return;
    }

    this.isSaving.set(true);
    try {
      const user = await this.authService.updateCurrentUser({
        fullName: currentForm.fullName.trim(),
        email: email.trim(),
      });
      this.patchFromUser(user);
      this.notifications.success('Profile updated.');
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to update profile.'));
    } finally {
      this.isSaving.set(false);
    }
  }

  patchForm(value: Partial<{ fullName: string; email: string }>): void {
    this.form.update((form) => ({ ...form, ...value }));
  }

  openDeleteDialog(): void {
    this.isDeleteDialogOpen.set(true);
  }

  closeDeleteDialog(): void {
    if (this.isDeleting()) {
      return;
    }

    this.isDeleteDialogOpen.set(false);
  }

  async confirmDeleteAccount(): Promise<void> {
    this.isDeleting.set(true);
    try {
      await this.authService.deleteCurrentUser();
      this.notifications.success('Account deleted.');
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to delete account.'));
      this.isDeleting.set(false);
      this.isDeleteDialogOpen.set(false);
    }
  }

  stringValue(value: unknown): string {
    return typeof value === 'string' ? value : '';
  }

  private patchFromUser(user: { fullName: string; email: string } | null): void {
    if (!user) {
      return;
    }

    this.form.set({
      fullName: user.fullName,
      email: user.email,
    });
  }
}
