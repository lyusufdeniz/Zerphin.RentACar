import { Injectable } from '@angular/core';
import {
  ChartConfiguration,
  ChartData,
  ChartType,
  ChartOptions,
} from 'chart.js';

@Injectable({
  providedIn: 'root',
})
export class ChartService {
  /**
   * Get default chart options with dark theme
   */
  getDefaultOptions(): ChartOptions {
    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          labels: {
            color: '#cccccc',
            font: {
              size: 12,
            },
          },
        },
        tooltip: {
          backgroundColor: '#1a1a1a',
          titleColor: '#ffffff',
          bodyColor: '#cccccc',
          borderColor: '#252525',
          borderWidth: 1,
        },
      },
      scales: {
        x: {
          ticks: {
            color: '#999999',
          },
          grid: {
            display: false, // Remove grid lines
            drawOnChartArea: false,
          },
          border: {
            display: false,
          },
        },
        y: {
          ticks: {
            color: '#999999',
          },
          grid: {
            display: false, // Remove grid lines
            drawOnChartArea: false,
          },
          border: {
            display: false,
          },
        },
      },
    };
  }

  /**
   * Get dark theme colors - Modern and vibrant palette
   */
  getDarkThemeColors(): string[] {
    return [
      '#00d084', // Primary green
      '#6366f1', // Indigo
      '#8b5cf6', // Purple
      '#ec4899', // Pink
      '#f59e0b', // Amber
      '#10b981', // Emerald
      '#3b82f6', // Blue
      '#f97316', // Orange
    ];
  }

  /**
   * Create line chart configuration
   */
  createLineChartConfig(
    data: ChartData<'line'>,
    options?: Partial<ChartOptions<'line'>>
  ): ChartConfiguration<'line'> {
    return {
      type: 'line',
      data,
      options: {
        ...this.getDefaultOptions(),
        ...(options as any),
        elements: {
          line: {
            tension: 0.4,
          },
        },
      } as ChartOptions<'line'>,
    };
  }

  /**
   * Create bar chart configuration
   */
  createBarChartConfig(
    data: ChartData<'bar'>,
    options?: Partial<ChartOptions<'bar'>>
  ): ChartConfiguration<'bar'> {
    return {
      type: 'bar',
      data,
      options: {
        ...this.getDefaultOptions(),
        ...(options as any),
      } as ChartOptions<'bar'>,
    };
  }

  /**
   * Create pie chart configuration
   */
  createPieChartConfig(
    data: ChartData<'pie'>,
    options?: Partial<ChartOptions<'pie'>>
  ): ChartConfiguration<'pie'> {
    return {
      type: 'pie',
      data,
      options: {
        ...this.getDefaultOptions(),
        ...(options as any),
      } as ChartOptions<'pie'>,
    };
  }

  /**
   * Create doughnut chart configuration
   */
  createDoughnutChartConfig(
    data: ChartData<'doughnut'>,
    options?: Partial<ChartOptions<'doughnut'>>
  ): ChartConfiguration<'doughnut'> {
    return {
      type: 'doughnut',
      data,
      options: {
        ...this.getDefaultOptions(),
        ...(options as any),
      } as ChartOptions<'doughnut'>,
    };
  }

  /**
   * Create radar chart configuration
   */
  createRadarChartConfig(
    data: ChartData<'radar'>,
    options?: Partial<ChartOptions<'radar'>>
  ): ChartConfiguration<'radar'> {
    return {
      type: 'radar',
      data,
      options: {
        ...this.getDefaultOptions(),
        ...(options as any),
      } as ChartOptions<'radar'>,
    };
  }

  /**
   * Create polar area chart configuration
   */
  createPolarAreaChartConfig(
    data: ChartData<'polarArea'>,
    options?: Partial<ChartOptions<'polarArea'>>
  ): ChartConfiguration<'polarArea'> {
    return {
      type: 'polarArea',
      data,
      options: {
        ...this.getDefaultOptions(),
        ...(options as any),
      } as ChartOptions<'polarArea'>,
    };
  }

  /**
   * Merge custom options with default options
   */
  mergeOptions<T extends ChartType = ChartType>(
    customOptions: Partial<ChartOptions<T>>,
    chartType?: T
  ): ChartOptions<T> {
    return {
      ...this.getDefaultOptions(),
      ...(customOptions as any),
    } as ChartOptions<T>;
  }
}

