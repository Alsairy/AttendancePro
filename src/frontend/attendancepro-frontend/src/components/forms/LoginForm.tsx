import React, { useState } from 'react'
import { useAuth } from '../../contexts/AuthContext'
import { useFormValidation } from '../../hooks/useFormValidation'
import { InputValidator } from '../../utils/inputValidation'

interface LoginFormProps {
  onSuccess?: () => void
  onError?: (error: string) => void
}

const LoginForm: React.FC<LoginFormProps> = ({ onSuccess, onError }) => {
  const { login } = useAuth()
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    twoFactorCode: ''
  })
  const [loading, setLoading] = useState(false)
  const [requiresTwoFactor, setRequiresTwoFactor] = useState(false)

  const { errors, validateForm, validateSingleField } = useFormValidation({
    email: { required: true, email: true },
    password: { required: true, minLength: 8 },
    twoFactorCode: { required: requiresTwoFactor, minLength: 6, maxLength: 6 }
  })

  const handleInputChange = (field: string, value: string) => {
    const sanitizedValue = InputValidator.sanitizeInput(value)
    setFormData(prev => ({ ...prev, [field]: sanitizedValue }))
    validateSingleField(field, sanitizedValue)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (!validateForm(formData)) {
      return
    }

    setLoading(true)
    try {
      const result = await login(formData.email, formData.password, formData.twoFactorCode)
      
      if (result.requiresTwoFactor) {
        setRequiresTwoFactor(true)
        return
      }

      if (result.success) {
        onSuccess?.()
      } else {
        onError?.(result.error || 'Login failed')
      }
    } catch (error) {
      onError?.(error instanceof Error ? error.message : 'Login failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
          Email
        </label>
        <input
          id="email"
          type="email"
          data-testid="email-input"
          value={formData.email}
          onChange={(e) => handleInputChange('email', e.target.value)}
          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          disabled={loading}
        />
        {errors.email && (
          <div data-testid="email-error" className="mt-1 text-sm text-red-600">
            {errors.email.join(', ')}
          </div>
        )}
      </div>

      <div>
        <label htmlFor="password" className="block text-sm font-medium text-gray-700">
          Password
        </label>
        <input
          id="password"
          type="password"
          data-testid="password-input"
          value={formData.password}
          onChange={(e) => handleInputChange('password', e.target.value)}
          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          disabled={loading}
        />
        {errors.password && (
          <div data-testid="password-error" className="mt-1 text-sm text-red-600">
            {errors.password.join(', ')}
          </div>
        )}
      </div>

      {requiresTwoFactor && (
        <div>
          <label htmlFor="twoFactorCode" className="block text-sm font-medium text-gray-700">
            Two-Factor Code
          </label>
          <input
            id="twoFactorCode"
            type="text"
            data-testid="two-factor-input"
            value={formData.twoFactorCode}
            onChange={(e) => handleInputChange('twoFactorCode', e.target.value)}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            disabled={loading}
            maxLength={6}
          />
          {errors.twoFactorCode && (
            <div className="mt-1 text-sm text-red-600">
              {errors.twoFactorCode.join(', ')}
            </div>
          )}
        </div>
      )}

      <button
        type="submit"
        data-testid="login-button"
        disabled={loading}
        className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
      >
        {loading ? 'Signing in...' : 'Sign in'}
      </button>
    </form>
  )
}

export default LoginForm
