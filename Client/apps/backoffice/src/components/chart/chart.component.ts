import {
  Component,
  Input,
  OnChanges,
  OnInit,
  OnDestroy,
  SimpleChanges,
  ViewChild,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  inject,
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
import { ThemeService } from '../../services/theme.service';
import { Subscription } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './chart.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChartComponent implements OnInit, OnChanges, OnDestroy {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  @Input() type: ChartType = 'line';
  @Input() data!: ChartData;
  @Input() options?: ChartOptions;
  @Input() height?: string = '300px';
  @Input() width?: string = '100%';

  public chartOptions: ChartOptions = {};
  public chartData: ChartData = { labels: [], datasets: [] };
  public chartType: ChartType = 'line';

  private chartService = inject(ChartService);
  private themeService = inject(ThemeService);
  private cdr = inject(ChangeDetectorRef);
  private themeSubscription?: Subscription;

  ngOnInit() {
    this.initializeChart();

    this.themeSubscription = toObservable(this.themeService.currentTheme).subscribe(() => {
      this.initializeChart();
    });
  }

  ngOnDestroy() {
    if (this.themeSubscription) {
      this.themeSubscription.unsubscribe();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['type'] || changes['data'] || changes['options']) {
      this.initializeChart();
    }
  }

  private initializeChart() {
    this.chartType = this.type;
    this.chartData = this.data || { labels: [], datasets: [] };

    this.chartOptions = this.chartService.mergeOptions(
      this.options || {},
      this.type
    );

    this.cdr.markForCheck();

    setTimeout(() => {
      if (this.chart) {
        this.chart.update();
      }
    }, 0);
  }

  updateChart(data: ChartData) {
    this.chartData = data;
    if (this.chart) {
      this.chart.update();
    }
  }

  updateOptions(options: ChartOptions) {
    this.chartOptions = this.chartService.mergeOptions(
      options,
      this.chartType
    );
    if (this.chart) {
      this.chart.update();
    }
  }

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
