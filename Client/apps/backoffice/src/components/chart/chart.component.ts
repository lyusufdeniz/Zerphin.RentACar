import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import {
  ChartConfiguration,
  ChartData,
  ChartType,
  ChartOptions,
} from 'chart.js';
import { ChartService } from '../../services/chart.service';

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './chart.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChartComponent implements OnInit, OnChanges {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  @Input() type: ChartType = 'line';
  @Input() data!: ChartData;
  @Input() options?: ChartOptions;
  @Input() height?: string = '300px';
  @Input() width?: string = '100%';

  public chartOptions: ChartOptions = {};
  public chartData: ChartData = { labels: [], datasets: [] };
  public chartType: ChartType = 'line';

  constructor(
    private chartService: ChartService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.initializeChart();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['type'] || changes['data'] || changes['options']) {
      this.initializeChart();
    }
  }

  private initializeChart() {
    this.chartType = this.type;
    this.chartData = this.data || { labels: [], datasets: [] };

    // Merge default options with custom options
    this.chartOptions = this.chartService.mergeOptions(
      this.options || {},
      this.type
    );

    this.cdr.markForCheck();

    // Update chart if it exists
    setTimeout(() => {
      if (this.chart) {
        this.chart.update();
      }
    }, 0);
  }

  /**
   * Update chart data
   */
  updateChart(data: ChartData) {
    this.chartData = data;
    if (this.chart) {
      this.chart.update();
    }
  }

  /**
   * Update chart options
   */
  updateOptions(options: ChartOptions) {
    this.chartOptions = this.chartService.mergeOptions(
      options,
      this.chartType
    );
    if (this.chart) {
      this.chart.update();
    }
  }

  /**
   * Get chart configuration
   */
  getChartConfiguration(): any {
    switch (this.type) {
      case 'bar':
        return this.chartService.createBarChartConfig(
          this.chartData as ChartData<'bar'>,
          this.chartOptions as Partial<ChartOptions<'bar'>>
        );
      case 'pie':
        return this.chartService.createPieChartConfig(
          this.chartData as ChartData<'pie'>,
          this.chartOptions as Partial<ChartOptions<'pie'>>
        );
      case 'doughnut':
        return this.chartService.createDoughnutChartConfig(
          this.chartData as ChartData<'doughnut'>,
          this.chartOptions as Partial<ChartOptions<'doughnut'>>
        );
      case 'radar':
        return this.chartService.createRadarChartConfig(
          this.chartData as ChartData<'radar'>,
          this.chartOptions as Partial<ChartOptions<'radar'>>
        );
      case 'polarArea':
        return this.chartService.createPolarAreaChartConfig(
          this.chartData as ChartData<'polarArea'>,
          this.chartOptions as Partial<ChartOptions<'polarArea'>>
        );
      default:
        return this.chartService.createLineChartConfig(
          this.chartData as ChartData<'line'>,
          this.chartOptions as Partial<ChartOptions<'line'>>
        );
    }
  }
}

