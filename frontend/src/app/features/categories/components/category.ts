import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '@src/app/features/users/services/auth.service';
import { CategoryService } from '@src/app/features/categories/services/category.service';
import { NotificationService } from '@src/app/core/services/notification.service';
import { FormsModule } from '@angular/forms';
import { CategoryResponse } from '@src/app/core/models/models';
import { getApiErrorMessage } from '@src/app/core/utils/api-error';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmButtonImports } from '@spartan-ng/helm/button';

interface CategoryForm {
  name: string;
  hexColor: string;
}

@Component({
  selector: 'app-category',
  imports: [FormsModule, HlmInputImports, HlmButtonImports],
  templateUrl: './category.html',
})
export class Category implements OnInit {
  // Service Injection
  private readonly authService: AuthService = inject(AuthService);
  private readonly categoryService: CategoryService = inject(CategoryService);
  private readonly notifications: NotificationService = inject(NotificationService);

  // Variables
  readonly categories = signal<CategoryResponse[]>([]);
  readonly editingCategoryId = signal<string | null>(null);
  readonly form = signal<CategoryForm>(this.emptyForm());

  //OnInit Lifecycle Hook
  async ngOnInit(): Promise<void> {
    await this.loadCategories();
  }

  async loadCategories(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId) {
      return;
    }

    try {
      this.categories.set(await this.fetchCategories());
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to load categories'));
    }
  }

  // saving category
  async saveCategory(): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId) {
      return;
    }

    const currentForm = this.form();
    const payload = { name: currentForm.name, hexColor: currentForm.hexColor } as CategoryForm;
    const editingId = this.editingCategoryId();

    try {
      if (editingId) {
        await this.categoryService.update({ id: editingId, ...payload });
      } else {
        await this.categoryService.create(payload);
      }
      this.categories.set(await this.fetchCategories());
      this.notifications.success(editingId ? 'Category updated' : 'Category created!');
      this.resetForm();
    } catch (error: unknown) {
      this.notifications.error(getApiErrorMessage(error, 'Unable to save category'));
    }
  }

  editCategory(category: CategoryResponse): void {
    this.editingCategoryId.set(category.id);
    this.form.set({ name: category.name, hexColor: category.hexColor });
  }

  async deleteCategory(category: CategoryResponse): Promise<void> {
    const userId = this.authService.currentUser()?.id;
    if (!userId || !confirm(`Delete "${category.name}"?`)) {
      return;
    }

    try {
      await this.categoryService.delete(category.id);
      this.categories.set(await this.fetchCategories());
      this.notifications.success('Category deleted');
    } catch (err: unknown) {
      this.notifications.error(getApiErrorMessage(err, 'Unable to delete category'));
    }
  }

  patchForm(value: Partial<CategoryForm>): void {
    this.form.update((form) => ({ ...form, ...value }));
  }

  resetForm(): void {
    this.editingCategoryId.set(null);
    this.form.set(this.emptyForm());
  }

  private fetchCategories(): Promise<CategoryResponse[]> {
    return this.categoryService.loadByUser();
  }

  private emptyForm(): CategoryForm {
    return {
      name: '',
      hexColor: '#ffffff',
    };
  }
}
