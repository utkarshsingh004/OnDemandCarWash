import { HttpErrorResponse } from '@angular/common/http';

export function apiErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    if (typeof error.error === 'string' && error.error.trim()) {
      return error.error;
    }

    if (error.error && typeof error.error === 'object') {
      const body = error.error as { detail?: unknown; message?: unknown; title?: unknown };
      const message = body.detail ?? body.message ?? body.title;
      if (typeof message === 'string' && message.trim()) {
        return message;
      }
    }
  }

  return fallback;
}
