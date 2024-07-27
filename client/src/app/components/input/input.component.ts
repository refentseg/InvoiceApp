import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { z, ZodType } from 'zod';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './input.component.html',
  styleUrl: './input.component.css',
})
export class InputComponent{
  @Input() label: string = '';
  @Input() type: string = '';
  @Input() control!: FormControl;
  @Input() zodSchema!: z.ZodType<any>;
  
  ngOnInit() {
    if (this.zodSchema) {
      this.control.addValidators(this.zodValidator());
    }
  }

  zodValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      try {
        this.zodSchema.parse(control.value);
        return null;
      } catch (error) {
        if (error instanceof z.ZodError) {
          return { zodError: error.errors[0].message };
        }
        return { zodError: 'Invalid input' };
      }
    };
  }
}
