import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BudgetResponse,
  CreateBudgetRequest,
  UpdateBudgetRequest,
} from '@src/app/core/models/models';
import { environment } from '@src/environments/environment.development';
import { firstValueFrom, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BudgetService {
  private readonly http: HttpClient = inject(HttpClient);

  async loadByCategories(
    categoryIds: string[],
    year: number,
    month: number,
  ): Promise<BudgetResponse[]> {
    return (
      await Promise.all(categoryIds.map((catId) => this.loadByCategory(catId, year, month)))
    ).filter((budget): budget is BudgetResponse => budget !== null);
  }

  async create(request: CreateBudgetRequest): Promise<{ id: string }> {
    return await firstValueFrom(
      this.http
        .post<{ id: string; Id?: string }>(`${environment.serverUrl}/me/budgets`, request)
        .pipe(map((response) => ({ id: response.id ?? response.Id ?? '' }))),
    );
  }

  async update(request: UpdateBudgetRequest): Promise<void> {
    const { id, ...body } = request;
    await firstValueFrom(this.http.put<void>(`${environment.serverUrl}/me/budgets/${id}`, body));
  }

  async delete(budgetId: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${environment.serverUrl}/me/budgets/${budgetId}`));
  }

  private isNotFound(error: unknown): boolean {
    return typeof error === 'object' && error !== null && 'status' in error && error.status === 404;
  }

  private async loadByCategory(
    categoryId: string,
    year: number,
    month: number,
  ): Promise<BudgetResponse | null> {
    try {
      return await firstValueFrom(
        this.http.get<BudgetResponse>(
          `${environment.serverUrl}/me/budgets/${categoryId}/${year}/${month}`,
        ),
      );
    } catch (error: unknown) {
      if (this.isNotFound(error)) {
        return null;
      }

      throw error;
    }
  }
}
