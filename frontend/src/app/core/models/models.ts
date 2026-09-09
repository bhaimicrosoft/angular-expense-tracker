export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  fullName: string;
  email: string;
  password: string;
}

export type AuthResponse = {
  token: string;
  Token?: string;
};

export type UserResponse = {
  id: string;
  email: string;
  fullName: string;
};

export type UpdateCurrentUserRequest = {
  email: string;
  fullName: string;
};

export type RegisterResponse = UserResponse;

// Category DTOs
export type CategoryResponse = {
  id: string;
  userId: string;
  name: string;
  hexColor: string;
};

export type CreateCategoryRequest = {
  name: string;
  hexColor: string;
};

export type UpdateCategoryRequest = CreateCategoryRequest & {
  id: string;
};

// Expense DTOs
export type ExpenseResponse = {
  id: string;
  userId: string;
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  expenseDateUtc: string;
};

export type CreateExpenseRequest = {
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  expenseDateUtc: string;
};

export type UpdateExpenseRequest = CreateExpenseRequest & {
  id: string;
};

// Income DTOs
export type IncomeResponse = {
  id: string;
  userId: string;
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  incomeDateUtc: string;
};

export type CreateIncomeRequest = {
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  incomeDateUtc: string;
};

export type UpdateIncomeRequest = CreateIncomeRequest & {
  id: string;
};

// Budget DTOs
export type BudgetResponse = {
  id: string;
  userId: string;
  categoryId: string;
  amount: number;
  currency: string;
  month: number;
  year: number;
};

export type CreateBudgetRequest = {
  categoryId: string;
  amount: number;
  currency: string;
  month: number;
  year: number;
};

export type UpdateBudgetRequest = CreateBudgetRequest & {
  id: string;
};
