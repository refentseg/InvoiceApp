import { NgClass, NgStyle } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-nav-item',
  standalone: true,
  imports: [NgClass, RouterLink,NgStyle,LucideAngularModule],
  templateUrl: './nav-item.component.html',
  styleUrl: './nav-item.component.css'
})
export class NavItemComponent {
  @Input() icon: string = '';
  @Input() path: string = '';
  @Input() title: string = '';
  @Input() active!: boolean;
  @Input() expanded: boolean =true;
}
