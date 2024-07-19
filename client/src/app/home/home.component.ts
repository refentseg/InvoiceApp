import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { BarChartComponent } from '../components/charts/bar-chart/bar-chart.component';
import { DoughnutChartComponent } from '../components/charts/doughnut-chart/doughnut-chart.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule,BarChartComponent,DoughnutChartComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
