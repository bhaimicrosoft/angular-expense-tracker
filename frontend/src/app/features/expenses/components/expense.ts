import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { CategoryService } from '@src/app/features/categories/services/category.service';
import { ExpenseService } from '@src/app/features/expenses/services/expense.service';
import { NotificationService } from '@src/app/core/services/notification.service';
import { CategoryResponse, ExpenseResponse } from '@src/app/core/models/models';
import { DateRange, filterByDateRange, formatDateValue } from '@src/app/core/utils/date-utils';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  lucideDownload,
  lucidePencil,
  lucidePlus,
  lucideSearch,
  lucideSlidersHorizontal,
  lucideTrash2,
} from '@ng-icons/lucide';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmNativeSelectImports } from '@spartan-ng/helm/native-select';
import { HlmButtonImports } from '@spartan-ng/helm/button';

type ExpenseForm = {
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  expenseDateUtc: string;
};

type ExpenseSort = 'newest' | 'oldest' | 'highest' | 'lowest' | 'title';

@Component({
  selector: 'app-expense',
  imports: [
    NgIcon,
    FormsModule,
    CommonModule,
    HlmInputImports,
    HlmNativeSelectImports,
    HlmButtonImports,
  ],
  providers: [
    provideIcons({
      lucideDownload,
      lucidePencil,
      lucidePlus,
      lucideSearch,
      lucideSlidersHorizontal,
      lucideTrash2,
    }),
  ],
  templateUrl: './expense.html',
})
export class Expense implements OnInit {
  // services
  private readonly authService: AuthService = inject(AuthService);
  private readonly categoryService: CategoryService = inject(CategoryService);
  private readonly expenseService: ExpenseService = inject(ExpenseService);
  private readonly notificationService: NotificationService = inject(NotificationService);

  // signals
  readonly categories = signal<CategoryResponse[]>([]);
  readonly expenses = signal<ExpenseResponse[]>([]);
  readonly editingExpenseId = signal<string | null>(null);
  readonly dateRange = signal<DateRange>({ start: '', end: '' });
  readonly form = signal<ExpenseForm>(this.emptyForm());
  readonly searchTerm = signal<string>('');
  readonly selectedCategoryId = signal('');
  readonly sortBy = signal<ExpenseSort>('newest');
  readonly minAmount = signal<number | null>(null);
  readonly maxAmount = signal<number | null>(null);
  readonly filteredExpenses = computed(() =>
    this.sortExpenses(this.applyFilters(filterByDateRange(this.expenses(), this.dateRange()))),
  );
  readonly formatDateValue = formatDateValue;

  async ngOnInit(): Promise<void> {
    this.setCurrentMonth();
    await this.loadData();
  }

  async saveExpense(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    const currentForm = this.form();

    if (!userId || !currentForm.categoryId) {
      this.notificationService.error('Choose a category before saving the expense.');
      return;
    }

    const payload: ExpenseForm = {
      title: currentForm.title,
      amount: Number(currentForm.amount),
      currency: currentForm.currency.toUpperCase(),
      categoryId: currentForm.categoryId,
      expenseDateUtc: new Date(currentForm.expenseDateUtc).toISOString(),
    };

    const editingId = this.editingExpenseId();

    try {
      if (editingId) {
        await this.expenseService.update({ id: editingId, ...payload });
        this.notificationService.success('Expense updated successfully!!');
      } else {
        await this.expenseService.create(payload);
        this.notificationService.success('Expense added successfully!!');
      }
      await this.loadExpenses();
    } catch (err: unknown) {
      this.notificationService.error(getApiErrorMessage(err, 'Unable to save expense.'));
    }
  }

  editExpense(expense: ExpenseResponse): void {
    const { title, amount, currency, id, categoryId } = expense;
    this.editingExpenseId.set(id);
    this.form.set({
      title,
      amount,
      currency,
      categoryId,
      expenseDateUtc: expense.expenseDateUtc.slice(0, 10),
    });
  }

  async deleteExpense(expense: ExpenseResponse): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId || !confirm(`Delete "${expense.title}"?`)) {
      return;
    }

    try {
      await this.expenseService.delete(expense.id);
      await this.loadExpenses();
      this.notificationService.success('Expense Deleted!');
    } catch (err) {
      this.notificationService.error(getApiErrorMessage(err, 'Unable to delete expense'));
    }
  }

  private emptyForm(): ExpenseForm {
    return {
      title: '',
      amount: 0,
      currency: 'INR',
      categoryId: '',
      expenseDateUtc: new Date().toISOString().slice(0, 10),
    };
  }

  private sortExpenses(expenses: ExpenseResponse[]): ExpenseResponse[] {
    const sortBy = this.sortBy();
    return [...expenses].sort((first, second) => {
      if (sortBy === 'oldest') {
        return new Date(first.expenseDateUtc).getTime() - new Date(second.expenseDateUtc).getTime();
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

      return new Date(second.expenseDateUtc).getTime() - new Date(first.expenseDateUtc).getTime();
    });
  }

  private applyFilters(expenses: ExpenseResponse[]): ExpenseResponse[] {
    const search = this.searchTerm().trim().toLowerCase();
    const categoryId = this.selectedCategoryId();
    const minAmount = this.minAmount();
    const maxAmount = this.maxAmount();

    return expenses.filter((expense) => {
      const categoryName = this.categoryName(expense.categoryId).toLowerCase();
      const matchesSearch =
        !search || expense.title.toLowerCase().includes(search) || categoryName.includes(search);
      const matchesCategory = !categoryId || expense.categoryId === categoryId;
      const matchesMin = minAmount === null || expense.amount >= minAmount;
      const matchesMax = maxAmount === null || expense.amount <= maxAmount;
      return matchesSearch && matchesCategory && matchesMin && matchesMax;
    });
  }

  categoryName(categoryId: string): string {
    return (
      this.categories().find((category) => category.id === categoryId)?.name ?? 'Uncategorized'
    );
  }

  formatMoney(amount: number, currency: string): string {
    return new Intl.NumberFormat('en-IN', { style: 'currency', currency }).format(amount);
  }

  stringValue(value: unknown): string {
    return typeof value === 'string' ? value : '';
  }

  numberValue(value: unknown): number {
    return Number(value);
  }

  expenseSortValue(value: unknown): ExpenseSort {
    return value === 'oldest' || value === 'highest' || value === 'lowest' || value === 'title'
      ? value
      : 'newest';
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedCategoryId.set('');
    this.minAmount.set(null);
    this.maxAmount.set(null);
    this.setCurrentMonth();
  }

  resetForm(): void {
    this.editingExpenseId.set(null);
    this.form.set({ ...this.emptyForm(), categoryId: this.categories()[0]?.id ?? '' });
  }

  patchForm(value: Partial<ExpenseForm>): void {
    this.form.update((form) => ({ ...form, ...value }));
  }

  patchDateRange(value: Partial<DateRange>): void {
    this.dateRange.update((range) => ({ ...range, ...value }));
  }

  openDatePicker(picker: HTMLInputElement): void {
    picker.showPicker?.();
    picker.focus();
  }

  setCurrentMonth(): void {
    const now = new Date();
    this.dateRange.set({
      start: new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0, 10),
      end: new Date(now.getFullYear(), now.getMonth() + 1, 0).toISOString().slice(0, 10),
    });
  }

  exportCsv(): void {
    const rows = this.filteredExpenses();
    if (!rows.length) {
      this.notificationService.info('No expenses available to export.');
      return;
    }

    const csvRows = [
      ['Title', 'Category', 'Date', 'Amount', 'Currency'],
      ...rows.map((expense) => [
        expense.title,
        this.categoryName(expense.categoryId),
        expense.expenseDateUtc.slice(0, 10),
        String(expense.amount),
        expense.currency,
      ]),
    ];
    const csv = csvRows
      .map((row) => row.map((value) => `"${value.replace(/"/g, '""')}"`).join(','))
      .join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `expenses-${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(url);
    this.notificationService.success('Expense CSV exported.');
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
      await this.loadExpenses();
    } catch (error: unknown) {
      this.notificationService.error(getApiErrorMessage(error, 'Unable to load expense data.'));
    }
  }

  private async loadExpenses(): Promise<void> {
    this.expenses.set(await this.expenseService.loadByUser());
  }
}
