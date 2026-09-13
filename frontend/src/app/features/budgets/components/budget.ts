import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { NotificationService } from '@src/app/core/services/notification.service';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { BudgetService } from '@src/app/features/budgets/services/budget.service';
import { CategoryService } from '@src/app/features/categories/services/category.service';
import { BudgetResponse, CategoryResponse } from '@src/app/core/models/models';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';
import { FormsModule } from '@angular/forms';
import { HlmNativeSelectImports } from '@spartan-ng/helm/native-select';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmButtonImports } from '@spartan-ng/helm/button';

type BudgetForm = {
  categoryId: string;
  amount: number;
  currency: string;
  monthText: string;
  year: number;
};

@Component({
  selector: 'app-budget',
  imports: [FormsModule, HlmNativeSelectImports, HlmInputImports, HlmButtonImports],
  templateUrl: './budget.html',
})
export class Budget implements OnInit {
  protected readonly String = String;
  private readonly authService = inject(AuthService);
  private readonly budgetService = inject(BudgetService);
  private readonly categoryService = inject(CategoryService);
  private readonly notifications = inject(NotificationService);

  readonly months: string[] = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December',
  ];

  readonly categories = signal<CategoryResponse[]>([]);
  readonly budgets = signal<BudgetResponse[]>([]);
  readonly editingBudgetId = signal<string | null>(null);
  readonly form = signal<BudgetForm>(this.emptyForm());
  readonly monthNumber = computed(() => Number(this.form().monthText));

  async ngOnInit(): Promise<void> {
    // load categories
    await this.loadCategories();
  }

  private async loadCategories(): Promise<void> {
    const userId = this.authService.currentUser()?.id;

    if (!userId) {
      return;
    }

    try {
      const categories: CategoryResponse[] = await this.categoryService.loadByUser();
      this.categories.set(categories);
      this.patchForm({ categoryId: categories[0]?.id ?? '' });
      await this.loadBudgets();
    } catch (err) {}
  }

  async saveBudget(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    const currentForm = this.form();
    if (!userId || !currentForm.categoryId) {
      this.notifications.error('Choose a category before saving the budget.');
      return;
    }

    const payload = {
      categoryId: currentForm.categoryId,
      amount: Number(currentForm.amount),
      currency: currentForm.currency.toUpperCase(),
      month: this.monthNumber(),
      year: Number(currentForm.year),
    };
    const editingId = this.editingBudgetId();
    try {
      if (editingId) {
        await this.budgetService.update({ id: editingId, ...payload });
      } else {
        await this.budgetService.create(payload);
      }

      this.budgets.set(await this.fetchBudgets());
      this.notifications.success(editingId ? 'Budget updated' : 'Budget created!');
      this.resetForm();
    } catch (err: unknown) {
      this.notifications.error(getApiErrorMessage(err, 'Unable to save budget.'));
    }
  }

  editBudget(budget: BudgetResponse): void {
    this.editingBudgetId.set(budget.id);
    this.form.set({
      categoryId: budget.categoryId,
      amount: budget.amount,
      currency: budget.currency,
      monthText: String(budget.month),
      year: budget.year,
    });
  }

  async deleteBudget(budget: BudgetResponse): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId || !confirm(`Delete budget for ${this.categoryName(budget.categoryId)}?`)) {
      return;
    }

    try {
      await this.budgetService.delete(budget.id);
      this.budgets.set(await this.fetchBudgets());
      this.notifications.success('Budget deleted!');
    } catch (e) {
      this.notifications.error(getApiErrorMessage(e, 'Unable to delete budget.'));
    }
  }

  async loadBudgets(): Promise<void> {
    try {
      this.budgets.set(await this.fetchBudgets());
    } catch (err: unknown) {
      this.notifications.error(getApiErrorMessage(err, 'Unable to load budgets'));
    }
  }

  private fetchBudgets(): Promise<BudgetResponse[]> {
    return this.budgetService.loadByCategories(
      this.categories().map((category) => category.id),
      Number(this.form().year),
      this.monthNumber(),
    );
  }

  patchForm(value: Partial<BudgetForm>) {
    this.form.update((form) => ({ ...form, ...value }));
  }

  categoryName(categoryId: string): string {
    return (
      this.categories().find((category) => category.id === categoryId)?.name ?? 'Uncategorized'
    );
  }

  categoryColor(categoryId: string): string {
    return this.categories().find((category) => category.id === categoryId)?.hexColor ?? '#64748b';
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

  private emptyForm(): BudgetForm {
    const now = new Date();
    return {
      categoryId: '',
      amount: 0,
      currency: 'INR',
      monthText: String(now.getMonth() + 1),
      year: now.getFullYear(),
    };
  }

  resetForm() {
    const { monthText, year } = this.form();
    this.editingBudgetId.set(null);
    this.form.set({
      ...this.emptyForm(),
      monthText,
      year,
      categoryId: this.categories()[0]?.id ?? '',
    });
  }
}
