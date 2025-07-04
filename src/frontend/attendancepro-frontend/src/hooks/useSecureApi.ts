import { useState, useCallback } from 'react';
import CsrfService from '../services/csrfService';
import SecureStorage from '../utils/secureStorage';

interface ApiOptions {
  method?: string;
  body?: any;
  headers?: Record<string, string>;
}

export const useSecureApi = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const secureRequest = useCallback(async (url: string, options: ApiOptions = {}) => {
    setLoading(true);
    setError(null);

    try {
      const token = SecureStorage.getToken();
      const csrfHeaders = await CsrfService.getHeaders();
      
      const headers = {
        'Content-Type': 'application/json',
        'Authorization': token ? `Bearer ${token}` : '',
        ...csrfHeaders,
        ...options.headers
      };

      const response = await fetch(url, {
        method: options.method || 'GET',
        headers,
        body: options.body ? JSON.stringify(options.body) : undefined,
        credentials: 'include'
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      return data;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'An error occurred';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return { secureRequest, loading, error };
};
