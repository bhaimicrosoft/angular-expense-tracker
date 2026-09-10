import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toApiList } from '@src/app/core/utils/api-response';
import {
  CreateIncomeRequest,
  IncomeResponse,
  UpdateIncomeRequest,
} from '@src/app/core/models/models';
import { firstValueFrom, map } from 'rxjs';
import { environment } from '@src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class IncomeService {
  private readonly http = inject(HttpClient);

  async loadByUser(): Promise<IncomeResponse[]> {
    const response = await firstValueFrom(
      this.http.get<IncomeResponse[] | { items?: IncomeResponse[]; Items?: IncomeResponse[] }>(
        `${environment.serverUrl}/me/incomes`,
        {
          params: { Page: 1, PageSize: 100 },
        },
      ),
    );
    const incomes = toApiList(response);
    return this.sortIncomes(incomes);
  }

  async create(request: CreateIncomeRequest): Promise<{ id: string }> {
    return firstValueFrom(
      this.http
        .post<{ id: string; Id?: string }>(`${environment.serverUrl}/me/incomes`, request)
        .pipe(map((response) => ({ id: response.id ?? response.Id ?? '' }))),
    );
  }

  async update(request: UpdateIncomeRequest): Promise<void> {
    const { id, ...body } = request;
    await firstValueFrom(this.http.put<void>(`${environment.serverUrl}/me/incomes/${id}`, body));
  }

  async delete(incomeId: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${environment.serverUrl}/me/incomes/${incomeId}`));
  }

  private sortIncomes(incomes: IncomeResponse[]): IncomeResponse[] {
    return [...incomes].sort(
      (a, b) => new Date(b.incomeDateUtc).getTime() - new Date(a.incomeDateUtc).getTime(),
    );
  }
}
