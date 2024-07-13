import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, NgControl } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [],
  templateUrl: './input.component.html',
  styleUrl: './input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true
    }
  ]
})
export class InputComponent implements ControlValueAccessor, OnInit{
  @Input() label: string = '';
  @Input() type: string = '';
  @Input() pattern: string = '';

  control: FormControl;
  showError: boolean = false;
  errorMessage: string = '';

  constructor(private ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.control = new FormControl();
  }

  ngOnInit() {
    this.control = this.ngControl.control as FormControl;
  }

  writeValue(value: any): void {
    if (this.control) {
      this.control.setValue(value, { emitEvent: false });
    }
  }

  registerOnChange(fn: any): void {
    this.control.valueChanges.subscribe(fn);
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.control.disable() : this.control.enable();
  }

  onTouched = () => {};

  ngDoCheck() {
    if (this.control.touched && this.control.invalid) {
      this.showError = true;
      this.errorMessage = this.getErrorMessage();
    } else {
      this.showError = false;
    }
  }

  private getErrorMessage(): string {
    if (this.control.hasError('required')) {
      return `${this.label} is required`;
    }
    if (this.control.hasError('pattern')) {
      return `Invalid ${this.label} format`;
    }
    return 'Invalid input';
  }
}
