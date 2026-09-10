import { inject, Injectable } from '@angular/core';
import { toApiList } from '@src/app/core/utils/api-response';
import { HttpClient } from '@angular/common/http';
import {
  CreateExpenseRequest,
  ExpenseResponse,
  UpdateExpenseRequest,
} from '@src/app/core/models/models';
import { firstValueFrom, map } from 'rxjs';
import { environment } from '@src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class ExpenseService {
  private readonly http = inject(HttpClient);

  async loadByUser(): Promise<ExpenseResponse[]> {
    const response = await firstValueFrom(
      this.http.get<ExpenseResponse[] | { items?: ExpenseResponse[]; Items?: ExpenseResponse[] }>(
        `${environment.serverUrl}/me/expenses`,
        {
          params: { Page: 1, PageSize: 100 },
        },
      ),
    );
    const expenses = toApiList(response);
    return this.sortExpenses(expenses);
  }

  async create(request: CreateExpenseRequest): Promise<{ id: string }> {
    return await firstValueFrom(
      this.http
        .post<{ id: string; Id?: string }>(`${environment.serverUrl}/me/expenses`, request)
        .pipe(map((response) => ({ id: response.id ?? response.Id ?? '' }))),
    );
  }

  async update(request: UpdateExpenseRequest): Promise<void> {
    const { id, ...body } = request;
    await firstValueFrom(this.http.put<void>(`${environment.serverUrl}/me/expenses/${id}`, body));
  }

  async delete(expenseId: string): Promise<void> {
    await firstValueFrom(
      this.http.delete<void>(`${environment.serverUrl}/me/expenses/${expenseId}`),
    );
  }

  private sortExpenses(expenses: ExpenseResponse[]): ExpenseResponse[] {
    return [...expenses].sort(
      (a, b) => new Date(b.expenseDateUtc).getTime() - new Date(a.expenseDateUtc).getTime(),
    );
  }
}
