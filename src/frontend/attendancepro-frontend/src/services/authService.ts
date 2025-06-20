import axios, { AxiosResponse } from 'axios'

const API_BASE_URL = (import.meta as any).env?.VITE_API_BASE_URL || 'http://localhost:5000/api'

interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: User
  requiresTwoFactor?: boolean
}

interface User {
  id: string
  firstName: string
  lastName: string
  email: string
  phoneNumber?: string
  employeeId?: string
  department?: string
  position?: string
  profilePictureUrl?: string
  status: string
  roles: string[]
}

interface RegisterData {
  firstName: string
  lastName: string
  email: string
  password: string
  phoneNumber?: string
  employeeId?: string
  department?: string
  position?: string
}

class AuthService {
  private api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
      'Content-Type': 'application/json',
    },
  })

  constructor() {
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('accessToken')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    this.api.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config
        
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true
          
          const refreshToken = localStorage.getItem('refreshToken')
          if (refreshToken) {
            try {
              const response = await this.refreshToken(refreshToken)
              localStorage.setItem('accessToken', response.accessToken)
              localStorage.setItem('refreshToken', response.refreshToken)
              
              originalRequest.headers.Authorization = `Bearer ${response.accessToken}`
              return this.api(originalRequest)
            } catch (refreshError) {
              localStorage.removeItem('accessToken')
              localStorage.removeItem('refreshToken')
              window.location.href = '/login'
              return Promise.reject(refreshError)
            }
          }
        }
        
        return Promise.reject(error)
      }
    )
  }

  async login(email: string, password: string, twoFactorCode?: string): Promise<LoginResponse> {
    try {
      const response: AxiosResponse<LoginResponse> = await this.api.post('/auth/login', {
        email,
        password,
        twoFactorCode,
      })
      return response.data
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Login failed')
    }
  }

  async register(userData: RegisterData): Promise<void> {
    try {
      await this.api.post('/auth/register', userData)
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Registration failed')
    }
  }

  async logout(userId: string): Promise<void> {
    try {
      await this.api.post('/auth/logout', { userId })
    } catch (error: any) {
      console.error('Logout error:', error)
    }
  }

  async refreshToken(refreshToken: string): Promise<LoginResponse> {
    try {
      const response: AxiosResponse<LoginResponse> = await this.api.post('/auth/refresh-token', {
        refreshToken,
      })
      return response.data
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Token refresh failed')
    }
  }

  async getCurrentUser(): Promise<User> {
    try {
      const response: AxiosResponse<User> = await this.api.get('/auth/me')
      return response.data
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to get current user')
    }
  }

  async forgotPassword(email: string): Promise<void> {
    try {
      await this.api.post('/auth/forgot-password', { email })
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to send reset email')
    }
  }

  async resetPassword(token: string, newPassword: string): Promise<void> {
    try {
      await this.api.post('/auth/reset-password', { token, newPassword })
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Password reset failed')
    }
  }

  async changePassword(userId: string, currentPassword: string, newPassword: string): Promise<void> {
    try {
      await this.api.post('/auth/change-password', {
        userId,
        currentPassword,
        newPassword,
      })
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Password change failed')
    }
  }

  async validateTwoFactor(userId: string, code: string): Promise<LoginResponse> {
    try {
      const response: AxiosResponse<LoginResponse> = await this.api.post('/auth/validate-2fa', {
        userId,
        code,
      })
      return response.data
    } catch (error: any) {
      throw new Error(error.response?.data?.message || '2FA validation failed')
    }
  }

  async setupTwoFactor(userId: string): Promise<{ qrCodeUrl: string; secret: string }> {
    try {
      const response = await this.api.post('/auth/setup-2fa', { userId })
      return response.data
    } catch (error: any) {
      throw new Error(error.response?.data?.message || '2FA setup failed')
    }
  }

  async enableTwoFactor(userId: string, code: string): Promise<void> {
    try {
      await this.api.post('/auth/enable-2fa', { userId, code })
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to enable 2FA')
    }
  }

  async disableTwoFactor(userId: string, password: string): Promise<void> {
    try {
      await this.api.post('/auth/disable-2fa', { userId, password })
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to disable 2FA')
    }
  }
}

export const authService = new AuthService()
export type { AuthService, User, RegisterData, LoginResponse }
