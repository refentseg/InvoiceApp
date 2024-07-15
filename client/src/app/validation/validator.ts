import { AbstractControl, ValidatorFn } from '@angular/forms';
import { ZodIssue, ZodSchema } from 'zod';

export function zodValidator(schema: ZodSchema): ValidatorFn {
  return (control: AbstractControl) => {
    const result = schema.safeParse(control.value);
    if (!result.success) {
      const errors: Record<string, string> = {};
      result.error.issues.forEach((issue: ZodIssue) => {
        errors[issue.code] = issue.message;
      });
      return errors;
    }
    return null;
  };
}
