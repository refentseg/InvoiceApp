import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  @Input() mode!: string;
  @Output() toggleMode = new EventEmitter<string>();

  onToggleMode() {
    this.toggleMode.emit('login');
  }
}
