// Authentication & User DTOs
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token?: string;
  Token?: string;
}

export interface RegisterResponse {
  userId?: string;
  UserId?: string;
}

export interface UserResponse {
  id: string;
  email: string;
  fullName: string;
}

// Category DTOs
export interface CategoryResponse {
  id: string;
  userId: string;
  name: string;
  hexColor: string;
}

export interface CreateCategoryRequest {
  userId: string;
  name: string;
  hexColor: string;
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {
  id: string;
}

// Expense DTOs
export interface ExpenseResponse {
  id: string;
  userId: string;
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  expenseDateUtc: string;
}

export interface CreateExpenseRequest {
  userId: string;
  title: string;
  amount: number;
  currency: string;
  categoryId: string;
  expenseDateUtc: string;
}

export interface UpdateExpenseRequest extends CreateExpenseRequest {
  id: string;
}

// Budget DTOs
export interface BudgetResponse {
  id: string;
  userId: string;
  categoryId: string;
  amount: number;
  currency: string;
  month: number;
  year: number;
}

export interface CreateBudgetRequest {
  userId: string;
  categoryId: string;
  amount: number;
  currency: string;
  month: number;
  year: number;
}

export interface UpdateBudgetRequest extends CreateBudgetRequest {
  id: string;
}
