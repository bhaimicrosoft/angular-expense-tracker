import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideMoonStar, lucideSun } from '@ng-icons/lucide';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NotificationKind, NotificationService } from './core/services/notification.service';
import { AuthService } from './features/users/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [NgIcon, RouterLink, RouterLinkActive, RouterOutlet],
  providers: [provideIcons({ lucideMoonStar, lucideSun })],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  readonly authService = inject(AuthService);
  readonly notifications = inject(NotificationService);
  protected readonly title = signal('angular-expense-tracker');
  readonly themeLabel = computed(() =>
    this.isDark() ? 'Switch to light theme' : 'Switch to dark theme',
  );
  readonly themeIcon = computed(() => (this.isDark() ? 'lucideSun' : 'lucideMoonStar'));
  protected readonly isDark = signal(false);

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('expense-tracker-theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    this.setTheme(savedTheme ? savedTheme === 'dark' : prefersDark);
  }

  toggleTheme(): void {
    this.setTheme(!this.isDark());
  }

  logout(): void {
    this.notifications.info('Signed out.');
    this.authService.logout();
  }

  notificationClass(kind: NotificationKind): string {
    const base =
      'fixed right-4 top-24 z-50 max-w-sm rounded-2xl border px-5 py-4 text-sm font-bold shadow-2xl';
    const variants: Record<NotificationKind, string> = {
      error: 'border-red-500/40 bg-red-50 text-red-700 dark:bg-red-950 dark:text-red-200',
      success:
        'border-emerald-500/40 bg-emerald-50 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-200',
      info: 'border-sky-500/40 bg-sky-50 text-sky-700 dark:bg-sky-950 dark:text-sky-200',
    };

    return `${base} ${variants[kind]}`;
  }

  private setTheme(isDark: boolean): void {
    this.isDark.set(isDark);
    document.documentElement.classList.toggle('dark', isDark);
    localStorage.setItem('expense-tracker-theme', isDark ? 'dark' : 'light');
  }
}
