import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { NotificationService } from '@src/app/core/services/notification.service';
import { IncomeService } from '@src/app/features/incomes/services/income.service';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { CategoryService } from '@src/app/features/categories/services/category.service';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { formatDateValue } from '@src/app/core/utils/date-utils';
import {
  lucideBanknoteArrowUp,
  lucideDownload,
  lucidePencil,
  lucidePlus,
  lucideSearch,
  lucideTrash2,
} from '@ng-icons/lucide';
import { CategoryResponse, IncomeResponse } from '@src/app/core/models/models';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmNativeSelectImports } from '@spartan-ng/helm/native-select';

type IncomeForm = {
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  incomeDateUtc: string;
};

type IncomeSort = 'newest' | 'oldest' | 'highest' | 'lowest' | 'title';

@Component({
  selector: 'app-income',
  imports: [FormsModule, NgIcon, HlmInputImports, HlmButtonImports, HlmNativeSelectImports],
  providers: [
    provideIcons({
      lucideBanknoteArrowUp,
      lucideDownload,
      lucidePencil,
      lucidePlus,
      lucideSearch,
      lucideTrash2,
    }),
  ],
  templateUrl: `./income.html`,
  standalone: true,
})
export class Income implements OnInit {
  //region Service
  private readonly authService = inject(AuthService);
  private readonly categoryService = inject(CategoryService);
  private readonly incomeService = inject(IncomeService);
  private readonly notifications = inject(NotificationService);
  //endregion

  //region Signals
  readonly categories = signal<CategoryResponse[]>([]);
  readonly incomes = signal<IncomeResponse[]>([]);
  readonly editingIncomeId = signal<string | null>(null);
  readonly form = signal<IncomeForm>(this.emptyForm());
  readonly searchTerm = signal('');
  readonly sortBy = signal<IncomeSort>('newest');
  readonly filteredIncomes = computed(() => this.sortIncomes(this.applyFilters(this.incomes())));
  readonly totalIncome = computed(() =>
    this.filteredIncomes().reduce((total, income) => total + income.amount, 0),
  );
  readonly formatDateValue = formatDateValue;
  //endregion

  //region HTML Methods
  async ngOnInit(): Promise<void> {
    await this.loadData();
  }

  async saveIncome(): Promise<void> {
    const userId = this.authService.currentUser()?.id;

    const currentForm: IncomeForm = this.form();

    if (!userId || !currentForm.categoryId) {
      this.notifications.error('Choose a category before saving income');
      return;
    }

    const payload: IncomeForm = {
      title: currentForm.title,
      amount: Number(currentForm.amount),
      currency: currentForm.currency.toUpperCase(),
      categoryId: currentForm.categoryId,
      incomeDateUtc: new Date(currentForm.incomeDateUtc).toISOString(),
    };

    const editingId: string | null = this.editingIncomeId();

    try {
      if (editingId) {
        await this.incomeService.update({ id: editingId, ...payload });
        this.notifications.success('Income updated successfully!!');
      } else {
        await this.incomeService.create(payload);
        this.notifications.success('Income created!!');
      }
      this.resetForm();
      await this.loadIncomes();
    } catch (err: unknown) {
      this.notifications.error(getApiErrorMessage(err, 'Unable to save income'));
    }
  }

  editIncome(income: IncomeResponse): void {
    this.editingIncomeId.set(income.id);
    this.form.set({
      title: income.title,
      amount: income.amount,
      currency: income.currency,
      categoryId: income.categoryId,
      incomeDateUtc: income.incomeDateUtc.slice(0, 10),
    });
  }

  async deleteIncome(income: IncomeResponse): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId || !confirm(`Delete "${income.title}"?`)) {
      return;
    }

    try {
      await this.incomeService.delete(income.id);
      await this.loadIncomes();
      this.notifications.success('Income deleted.');
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to delete income.'));
    }
  }

  //endregion

  //region Helper Methods
  resetForm(): void {
    this.editingIncomeId.set(null);
    this.form.set(this.emptyForm());
  }

  patchForm(value: Partial<IncomeForm>): void {
    this.form.update((form) => ({ ...form, ...value }));
  }

  openDatePicker(picker: HTMLInputElement): void {
    picker.showPicker?.();
    picker.focus();
  }

  numberValue(value: unknown): number {
    return Number(value);
  }

  stringValue(value: unknown): string {
    return typeof value === 'string' ? value : '';
  }

  incomeSortValue(value: unknown): IncomeSort {
    return value === 'oldest' || value === 'highest' || value === 'lowest' || value === 'title'
      ? value
      : 'newest';
  }

  formatMoney(amount: number, currency = 'INR'): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency,
      maximumFractionDigits: 0,
    }).format(amount);
  }

  exportCsv(): void {
    const rows = this.filteredIncomes();
    if (!rows.length) {
      this.notifications.info('No income available to export.');
      return;
    }

    const csvRows = [
      ['Title', 'Category', 'Date', 'Amount', 'Currency'],
      ...rows.map((income) => [
        income.title,
        this.categoryName(income.categoryId),
        income.incomeDateUtc.slice(0, 10),
        String(income.amount),
        income.currency,
      ]),
    ];
    const csv = csvRows
      .map((row) => row.map((value) => `"${value.replace(/"/g, '""')}"`).join(','))
      .join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `income-${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(url);
    this.notifications.success('Income CSV exported.');
  }

  private async loadData(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId) {
      return;
    }

    try {
      const categories = await this.categoryService.loadByUser();
      this.categories.set(categories);
      this.patchForm({ categoryId: categories[0]?.id ?? '' });
      await this.loadIncomes();
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to load income.'));
    }
  }

  private async loadIncomes(): Promise<void> {
    this.incomes.set(await this.incomeService.loadByUser());
  }

  private applyFilters(incomes: IncomeResponse[]): IncomeResponse[] {
    const search = this.searchTerm().trim().toLowerCase();
    return incomes.filter((income) => {
      const categoryName = this.categoryName(income.categoryId).toLowerCase();
      return (
        !search || income.title.toLowerCase().includes(search) || categoryName.includes(search)
      );
    });
  }

  categoryName(categoryId: string): string {
    return (
      this.categories().find((category) => category.id === categoryId)?.name ?? 'Uncategorized'
    );
  }

  private sortIncomes(incomes: IncomeResponse[]): IncomeResponse[] {
    const sortBy = this.sortBy();
    return [...incomes].sort((first, second) => {
      if (sortBy === 'oldest') {
        return new Date(first.incomeDateUtc).getTime() - new Date(second.incomeDateUtc).getTime();
      }

      if (sortBy === 'highest') {
        return second.amount - first.amount;
      }

      if (sortBy === 'lowest') {
        return first.amount - second.amount;
      }

      if (sortBy === 'title') {
        return first.title.localeCompare(second.title);
      }

      return new Date(second.incomeDateUtc).getTime() - new Date(first.incomeDateUtc).getTime();
    });
  }

  private emptyForm(): IncomeForm {
    return {
      title: '',
      amount: 0,
      currency: 'INR',
      categoryId: '',
      incomeDateUtc: new Date().toISOString().slice(0, 10),
    };
  }
  //endregion
}
