export interface DateRange {
  start: string;
  end: string;
}

export interface HasExpenseDate {
  expenseDateUtc: string;
}

export function filterByDateRange<T extends HasExpenseDate>(
  expenses: readonly T[],
  range: DateRange,
): T[] {
  const start = range.start ? new Date(`${range.start}T00:00:00`) : null;
  const end = range.end ? new Date(`${range.end}T23:59:59`) : null;

  return expenses.filter((expense) => {
    const date = new Date(expense.expenseDateUtc);
    return (!start || date >= start) && (!end || date <= end);
  });
}

export function formatDateValue(value: string): string {
  if (!value) {
    return 'Select date';
  }

  const [year, month, day] = value.slice(0, 10).split('-');
  return day && month && year ? `${day}-${month}-${year}` : value;
}
