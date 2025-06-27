import { useEffect, useRef, useState } from 'react';
import { PerformanceMonitor, PerformanceMetrics } from '../utils/performanceOptimization';

export const usePerformanceMonitoring = (componentName: string) => {
  const renderStartTime = useRef<number>(Date.now());
  const [metrics, setMetrics] = useState<PerformanceMetrics | null>(null);
  const monitor = PerformanceMonitor.getInstance();

  useEffect(() => {
    const startTime = renderStartTime.current;
    monitor.markStart(`${componentName}-render`);

    return () => {
      monitor.markEnd(`${componentName}-render`);
      const renderTime = Date.now() - startTime;
      
      if (renderTime > 100) {
        console.warn(`Slow render detected in ${componentName}: ${renderTime}ms`);
      }
    };
  }, [componentName, monitor]);

  useEffect(() => {
    const updateMetrics = () => {
      const currentMetrics = monitor.getAverageMetrics();
      setMetrics(currentMetrics);
    };

    const interval = setInterval(updateMetrics, 5000);
    updateMetrics();

    return () => clearInterval(interval);
  }, [monitor]);

  const measureOperation = (operationName: string, operation: () => void | Promise<void>) => {
    const startTime = performance.now();
    monitor.markStart(operationName);

    const result = operation();

    if (result instanceof Promise) {
      return result.finally(() => {
        monitor.markEnd(operationName);
        const duration = performance.now() - startTime;
        console.log(`${operationName} completed in ${duration.toFixed(2)}ms`);
      });
    } else {
      monitor.markEnd(operationName);
      const duration = performance.now() - startTime;
      console.log(`${operationName} completed in ${duration.toFixed(2)}ms`);
      return result;
    }
  };

  return {
    metrics,
    measureOperation,
    markStart: (name: string) => monitor.markStart(name),
    markEnd: (name: string) => monitor.markEnd(name)
  };
};

export const useImageOptimization = () => {
  const [isSupported, setIsSupported] = useState(false);

  useEffect(() => {
    setIsSupported('IntersectionObserver' in window);
  }, []);

  const optimizeImage = (src: string, maxWidth = 800, maxHeight = 600, quality = 0.8): Promise<string> => {
    return new Promise((resolve) => {
      const img = new Image();
      img.crossOrigin = 'anonymous';
      
      img.onload = () => {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');
        
        let { width, height } = img;
        
        if (width > maxWidth || height > maxHeight) {
          const ratio = Math.min(maxWidth / width, maxHeight / height);
          width *= ratio;
          height *= ratio;
        }
        
        canvas.width = width;
        canvas.height = height;
        
        ctx?.drawImage(img, 0, 0, width, height);
        
        canvas.toBlob((blob) => {
          if (blob) {
            const optimizedUrl = URL.createObjectURL(blob);
            resolve(optimizedUrl);
          } else {
            resolve(src);
          }
        }, 'image/jpeg', quality);
      };
      
      img.onerror = () => resolve(src);
      img.src = src;
    });
  };

  return { isSupported, optimizeImage };
};

export const useServiceWorker = () => {
  const [isSupported, setIsSupported] = useState(false);
  const [isRegistered, setIsRegistered] = useState(false);
  const [updateAvailable, setUpdateAvailable] = useState(false);

  useEffect(() => {
    const supported = 'serviceWorker' in navigator;
    setIsSupported(supported);

    if (supported) {
      navigator.serviceWorker.register('/sw.js')
        .then((registration) => {
          setIsRegistered(true);
          console.log('Service Worker registered:', registration);

          registration.addEventListener('updatefound', () => {
            const newWorker = registration.installing;
            if (newWorker) {
              newWorker.addEventListener('statechange', () => {
                if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                  setUpdateAvailable(true);
                }
              });
            }
          });
        })
        .catch((error) => {
          console.error('Service Worker registration failed:', error);
        });
    }
  }, []);

  const updateServiceWorker = () => {
    if ('serviceWorker' in navigator) {
      navigator.serviceWorker.getRegistration().then((registration) => {
        if (registration) {
          registration.update();
          window.location.reload();
        }
      });
    }
  };

  const clearCache = async (): Promise<boolean> => {
    if ('serviceWorker' in navigator && 'caches' in window) {
      try {
        const cacheNames = await caches.keys();
        await Promise.all(cacheNames.map(name => caches.delete(name)));
        return true;
      } catch (error) {
        console.error('Failed to clear cache:', error);
        return false;
      }
    }
    return false;
  };

  return {
    isSupported,
    isRegistered,
    updateAvailable,
    updateServiceWorker,
    clearCache
  };
};

export const useBundleAnalyzer = () => {
  const [bundleInfo, setBundleInfo] = useState<{
    totalSize: number;
    chunks: Array<{ name: string; size: number }>;
  } | null>(null);

  useEffect(() => {
    const mockBundleInfo = {
      totalSize: 2048000, // 2MB
      chunks: [
        { name: 'main', size: 512000 },
        { name: 'vendor', size: 1024000 },
        { name: 'runtime', size: 256000 },
        { name: 'async-components', size: 256000 }
      ]
    };
    
    setBundleInfo(mockBundleInfo);
  }, []);

  const getLargestChunks = (count = 5) => {
    if (!bundleInfo) return [];
    return bundleInfo.chunks
      .sort((a, b) => b.size - a.size)
      .slice(0, count);
  };

  const formatSize = (bytes: number) => {
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    if (bytes === 0) return '0 Bytes';
    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    return Math.round(bytes / Math.pow(1024, i) * 100) / 100 + ' ' + sizes[i];
  };

  return {
    bundleInfo,
    getLargestChunks,
    formatSize
  };
};
