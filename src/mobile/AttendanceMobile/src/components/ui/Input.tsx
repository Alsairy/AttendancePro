import React from 'react'
import { View, Text, TextInput, StyleSheet, TextInputProps, ViewStyle } from 'react-native'

interface InputProps extends TextInputProps {
  label?: string
  error?: string
  helperText?: string
  containerStyle?: ViewStyle
}

const Input: React.FC<InputProps> = ({
  label,
  error,
  helperText,
  containerStyle,
  style,
  ...props
}) => {
  const inputStyle = [
    styles.input,
    error && styles.inputError,
    style
  ]

  return (
    <View style={[styles.container, containerStyle]}>
      {label && <Text style={styles.label}>{label}</Text>}
      <TextInput style={inputStyle as any} {...props} />
      {error && <Text style={styles.errorText}>{error}</Text>}
      {helperText && !error && <Text style={styles.helperText}>{helperText}</Text>}
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    marginBottom: 16
  },
  label: {
    fontSize: 14,
    fontWeight: '500',
    color: '#374151',
    marginBottom: 4
  },
  input: {
    borderWidth: 1,
    borderColor: '#d1d5db',
    borderRadius: 8,
    paddingHorizontal: 12,
    paddingVertical: 10,
    fontSize: 16,
    backgroundColor: '#fff'
  },
  inputError: {
    borderColor: '#ef4444'
  },
  errorText: {
    fontSize: 12,
    color: '#ef4444',
    marginTop: 4
  },
  helperText: {
    fontSize: 12,
    color: '#6b7280',
    marginTop: 4
  }
})

export default Input
