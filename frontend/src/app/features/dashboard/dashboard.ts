import { Component, computed, inject, OnInit, signal } from '@angular/core';
import {
  BudgetResponse,
  CategoryResponse,
  ExpenseResponse,
  IncomeResponse,
} from '@src/app/core/models/models';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '@src/app/core/services/notification.service';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { BudgetService } from '@src/app/features/budgets/services/budget.service';
import { CategoryService } from '@src/app/features/categories/services/category.service';
import { ExpenseService } from '@src/app/features/expenses/services/expense.service';
import { IncomeService } from '@src/app/features/incomes/services/income.service';
import { DateRange, filterByDateRange, formatDateValue } from '@src/app/core/utils/date-utils';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  lucideActivity,
  lucideBadgeAlert,
  lucideBanknoteArrowUp,
  lucideChartNoAxesColumnIncreasing,
  lucideCircleDollarSign,
  lucidePieChart,
  lucideReceiptText,
  lucideSparkles,
  lucideTarget,
  lucideTrendingUp,
  lucideX,
} from '@ng-icons/lucide';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { RouterLink } from '@angular/router';
import { HlmNativeSelectImports } from '@spartan-ng/helm/native-select';

type QuickAddMode = 'income' | 'expense' | null;

type MoneyForm = {
  title: string;
  amount: number;
  currency: string;
  date: string;
  categoryId: string;
};

type StatCard = {
  label: string;
  value: string;
  hint: string;
  tone: string;
  icon: string;
};

type CategorySpend = {
  id: string;
  name: string;
  color: string;
  amount: number;
  percent: number;
  currency: string;
};

type CashFlowPoint = {
  label: string;
  income: number;
  expense: number;
  incomeHeight: number;
  expenseHeight: number;
};

type BudgetRow = {
  category: CategoryResponse;
  spent: number;
  limit: number;
  currency: string;
  percent: number;
  tone: string;
};

type BudgetAlert = BudgetRow & {
  message: string;
};

@Component({
  selector: 'app-dashboard',
  imports: [
    FormsModule,
    NgIcon,
    HlmInputImports,
    HlmButtonImports,
    RouterLink,
    HlmNativeSelectImports,
  ],
  providers: [
    provideIcons({
      lucideActivity,
      lucideBadgeAlert,
      lucideBanknoteArrowUp,
      lucideChartNoAxesColumnIncreasing,
      lucideCircleDollarSign,
      lucidePieChart,
      lucideReceiptText,
      lucideSparkles,
      lucideTarget,
      lucideTrendingUp,
      lucideX,
    }),
  ],
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  //region Services
  protected readonly Math = Math;
  private readonly authService = inject(AuthService);
  private readonly budgetService = inject(BudgetService);
  private readonly categoryService = inject(CategoryService);
  private readonly expenseService = inject(ExpenseService);
  private readonly incomeService = inject(IncomeService);
  private readonly notifications = inject(NotificationService);

  //endregion

  //region Signals
  readonly categories = signal<CategoryResponse[]>([]);
  readonly expenses = signal<ExpenseResponse[]>([]);
  readonly incomes = signal<IncomeResponse[]>([]);
  readonly budgets = signal<BudgetResponse[]>([]);
  readonly dateRange = signal<DateRange>({ start: '', end: '' });
  readonly quickAddMode = signal<QuickAddMode>(null);
  readonly quickAddForm = signal<MoneyForm>(this.emptyQuickAddForm());

  readonly firstName = computed(
    () => this.authService.currentUser()?.fullName.split(' ')[0] ?? 'Creator',
  );
  readonly filteredExpenses = computed(() => filterByDateRange(this.expenses(), this.dateRange()));
  readonly filteredIncomes = computed(() => this.filterIncomesByDateRange(this.incomes()));
  readonly formatDateValue = formatDateValue;
  readonly totalSpent = computed(() =>
    this.filteredExpenses().reduce((total, expense) => total + expense.amount, 0),
  );
  readonly totalIncome = computed(() =>
    this.filteredIncomes().reduce((total, income) => total + income.amount, 0),
  );
  readonly balance = computed(() => this.totalIncome() - this.totalSpent());
  readonly savingsRate = computed(() =>
    this.totalIncome() ? Math.round((this.balance() / this.totalIncome()) * 100) : 0,
  );
  readonly totalBudget = computed(() =>
    this.budgets().reduce((total, budget) => total + budget.amount, 0),
  );
  readonly dailyAverage = computed(() => this.totalSpent() / Math.max(this.daysInRange(), 1));
  readonly projectedSpend = computed(() => this.dailyAverage() * this.daysInCurrentMonth());
  readonly biggestExpense = computed(() =>
    this.filteredExpenses().reduce<ExpenseResponse | null>(
      (biggest, expense) => (!biggest || expense.amount > biggest.amount ? expense : biggest),
      null,
    ),
  );
  readonly recentExpenses = computed(() =>
    [...this.filteredExpenses()]
      .sort((a, b) => new Date(b.expenseDateUtc).getTime() - new Date(a.expenseDateUtc).getTime())
      .slice(0, 6),
  );
  readonly stats = computed<StatCard[]>(() => {
    const usage = this.totalBudget()
      ? Math.round((this.totalSpent() / this.totalBudget()) * 100)
      : 0;
    return [
      {
        label: 'Income in range',
        value: this.formatMoney(this.totalIncome()),
        hint: `${this.filteredIncomes().length} income entries`,
        tone: 'text-emerald-500',
        icon: 'lucideBanknoteArrowUp',
      },
      {
        label: 'Spent in range',
        value: this.formatMoney(this.totalSpent()),
        hint: `${this.filteredExpenses().length} transactions`,
        tone: 'text-muted-foreground',
        icon: 'lucideReceiptText',
      },
      {
        label: 'Current balance',
        value: this.formatMoney(this.balance()),
        hint: `${this.savingsRate()}% savings rate`,
        tone: this.balance() >= 0 ? 'text-emerald-500' : 'text-destructive',
        icon: 'lucideCircleDollarSign',
      },
      {
        label: 'Budget runway',
        value: this.formatMoney(Math.max(this.totalBudget() - this.totalSpent(), 0)),
        hint: `${usage}% of budget used`,
        tone: usage > 90 ? 'text-destructive' : 'text-emerald-500',
        icon: 'lucideTarget',
      },
    ];
  });
  readonly cashFlowTrend = computed<CashFlowPoint[]>(() => {
    const now = new Date();
    const months = Array.from({ length: 6 }, (_, index) => {
      const date = new Date(now.getFullYear(), now.getMonth() - 5 + index, 1);
      return {
        key: `${date.getFullYear()}-${date.getMonth()}`,
        label: date.toLocaleString('en-IN', { month: 'short' }),
        income: 0,
        expense: 0,
      };
    });

    for (const income of this.incomes()) {
      const date = new Date(income.incomeDateUtc);
      const month = months.find((item) => item.key === `${date.getFullYear()}-${date.getMonth()}`);
      if (month) {
        month.income += income.amount;
      }
    }

    for (const expense of this.expenses()) {
      const date = new Date(expense.expenseDateUtc);
      const month = months.find((item) => item.key === `${date.getFullYear()}-${date.getMonth()}`);
      if (month) {
        month.expense += expense.amount;
      }
    }

    const max = Math.max(...months.flatMap((month) => [month.income, month.expense]), 1);
    return months.map((month) => ({
      label: month.label,
      income: month.income,
      expense: month.expense,
      incomeHeight: month.income ? Math.max((month.income / max) * 100, 8) : 3,
      expenseHeight: month.expense ? Math.max((month.expense / max) * 100, 8) : 3,
    }));
  });
  readonly categoryBreakdown = computed<CategorySpend[]>(() => {
    const totals = new Map<string, number>();
    for (const expense of this.filteredExpenses()) {
      totals.set(expense.categoryId, (totals.get(expense.categoryId) ?? 0) + expense.amount);
    }

    const total = this.totalSpent();
    return [...totals.entries()]
      .map(([categoryId, amount]) => {
        const category = this.categories().find((item) => item.id === categoryId);
        return {
          id: categoryId,
          name: category?.name ?? 'Uncategorized',
          color: category?.hexColor ?? '#64748b',
          amount,
          percent: total ? Math.round((amount / total) * 100) : 0,
          currency: this.currencyForCategory(categoryId),
        };
      })
      .sort((a, b) => b.amount - a.amount);
  });
  readonly categoryDonut = computed(() => {
    const segments = this.categoryBreakdown();
    if (!segments.length) {
      return 'conic-gradient(var(--color-secondary) 0 100%)';
    }

    let start = 0;
    const stops = segments.map((segment, index) => {
      const end = index === segments.length - 1 ? 100 : start + segment.percent;
      const stop = `${segment.color} ${start}% ${end}%`;
      start = end;
      return stop;
    });

    return `conic-gradient(${stops.join(', ')})`;
  });
  readonly trendDirection = computed(() => {
    const trend = this.cashFlowTrend();
    const previous = (trend.at(-2)?.income ?? 0) - (trend.at(-2)?.expense ?? 0);
    const current = (trend.at(-1)?.income ?? 0) - (trend.at(-1)?.expense ?? 0);
    if (!previous && !current) {
      return 'No trend yet';
    }

    return current >= previous ? 'Balance improving' : 'Balance tightening';
  });
  readonly budgetRows = computed<BudgetRow[]>(() =>
    this.budgets().map((budget) => {
      const category = this.categories().find((item) => item.id === budget.categoryId);
      const spent = this.filteredExpenses()
        .filter((expense) => expense.categoryId === budget.categoryId)
        .reduce((total, expense) => total + expense.amount, 0);
      const percent =
        budget.amount > 0 ? Math.min(Math.round((spent / budget.amount) * 100), 100) : 0;
      return {
        category: category ?? {
          id: budget.categoryId,
          userId: budget.userId,
          name: 'Uncategorized',
          hexColor: '#64748b',
        },
        spent,
        limit: budget.amount,
        currency: budget.currency,
        percent,
        tone: percent >= 100 ? 'bg-destructive' : percent >= 80 ? 'bg-amber-500' : 'bg-primary',
      };
    }),
  );
  readonly budgetAlerts = computed<BudgetAlert[]>(() =>
    this.budgetRows()
      .filter((row) => row.percent >= 80)
      .map((row) => ({
        ...row,
        message:
          row.percent >= 100
            ? `Limit reached: ${this.formatMoney(row.spent, row.currency)} spent.`
            : `${row.percent}% used with ${this.formatMoney(Math.max(row.limit - row.spent, 0), row.currency)} left.`,
      })),
  );

  //endregion

  async ngOnInit(): Promise<void> {
    this.setCurrentMonth();
    await this.loadDashboard();
  }

  //region Helpers
  openQuickAdd(mode: Exclude<QuickAddMode, null>): void {
    this.quickAddMode.set(mode);
    this.quickAddForm.set({
      ...this.emptyQuickAddForm(),
      categoryId: this.categories()[0]?.id ?? '',
    });
  }

  closeQuickAdd(): void {
    this.quickAddMode.set(null);
    this.quickAddForm.set(this.emptyQuickAddForm());
  }

  async saveQuickAdd(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    const mode = this.quickAddMode();
    const form = this.quickAddForm();
    if (!userId || !mode) {
      return;
    }

    if (!form.categoryId) {
      this.notifications.error(`Choose a category before saving the ${mode}.`);
      return;
    }

    try {
      if (mode === 'income') {
        await this.incomeService.create({
          title: form.title,
          amount: Number(form.amount),
          currency: form.currency.toUpperCase(),
          categoryId: form.categoryId,
          incomeDateUtc: new Date(form.date).toISOString(),
        });
      } else {
        await this.expenseService.create({
          title: form.title,
          amount: Number(form.amount),
          currency: form.currency.toUpperCase(),
          categoryId: form.categoryId,
          expenseDateUtc: new Date(form.date).toISOString(),
        });
      }

      await this.loadDashboard();
      this.notifications.success(`${mode === 'income' ? 'Income' : 'Expense'} saved.`);
      this.closeQuickAdd();
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, `Unable to save ${mode}.`));
    }
  }

  patchQuickAddForm(value: Partial<MoneyForm>): void {
    this.quickAddForm.update((form) => ({ ...form, ...value }));
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
      start: this.formatLocalDate(new Date(now.getFullYear(), now.getMonth(), 1)),
      end: this.formatLocalDate(new Date(now.getFullYear(), now.getMonth() + 1, 0)),
    });
  }

  private formatLocalDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  categoryName(categoryId: string): string {
    return (
      this.categories().find((category) => category.id === categoryId)?.name ?? 'Uncategorized'
    );
  }

  formatMoney(amount: number, currency = 'INR'): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency,
      maximumFractionDigits: 0,
    }).format(amount);
  }

  compactMoney(amount: number): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      notation: 'compact',
      maximumFractionDigits: 1,
    }).format(amount);
  }

  cashFlowPercent(value: number): number {
    return Math.min(
      Math.round((value / Math.max(this.totalIncome(), this.totalSpent(), 1)) * 100),
      100,
    );
  }

  stringValue(value: unknown): string {
    return typeof value === 'string' ? value : '';
  }

  numberValue(value: unknown): number {
    return Number(value);
  }

  private async loadDashboard(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId) {
      return;
    }

    try {
      const { categories, expenses } = await this.fetchDashboardData();
      this.categories.set(categories);
      this.expenses.set(expenses);
      this.budgets.set(await this.loadCurrentBudgets(categories));
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to load dashboard.'));
      return;
    }

    try {
      this.incomes.set(await this.incomeService.loadByUser());
    } catch (error: unknown) {
      this.incomes.set([]);
      this.notifications.info(getApiErrorMessage(error, 'Income data is unavailable right now.'));
    }
  }

  private async fetchDashboardData(): Promise<{
    categories: CategoryResponse[];
    expenses: ExpenseResponse[];
  }> {
    const categories = await this.categoryService.loadByUser();
    const expenses = await this.expenseService.loadByUser();

    return { categories, expenses };
  }

  private async loadCurrentBudgets(categories: CategoryResponse[]): Promise<BudgetResponse[]> {
    const now = new Date();
    try {
      return await this.budgetService.loadByCategories(
        categories.map((category) => category.id),
        now.getFullYear(),
        now.getMonth() + 1,
      );
    } catch {
      this.notifications.info('Budget data is unavailable right now.');
      return [];
    }
  }

  private currencyForCategory(categoryId: string): string {
    return (
      this.filteredExpenses().find((expense) => expense.categoryId === categoryId)?.currency ??
      'INR'
    );
  }

  private daysInRange(): number {
    const { start, end } = this.dateRange();
    if (!start || !end) {
      return 1;
    }

    const startDate = new Date(`${start}T00:00:00`);
    const endDate = new Date(`${end}T00:00:00`);
    return Math.max(Math.floor((endDate.getTime() - startDate.getTime()) / 86_400_000) + 1, 1);
  }

  private daysInCurrentMonth(): number {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
  }

  private filterIncomesByDateRange(incomes: readonly IncomeResponse[]): IncomeResponse[] {
    const { start, end } = this.dateRange();
    const startDate = start ? new Date(`${start}T00:00:00`) : null;
    const endDate = end ? new Date(`${end}T23:59:59`) : null;

    return incomes.filter((income) => {
      const date = new Date(income.incomeDateUtc);
      return (!startDate || date >= startDate) && (!endDate || date <= endDate);
    });
  }

  private emptyQuickAddForm(): MoneyForm {
    return {
      title: '',
      amount: 1,
      currency: 'INR',
      date: new Date().toISOString().slice(0, 10),
      categoryId: '',
    };
  }

  //endregion
}
