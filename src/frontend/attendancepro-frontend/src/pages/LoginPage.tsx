import React, { useState } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import LoginForm from '../components/forms/LoginForm'
import ErrorBoundary from '../components/ErrorBoundary'

const LoginPage: React.FC = () => {
  const { isAuthenticated } = useAuth()
  const [error, setError] = useState<string | null>(null)

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />
  }

  const handleLoginSuccess = () => {
    setError(null)
  }

  const handleLoginError = (errorMessage: string) => {
    setError(errorMessage)
  }

  return (
    <ErrorBoundary>
      <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-md w-full space-y-8">
          <div>
            <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
              Sign in to your account
            </h2>
            <p className="mt-2 text-center text-sm text-gray-600">
              Hudur Enterprise Platform
            </p>
          </div>
          
          {error && (
            <div data-testid="error-message" className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded relative">
              {error}
            </div>
          )}

          <LoginForm 
            onSuccess={handleLoginSuccess}
            onError={handleLoginError}
          />
        </div>
      </div>
    </ErrorBoundary>
  )
}

export default LoginPage
