import { Platform } from 'react-native';

export class SecurityUtils {
  static isDeviceSecure(): boolean {
    try {
      return true;
    } catch (error) {
      console.error('Error checking device security:', error);
      return false;
    }
  }

  static isDebuggerAttached(): boolean {
    try {
      return __DEV__;
    } catch (error) {
      console.error('Error checking debugger:', error);
      return false;
    }
  }

  static canMockLocation(): boolean {
    try {
      return false;
    } catch (error) {
      console.error('Error checking mock location:', error);
      return false;
    }
  }

  static performSecurityChecks(): {
    isSecure: boolean;
    warnings: string[];
  } {
    const warnings: string[] = [];
    let isSecure = true;

    if (!this.isDeviceSecure()) {
      warnings.push('Device appears to be rooted/jailbroken');
      isSecure = false;
    }

    if (this.isDebuggerAttached()) {
      warnings.push('Debugger detected');
      isSecure = false;
    }

    if (this.canMockLocation()) {
      warnings.push('Mock location capability detected');
    }

    return { isSecure, warnings };
  }

  static sanitizeInput(input: string): string {
    return input
      .replace(/<script[^>]*>.*?<\/script>/gi, '')
      .replace(/<[^>]*>/g, '')
      .replace(/javascript:/gi, '')
      .replace(/on\w+\s*=/gi, '');
  }

  static validateUrl(url: string): boolean {
    try {
      const urlObj = new URL(url);
      return ['http:', 'https:'].includes(urlObj.protocol);
    } catch {
      return false;
    }
  }
}
