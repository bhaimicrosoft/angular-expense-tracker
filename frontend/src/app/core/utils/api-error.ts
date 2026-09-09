import { HttpErrorResponse } from '@angular/common/http';

export function getApiErrorMessage(
  error: unknown,
  fallback = 'Something went wrong. Please try again.',
): string {
  if (error instanceof HttpErrorResponse) {
    const apiError = error.error;
    if (typeof apiError === 'string' && apiError.trim()) {
      return apiError;
    }

    if (apiError && typeof apiError === 'object') {
      const details = apiError as Record<string, unknown>;
      const errors = details['errors'];
      const validationMessage = readValidationMessage(errors);
      if (validationMessage) {
        return validationMessage;
      }

      const message = details['message'] ?? details['detail'] ?? details['title'];
      if (typeof message === 'string' && message.trim()) {
        return message;
      }
    }
  }

  return fallback;
}

function readValidationMessage(errors: unknown): string | null {
  if (!errors || typeof errors !== 'object' || Array.isArray(errors)) {
    return null;
  }

  for (const value of Object.values(errors as Record<string, unknown>)) {
    if (typeof value === 'string' && value.trim()) {
      return value;
    }

    if (Array.isArray(value)) {
      const message = value.find(
        (item): item is string => typeof item === 'string' && !!item.trim(),
      );
      if (message) {
        return message;
      }
    }
  }

  return null;
}
