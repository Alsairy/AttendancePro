import React from 'react'
import { render, fireEvent, waitFor } from '@testing-library/react-native'
import { describe, it, expect, beforeEach, jest } from '@jest/globals'
import LoginScreen from '../LoginScreen'
import type { AuthResponse } from '../../types/User'

jest.mock('../../services/AuthService')

import { AuthService } from '../../services/AuthService'
const mockAuthService = AuthService as jest.Mocked<typeof AuthService>

describe('LoginScreen', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('renders login form', () => {
    const { getByPlaceholderText, getAllByText } = render(<LoginScreen />)
    
    expect(getByPlaceholderText('Email')).toBeTruthy()
    expect(getByPlaceholderText('Password')).toBeTruthy()
    expect(getAllByText('Login').length).toBeGreaterThan(0)
  })

  it('handles successful login', async () => {
    mockAuthService.login.mockResolvedValue({
      token: 'mock-token',
      user: { 
        id: '1', 
        firstName: 'John', 
        lastName: 'Doe', 
        email: 'john@example.com',
        isActive: true,
        tenantId: 'tenant-1',
        createdAt: '2024-01-01T00:00:00Z'
      },
      refreshToken: 'refresh-token',
      expiresAt: '2024-01-01T01:00:00Z'
    })

    const { getByPlaceholderText, getAllByText } = render(<LoginScreen />)
    
    fireEvent.changeText(getByPlaceholderText('Email'), 'test@example.com')
    fireEvent.changeText(getByPlaceholderText('Password'), 'password')
    const loginElements = getAllByText('Login')
    fireEvent.press(loginElements[loginElements.length - 1])

    await waitFor(() => {
      expect(mockAuthService.login).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password',
        tenantSubdomain: 'default'
      })
    }, { timeout: 3000 })
  })
})
