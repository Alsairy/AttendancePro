import { render, screen, waitFor } from '@testing-library/react'
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { AuthProvider, useAuth } from '../../contexts/AuthContext'
import { authService } from '../../services/authService'

vi.mock('../../services/authService')
vi.mock('../../utils/secureStorage')

const TestComponent = () => {
  const { user, isAuthenticated, login, logout } = useAuth()
  
  return (
    <div>
      <div data-testid="auth-status">
        {isAuthenticated ? 'Authenticated' : 'Not Authenticated'}
      </div>
      <div data-testid="user-name">
        {user ? `${user.firstName} ${user.lastName}` : 'No User'}
      </div>
      <button onClick={() => login('test@example.com', 'password')}>
        Login
      </button>
      <button onClick={logout}>Logout</button>
    </div>
  )
}

describe('AuthContext', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('provides authentication context', () => {
    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    )

    expect(screen.getByTestId('auth-status')).toHaveTextContent('Not Authenticated')
    expect(screen.getByTestId('user-name')).toHaveTextContent('No User')
  })

  it('handles successful login', async () => {
    const mockUser = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      email: 'john@example.com',
      phoneNumber: '+1234567890',
      employeeId: 'EMP001',
      department: 'IT',
      position: 'Developer',
      profilePictureUrl: 'https://example.com/avatar.jpg',
      status: 'Active',
      roles: ['Employee']
    }

    vi.mocked(authService.login).mockResolvedValue({
      access_token: 'mock-token',
      user: mockUser
    } as any)

    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    )

    const loginButton = screen.getByText('Login')
    loginButton.click()

    await waitFor(() => {
      expect(screen.getByTestId('auth-status')).toHaveTextContent('Authenticated')
      expect(screen.getByTestId('user-name')).toHaveTextContent('John Doe')
    })
  })
})
