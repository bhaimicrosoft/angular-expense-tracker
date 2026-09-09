import { Injectable, signal } from '@angular/core';

export type NotificationKind = 'success' | 'error' | 'info';

export interface AppNotification {
  message: string;
  kind: NotificationKind;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private readonly notificationSignal = signal<AppNotification | null>(null);
  private hideTimer: number | null = null;

  readonly notification = this.notificationSignal.asReadonly();

  success(message: string): void {
    this.show(message, 'success');
  }
  error(message: string): void {
    this.show(message, 'error');
  }
  info(message: string): void {
    this.show(message, 'info');
  }
  dismiss(): void {
    this.notificationSignal.set(null);
  }

  private show(message: string, kind: NotificationKind): void {
    if (this.hideTimer !== null) {
      window.clearTimeout(this.hideTimer);
    }

    this.notificationSignal.set({ message, kind });
    this.hideTimer = window.setTimeout(() => this.dismiss(), 2000);
  }
}
