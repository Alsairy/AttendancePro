import React, { useState } from 'react'
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Alert,
  ActivityIndicator
} from 'react-native'
import { AuthService } from '../../services/AuthService'
import { InputValidator } from '../../utils/InputValidator'
import { LoginCredentials } from '../../types/auth'

interface LoginFormProps {
  onSuccess?: () => void
  onError?: (error: string) => void
}

const LoginForm: React.FC<LoginFormProps> = ({ onSuccess, onError }) => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  })
  const [errors, setErrors] = useState<{ [key: string]: string }>({})
  const [loading, setLoading] = useState(false)

  const validateForm = (): boolean => {
    const newErrors: { [key: string]: string } = {}

    if (!InputValidator.validateRequired(formData.email)) {
      newErrors.email = 'Email is required'
    } else if (!InputValidator.validateEmail(formData.email)) {
      newErrors.email = 'Please enter a valid email'
    }

    if (!InputValidator.validateRequired(formData.password)) {
      newErrors.password = 'Password is required'
    } else {
      const passwordValidation = InputValidator.validatePassword(formData.password)
      if (!passwordValidation.isValid) {
        newErrors.password = passwordValidation.errors[0]
      }
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleInputChange = (field: string, value: string) => {
    const sanitizedValue = InputValidator.sanitizeInput(value)
    setFormData(prev => ({ ...prev, [field]: sanitizedValue }))
    
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev }
        delete newErrors[field]
        return newErrors
      })
    }
  }

  const handleSubmit = async () => {
    if (!validateForm()) {
      return
    }

    setLoading(true)
    try {
      const credentials = {
        email: formData.email,
        password: formData.password
      }
      
      await AuthService.login(credentials as any)
      onSuccess?.()
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Login failed'
      onError?.(errorMessage)
      Alert.alert('Error', errorMessage)
    } finally {
      setLoading(false)
    }
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Login</Text>
      
      <View style={styles.inputContainer}>
        <TextInput
          style={[styles.input, errors.email ? styles.inputError : null]}
          placeholder="Email"
          value={formData.email}
          onChangeText={(value) => handleInputChange('email', value)}
          keyboardType="email-address"
          autoCapitalize="none"
          editable={!loading}
        />
        {errors.email && <Text style={styles.errorText}>{errors.email}</Text>}
      </View>
      
      <View style={styles.inputContainer}>
        <TextInput
          style={[styles.input, errors.password ? styles.inputError : null]}
          placeholder="Password"
          value={formData.password}
          onChangeText={(value) => handleInputChange('password', value)}
          secureTextEntry
          editable={!loading}
        />
        {errors.password && <Text style={styles.errorText}>{errors.password}</Text>}
      </View>
      
      <TouchableOpacity
        style={[styles.button, loading && styles.buttonDisabled]}
        onPress={handleSubmit}
        disabled={loading}
      >
        {loading ? (
          <ActivityIndicator color="#fff" />
        ) : (
          <Text style={styles.buttonText}>Login</Text>
        )}
      </TouchableOpacity>
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    padding: 20
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: 30,
    color: '#333'
  },
  inputContainer: {
    marginBottom: 15
  },
  input: {
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
    fontSize: 16
  },
  inputError: {
    borderColor: '#ff4444'
  },
  errorText: {
    color: '#ff4444',
    fontSize: 12,
    marginTop: 5,
    marginLeft: 5
  },
  button: {
    backgroundColor: '#007bff',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10
  },
  buttonDisabled: {
    opacity: 0.6
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold'
  }
})

export default LoginForm
