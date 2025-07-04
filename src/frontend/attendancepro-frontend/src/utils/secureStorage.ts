interface SecureStorageOptions {
  httpOnly?: boolean;
  secure?: boolean;
  sameSite?: 'strict' | 'lax' | 'none';
  maxAge?: number;
}

class SecureStorage {
  private static readonly TOKEN_COOKIE_NAME = 'auth_token';
  private static readonly REFRESH_TOKEN_COOKIE_NAME = 'refresh_token';

  static setToken(token: string, options: SecureStorageOptions = {}): void {
    const defaultOptions: SecureStorageOptions = {
      secure: window.location.protocol === 'https:',
      sameSite: 'strict',
      maxAge: 60 * 60 * 24
    };

    const finalOptions = { ...defaultOptions, ...options };
    this.setCookie(this.TOKEN_COOKIE_NAME, token, finalOptions);
  }

  static getToken(): string | null {
    return this.getCookie(this.TOKEN_COOKIE_NAME);
  }

  static setRefreshToken(token: string, options: SecureStorageOptions = {}): void {
    const defaultOptions: SecureStorageOptions = {
      secure: window.location.protocol === 'https:',
      sameSite: 'strict',
      maxAge: 60 * 60 * 24 * 7
    };

    const finalOptions = { ...defaultOptions, ...options };
    this.setCookie(this.REFRESH_TOKEN_COOKIE_NAME, token, finalOptions);
  }

  static getRefreshToken(): string | null {
    return this.getCookie(this.REFRESH_TOKEN_COOKIE_NAME);
  }

  static removeToken(): void {
    this.removeCookie(this.TOKEN_COOKIE_NAME);
  }

  static removeRefreshToken(): void {
    this.removeCookie(this.REFRESH_TOKEN_COOKIE_NAME);
  }

  static clearAll(): void {
    this.removeToken();
    this.removeRefreshToken();
  }

  private static setCookie(name: string, value: string, options: SecureStorageOptions): void {
    let cookieString = `${name}=${encodeURIComponent(value)}`;

    if (options.maxAge) {
      cookieString += `; Max-Age=${options.maxAge}`;
    }

    if (options.secure) {
      cookieString += '; Secure';
    }

    if (options.sameSite) {
      cookieString += `; SameSite=${options.sameSite}`;
    }

    cookieString += '; Path=/';

    document.cookie = cookieString;
  }

  private static getCookie(name: string): string | null {
    const nameEQ = name + '=';
    const ca = document.cookie.split(';');
    
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) === 0) {
        return decodeURIComponent(c.substring(nameEQ.length, c.length));
      }
    }
    return null;
  }

  private static removeCookie(name: string): void {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  }
}

export default SecureStorage;
