class CsrfService {
  private static token: string | null = null;

  static async getToken(): Promise<string> {
    if (!this.token) {
      try {
        const response = await fetch('/api/csrf/token', {
          method: 'GET',
          credentials: 'include'
        });
        
        if (response.ok) {
          const data = await response.json();
          this.token = data.token;
        } else {
          throw new Error('Failed to get CSRF token');
        }
      } catch (error) {
        console.error('Error fetching CSRF token:', error);
        throw error;
      }
    }
    
    return this.token!;
  }

  static clearToken(): void {
    this.token = null;
  }

  static async getHeaders(): Promise<Record<string, string>> {
    const token = await this.getToken();
    return {
      'X-CSRF-TOKEN': token
    };
  }
}

export default CsrfService;
