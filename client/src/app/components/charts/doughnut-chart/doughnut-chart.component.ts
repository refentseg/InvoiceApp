import { Component, OnInit } from '@angular/core';
import { ChartData, ChartDataset, ChartOptions, ChartType } from 'chart.js';
import { ChartService } from '../../../services/chart.service';
import { BaseChartDirective } from 'ng2-charts';
import { statusColors } from '../../../util/util';

@Component({
  selector: 'app-doughnut-chart',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './doughnut-chart.component.html',
  styleUrl: './doughnut-chart.component.css'
})
export class DoughnutChartComponent implements OnInit{
  public doughnutChartOptions: ChartOptions = {
    responsive: true,
  };

  constructor(private chartsService: ChartService) {}

  public doughnutChartLabels: string[] = [];
  public doughnutChartType: ChartType = 'doughnut';
  public doughnutChartData: ChartDataset[] = [
    { data: [], label: 'Total Status',backgroundColor: []  }
  ];

  ngOnInit(): void {
    this.chartsService.getSumByStatus().subscribe(data => {
      this.doughnutChartLabels = Object.keys(data);
      this.doughnutChartData[0].data = Object.values(data);
      this.doughnutChartData[0].backgroundColor = this.doughnutChartLabels.map(label => statusColors[label]);
    });
  }
}
