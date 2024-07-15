import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ControlValueAccessor, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZodType } from 'zod';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './input.component.html',
  styleUrl: './input.component.css',
})
export class InputComponent{
  @Input() label: string = '';
  @Input() type: string = 'text';
  @Input() control!: FormControl;
}
