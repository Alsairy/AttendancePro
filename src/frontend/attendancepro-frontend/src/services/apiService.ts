import { authService } from './authService'
import CsrfService from './csrfService'

class ApiService {
  private baseURL: string

  constructor() {
    this.baseURL = (import.meta as any).env?.VITE_API_BASE_URL || 'http://localhost:5000/api'
  }

  private async getHeaders(): Promise<Record<string, string>> {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json'
    }

    const token = (authService as any).getToken?.()
    if (token) {
      headers.Authorization = `Bearer ${token}`
    }

    try {
      const csrfHeaders = await CsrfService.getHeaders()
      Object.assign(headers, csrfHeaders)
    } catch (error) {
      console.warn('Failed to get CSRF token:', error)
    }

    return headers
  }

  async get<T>(endpoint: string): Promise<T> {
    const headers = await this.getHeaders()
    
    const response = await fetch(`${this.baseURL}${endpoint}`, {
      method: 'GET',
      headers,
      credentials: 'include'
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return response.json()
  }

  async post<T>(endpoint: string, data?: any): Promise<T> {
    const headers = await this.getHeaders()
    
    const response = await fetch(`${this.baseURL}${endpoint}`, {
      method: 'POST',
      headers,
      credentials: 'include',
      body: data ? JSON.stringify(data) : undefined
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return response.json()
  }

  async put<T>(endpoint: string, data?: any): Promise<T> {
    const headers = await this.getHeaders()
    
    const response = await fetch(`${this.baseURL}${endpoint}`, {
      method: 'PUT',
      headers,
      credentials: 'include',
      body: data ? JSON.stringify(data) : undefined
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return response.json()
  }

  async delete<T>(endpoint: string): Promise<T> {
    const headers = await this.getHeaders()
    
    const response = await fetch(`${this.baseURL}${endpoint}`, {
      method: 'DELETE',
      headers,
      credentials: 'include'
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return response.json()
  }
}

export const apiService = new ApiService()
