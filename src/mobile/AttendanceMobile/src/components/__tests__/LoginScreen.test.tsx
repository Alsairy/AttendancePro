import React from 'react'
import { render, fireEvent, waitFor } from '@testing-library/react-native'
import { describe, it, expect, beforeEach, jest } from '@jest/globals'
import LoginScreen from '../LoginScreen'
import { AuthService } from '../../services/AuthService'

jest.mock('../../services/AuthService')

describe('LoginScreen', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('renders login form', () => {
    const { getByPlaceholderText, getByText } = render(<LoginScreen />)
    
    expect(getByPlaceholderText('Email')).toBeTruthy()
    expect(getByPlaceholderText('Password')).toBeTruthy()
    expect(getByText('Login')).toBeTruthy()
  })

  it('handles successful login', async () => {
    const mockLogin = jest.mocked(AuthService.login)
    mockLogin.mockResolvedValue({
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
      refreshToken: 'refresh-token'
    })

    const { getByPlaceholderText, getByText } = render(<LoginScreen />)
    
    fireEvent.changeText(getByPlaceholderText('Email'), 'test@example.com')
    fireEvent.changeText(getByPlaceholderText('Password'), 'password')
    fireEvent.press(getByText('Login'))

    await waitFor(() => {
      expect(mockLogin).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password'
      })
    })
  })
})
