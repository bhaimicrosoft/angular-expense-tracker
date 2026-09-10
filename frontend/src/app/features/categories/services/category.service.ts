import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  CategoryResponse,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from '@src/app/core/models/models';
import { firstValueFrom, map } from 'rxjs';
import { environment } from '@src/environments/environment.development';
import { toApiList } from '@src/app/core/utils/api-response';
import { AuthService } from '@src/app/features/users/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly auth = inject(AuthService)

  async loadByUser(): Promise<CategoryResponse[]> {
    const response = await firstValueFrom(
      this.http.get<
        CategoryResponse[] | { items?: CategoryResponse[]; Items?: CategoryResponse[] }
      >(`${environment.serverUrl}/me/categories`),
    );

    return toApiList(response);
  }

  async create(request: CreateCategoryRequest): Promise<{ id: string }> {
    return firstValueFrom(
      this.http
        .post<{ id: string; Id?: string }>(`${environment.serverUrl}/me/categories`, request)
        .pipe(map((response) => ({ id: response.id ?? response.Id ?? '' }))),
    );
  }

  async update(request: UpdateCategoryRequest): Promise<void> {
    const { id, ...body } = request;
    await firstValueFrom(this.http.put<void>(`${environment.serverUrl}/me/categories/${id}`, body));
  }

  async delete(categoryId: string): Promise<void> {
    await firstValueFrom(
      this.http.delete<void>(`${environment.serverUrl}/me/categories/${categoryId}`),
    );
  }
}
