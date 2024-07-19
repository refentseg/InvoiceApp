import { Component, OnInit } from '@angular/core';
import { ChartDataset,ChartOptions } from 'chart.js';
import { ChartService } from '../../../services/chart.service';
import { BaseChartDirective } from 'ng2-charts';
import { currencyFormat } from '../../../util/util';
import { callback } from 'chart.js/dist/helpers/helpers.core';

@Component({
  selector: 'app-bar-chart',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './bar-chart.component.html',
  styleUrl: './bar-chart.component.css'
})
export class BarChartComponent implements OnInit{
  public barChartOptions: ChartOptions= {
    responsive: true,
  };

  public barChartLabels: string[] = [];
  public barChartType = 'bar';
  public barChartLegend = true;
  public barChartPlugins = [];

  public barChartData: ChartDataset[] = [
    { data: [], label: 'Total Per Month' }
  ];

  constructor(private chartsService: ChartService) {}

  ngOnInit(): void {
    this.chartsService.getTotalSumPerMonth().subscribe(data => {
      this.barChartLabels = Object.keys(data).map(key => {
        const [month, year] = key.split('/');
        return new Date(Number(year), Number(month) - 1).toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
      });
      this.barChartData[0].data = Object.values(data);
    });
  }



}
