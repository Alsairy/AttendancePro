import { lazy, ComponentType } from 'react';
const debounce = <T extends (...args: any[]) => any>(
  func: T,
  delay: number,
  options: { leading?: boolean; trailing?: boolean } = {}
): T => {
  let timeoutId: NodeJS.Timeout | null = null;
  let lastCallTime = 0;
  const { leading = false, trailing = true } = options;

  return ((...args: any[]) => {
    const now = Date.now();
    const timeSinceLastCall = now - lastCallTime;

    const execute = () => {
      lastCallTime = now;
      return func.apply(null, args);
    };

    if (timeoutId) {
      clearTimeout(timeoutId);
    }

    if (leading && timeSinceLastCall >= delay) {
      return execute();
    }

    if (trailing) {
      timeoutId = setTimeout(execute, delay);
    }
  }) as T;
};

const throttle = <T extends (...args: any[]) => any>(
  func: T,
  delay: number,
  options: { leading?: boolean; trailing?: boolean } = {}
): T => {
  let lastCallTime = 0;
  let timeoutId: NodeJS.Timeout | null = null;
  const { leading = true, trailing = false } = options;

  return ((...args: any[]) => {
    const now = Date.now();
    const timeSinceLastCall = now - lastCallTime;

    const execute = () => {
      lastCallTime = now;
      return func.apply(null, args);
    };

    if (leading && timeSinceLastCall >= delay) {
      return execute();
    }

    if (trailing && !timeoutId) {
      timeoutId = setTimeout(() => {
        timeoutId = null;
        execute();
      }, delay - timeSinceLastCall);
    }
  }) as T;
};

export interface PerformanceMetrics {
  loadTime: number;
  renderTime: number;
  memoryUsage: number;
  bundleSize: number;
  cacheHitRate: number;
}

export class PerformanceMonitor {
  private static instance: PerformanceMonitor;
  private metrics: PerformanceMetrics[] = [];
  private observer: PerformanceObserver | null = null;

  private constructor() {
    this.initializePerformanceObserver();
  }

  public static getInstance(): PerformanceMonitor {
    if (!PerformanceMonitor.instance) {
      PerformanceMonitor.instance = new PerformanceMonitor();
    }
    return PerformanceMonitor.instance;
  }

  private initializePerformanceObserver(): void {
    if ('PerformanceObserver' in window) {
      this.observer = new PerformanceObserver((list) => {
        const entries = list.getEntries();
        entries.forEach((entry) => {
          this.recordMetric(entry);
        });
      });

      this.observer.observe({ 
        entryTypes: ['navigation', 'resource', 'measure', 'paint'] 
      });
    }
  }

  private recordMetric(entry: PerformanceEntry): void {
    const metric: Partial<PerformanceMetrics> = {
      loadTime: entry.duration,
      renderTime: performance.now()
    };

    if ('memory' in performance) {
      const memInfo = (performance as any).memory;
      metric.memoryUsage = memInfo.usedJSHeapSize;
    }

    this.metrics.push(metric as PerformanceMetrics);
    
    if (this.metrics.length > 100) {
      this.metrics = this.metrics.slice(-50);
    }
  }

  public getMetrics(): PerformanceMetrics[] {
    return [...this.metrics];
  }

  public getAverageMetrics(): PerformanceMetrics {
    if (this.metrics.length === 0) {
      return {
        loadTime: 0,
        renderTime: 0,
        memoryUsage: 0,
        bundleSize: 0,
        cacheHitRate: 0
      };
    }

    const totals = this.metrics.reduce((acc, metric) => ({
      loadTime: acc.loadTime + metric.loadTime,
      renderTime: acc.renderTime + metric.renderTime,
      memoryUsage: acc.memoryUsage + metric.memoryUsage,
      bundleSize: acc.bundleSize + metric.bundleSize,
      cacheHitRate: acc.cacheHitRate + metric.cacheHitRate
    }), {
      loadTime: 0,
      renderTime: 0,
      memoryUsage: 0,
      bundleSize: 0,
      cacheHitRate: 0
    });

    const count = this.metrics.length;
    return {
      loadTime: totals.loadTime / count,
      renderTime: totals.renderTime / count,
      memoryUsage: totals.memoryUsage / count,
      bundleSize: totals.bundleSize / count,
      cacheHitRate: totals.cacheHitRate / count
    };
  }

  public markStart(name: string): void {
    performance.mark(`${name}-start`);
  }

  public markEnd(name: string): void {
    performance.mark(`${name}-end`);
    performance.measure(name, `${name}-start`, `${name}-end`);
  }

  public clearMetrics(): void {
    this.metrics = [];
    performance.clearMarks();
    performance.clearMeasures();
  }
}

export const lazyLoadComponent = <T extends ComponentType<any>>(
  importFunc: () => Promise<{ default: T }>
) => {
  return lazy(() => {
    const startTime = performance.now();
    
    return importFunc().then((module) => {
      const loadTime = performance.now() - startTime;
      console.log(`Component loaded in ${loadTime.toFixed(2)}ms`);
      
      const monitor = PerformanceMonitor.getInstance();
      (monitor as any).recordMetric({
        name: 'component-load',
        duration: loadTime,
        startTime
      } as PerformanceEntry);
      
      return module;
    });
  });
};

export const optimizedDebounce = <T extends (...args: any[]) => any>(
  func: T,
  delay: number = 300
): T => {
  return debounce(func, delay, { leading: false, trailing: true }) as T;
};

export const optimizedThrottle = <T extends (...args: any[]) => any>(
  func: T,
  delay: number = 100
): T => {
  return throttle(func, delay, { leading: true, trailing: false }) as T;
};

export class VirtualScrollManager {
  private container: HTMLElement | null = null;
  private items: any[] = [];
  private itemHeight: number = 50;
  private visibleCount: number = 10;
  private scrollTop: number = 0;
  private renderCallback: ((items: any[], startIndex: number) => void) | null = null;

  constructor(
    container: HTMLElement,
    items: any[],
    itemHeight: number = 50,
    visibleCount: number = 10
  ) {
    this.container = container;
    this.items = items;
    this.itemHeight = itemHeight;
    this.visibleCount = visibleCount;
    this.setupScrollListener();
  }

  private setupScrollListener(): void {
    if (!this.container) return;

    const handleScroll = optimizedThrottle(() => {
      this.scrollTop = this.container!.scrollTop;
      this.updateVisibleItems();
    }, 16);

    this.container.addEventListener('scroll', handleScroll);
  }

  private updateVisibleItems(): void {
    const startIndex = Math.floor(this.scrollTop / this.itemHeight);
    const endIndex = Math.min(startIndex + this.visibleCount, this.items.length);
    const visibleItems = this.items.slice(startIndex, endIndex);

    if (this.renderCallback) {
      this.renderCallback(visibleItems, startIndex);
    }
  }

  public setRenderCallback(callback: (items: any[], startIndex: number) => void): void {
    this.renderCallback = callback;
    this.updateVisibleItems();
  }

  public updateItems(newItems: any[]): void {
    this.items = newItems;
    this.updateVisibleItems();
  }

  public scrollToIndex(index: number): void {
    if (!this.container) return;
    
    const scrollTop = index * this.itemHeight;
    this.container.scrollTop = scrollTop;
  }

  public getVisibleRange(): { start: number; end: number } {
    const start = Math.floor(this.scrollTop / this.itemHeight);
    const end = Math.min(start + this.visibleCount, this.items.length);
    return { start, end };
  }
}

export class ImageOptimizer {
  private static cache = new Map<string, string>();
  private static observer: IntersectionObserver | null = null;

  public static initializeLazyLoading(): void {
    if (!('IntersectionObserver' in window)) return;

    this.observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          const img = entry.target as HTMLImageElement;
          const src = img.dataset.src;
          
          if (src) {
            this.loadImage(src).then((optimizedSrc) => {
              img.src = optimizedSrc;
              img.classList.remove('lazy');
              this.observer?.unobserve(img);
            });
          }
        }
      });
    }, {
      rootMargin: '50px 0px',
      threshold: 0.01
    });
  }

  public static observeImage(img: HTMLImageElement): void {
    if (this.observer) {
      this.observer.observe(img);
    }
  }

  private static async loadImage(src: string): Promise<string> {
    if (this.cache.has(src)) {
      return this.cache.get(src)!;
    }

    try {
      const response = await fetch(src);
      const blob = await response.blob();
      
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d');
      const img = new Image();
      
      return new Promise((resolve) => {
        img.onload = () => {
          const maxWidth = 800;
          const maxHeight = 600;
          
          let { width, height } = img;
          
          if (width > maxWidth || height > maxHeight) {
            const ratio = Math.min(maxWidth / width, maxHeight / height);
            width *= ratio;
            height *= ratio;
          }
          
          canvas.width = width;
          canvas.height = height;
          
          ctx?.drawImage(img, 0, 0, width, height);
          
          canvas.toBlob((optimizedBlob) => {
            if (optimizedBlob) {
              const optimizedSrc = URL.createObjectURL(optimizedBlob);
              this.cache.set(src, optimizedSrc);
              resolve(optimizedSrc);
            } else {
              resolve(src);
            }
          }, 'image/jpeg', 0.8);
        };
        
        img.src = URL.createObjectURL(blob);
      });
    } catch (error) {
      console.error('Error optimizing image:', error);
      return src;
    }
  }

  public static clearCache(): void {
    this.cache.forEach((url) => {
      URL.revokeObjectURL(url);
    });
    this.cache.clear();
  }
}

export class BundleAnalyzer {
  private static chunks: Map<string, number> = new Map();

  public static recordChunkSize(chunkName: string, size: number): void {
    this.chunks.set(chunkName, size);
  }

  public static getChunkSizes(): Map<string, number> {
    return new Map(this.chunks);
  }

  public static getTotalBundleSize(): number {
    return Array.from(this.chunks.values()).reduce((total, size) => total + size, 0);
  }

  public static getLargestChunks(count: number = 5): Array<{ name: string; size: number }> {
    return Array.from(this.chunks.entries())
      .map(([name, size]) => ({ name, size }))
      .sort((a, b) => b.size - a.size)
      .slice(0, count);
  }

  public static generateReport(): string {
    const totalSize = this.getTotalBundleSize();
    const largestChunks = this.getLargestChunks();
    
    let report = `Bundle Analysis Report\n`;
    report += `Total Bundle Size: ${(totalSize / 1024).toFixed(2)} KB\n\n`;
    report += `Largest Chunks:\n`;
    
    largestChunks.forEach(({ name, size }, index) => {
      const percentage = ((size / totalSize) * 100).toFixed(1);
      report += `${index + 1}. ${name}: ${(size / 1024).toFixed(2)} KB (${percentage}%)\n`;
    });
    
    return report;
  }
}

export const preloadCriticalResources = (resources: string[]): void => {
  resources.forEach((resource) => {
    const link = document.createElement('link');
    link.rel = 'preload';
    link.href = resource;
    
    if (resource.endsWith('.js')) {
      link.as = 'script';
    } else if (resource.endsWith('.css')) {
      link.as = 'style';
    } else if (resource.match(/\.(jpg|jpeg|png|webp|svg)$/)) {
      link.as = 'image';
    }
    
    document.head.appendChild(link);
  });
};

export const enableServiceWorker = async (): Promise<boolean> => {
  if ('serviceWorker' in navigator) {
    try {
      const registration = await navigator.serviceWorker.register('/sw.js');
      console.log('Service Worker registered:', registration);
      return true;
    } catch (error) {
      console.error('Service Worker registration failed:', error);
      return false;
    }
  }
  return false;
};

export const measureWebVitals = (): void => {
  if ('PerformanceObserver' in window) {
    const lcpObserver = new PerformanceObserver((list) => {
      const entries = list.getEntries();
      const lastEntry = entries[entries.length - 1];
      console.log('LCP:', lastEntry.startTime);
    });
    lcpObserver.observe({ type: 'largest-contentful-paint', buffered: true });

    const fidObserver = new PerformanceObserver((list) => {
      const entries = list.getEntries();
      entries.forEach((entry: any) => {
        if (entry.processingStart) {
          console.log('FID:', entry.processingStart - entry.startTime);
        }
      });
    });
    fidObserver.observe({ type: 'first-input', buffered: true });

    let clsValue = 0;
    const clsObserver = new PerformanceObserver((list) => {
      const entries = list.getEntries();
      entries.forEach((entry: any) => {
        if (!entry.hadRecentInput) {
          clsValue += entry.value;
        }
      });
      console.log('CLS:', clsValue);
    });
    clsObserver.observe({ type: 'layout-shift', buffered: true });
  }
};
