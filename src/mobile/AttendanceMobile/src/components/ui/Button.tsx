import React from 'react'
import { TouchableOpacity, Text, StyleSheet, ActivityIndicator, ViewStyle, TextStyle } from 'react-native'

interface ButtonProps {
  title: string
  onPress: () => void
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  loading?: boolean
  disabled?: boolean
  style?: ViewStyle
  textStyle?: TextStyle
}

const Button: React.FC<ButtonProps> = ({
  title,
  onPress,
  variant = 'primary',
  size = 'md',
  loading = false,
  disabled = false,
  style,
  textStyle
}) => {
  const buttonStyle = [
    styles.base,
    styles[variant],
    styles[size],
    (disabled || loading) && styles.disabled,
    style
  ]

  const textStyles = [
    styles.text,
    styles[`${variant}Text`],
    styles[`${size}Text`],
    textStyle
  ]

  return (
    <TouchableOpacity
      style={buttonStyle}
      onPress={onPress}
      disabled={disabled || loading}
      activeOpacity={0.7}
    >
      {loading ? (
        <ActivityIndicator color={variant === 'primary' ? '#fff' : '#007bff'} />
      ) : (
        <Text style={textStyles}>{title}</Text>
      )}
    </TouchableOpacity>
  )
}

const styles = StyleSheet.create({
  base: {
    borderRadius: 8,
    alignItems: 'center',
    justifyContent: 'center',
    flexDirection: 'row'
  },
  primary: {
    backgroundColor: '#007bff'
  },
  secondary: {
    backgroundColor: '#6c757d'
  },
  danger: {
    backgroundColor: '#dc3545'
  },
  sm: {
    paddingVertical: 8,
    paddingHorizontal: 12
  },
  md: {
    paddingVertical: 12,
    paddingHorizontal: 16
  },
  lg: {
    paddingVertical: 16,
    paddingHorizontal: 20
  },
  disabled: {
    opacity: 0.6
  },
  text: {
    fontWeight: '600'
  },
  primaryText: {
    color: '#fff'
  },
  secondaryText: {
    color: '#fff'
  },
  dangerText: {
    color: '#fff'
  },
  smText: {
    fontSize: 14
  },
  mdText: {
    fontSize: 16
  },
  lgText: {
    fontSize: 18
  }
})

export default Button
