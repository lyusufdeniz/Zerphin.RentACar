import { Injectable, inject } from '@angular/core';
import {
  ChartConfiguration,
  ChartData,
  ChartType,
  ChartOptions,
} from 'chart.js';
import { ThemeService } from './theme.service';

@Injectable({
  providedIn: 'root',
})
export class ChartService {
  private themeService = inject(ThemeService);

  private getCSSVariable(variable: string): string {
    return getComputedStyle(document.documentElement)
      .getPropertyValue(variable)
      .trim();
  }

  getDefaultOptions(): ChartOptions {
    const isDark = this.themeService.isDarkTheme();

    const textPrimary = this.getCSSVariable('--text-primary') || (isDark ? '#ffffff' : '#1a1a1a');
    const textSecondary = this.getCSSVariable('--text-secondary') || (isDark ? '#cccccc' : '#666666');
    const textTertiary = this.getCSSVariable('--text-tertiary') || (isDark ? '#999999' : '#999999');
    const bgSecondary = this.getCSSVariable('--bg-secondary') || (isDark ? '#1a1a1a' : '#f5f5f5');
    const bgTertiary = this.getCSSVariable('--bg-tertiary') || (isDark ? '#141414' : '#ffffff');
    const borderColor = this.getCSSVariable('--border-color') || (isDark ? '#252525' : '#e0e0e0');

    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          labels: {
            color: textSecondary,
            font: {
              size: 12,
            },
          },
        },
        tooltip: {
          backgroundColor: bgSecondary,
          titleColor: textPrimary,
          bodyColor: textSecondary,
          borderColor: borderColor,
          borderWidth: 1,
        },
      },
      scales: {
        x: {
          ticks: {
            display: false, 
          },
          grid: {
            display: false, 
            drawOnChartArea: false,
          },
          border: {
            display: false,
          },
        },
        y: {
          ticks: {
            display: false, 
          },
          grid: {
            display: false, 
            drawOnChartArea: false,
          },
          border: {
            display: false,
          },
        },
        y1: {
          type: 'linear',
          position: 'right',
          ticks: {
            display: false, 
          },
          grid: {
            display: false, 
            drawOnChartArea: false,
          },
          border: {
            display: false,
          },
        },
      },
    };
  }

  getDarkThemeColors(): string[] {
    return [
      '#00d084', 
      '#6366f1', 
      '#8b5cf6', 
      '#ec4899', 
      '#f59e0b', 
      '#10b981', 
      '#3b82f6', 
      '#f97316', 
    ];
  }

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
