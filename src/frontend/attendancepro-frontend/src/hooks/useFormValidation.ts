import { useState, useCallback } from 'react';
import { InputValidator } from '../utils/inputValidation';

interface ValidationRule {
  required?: boolean;
  email?: boolean;
  password?: boolean;
  phone?: boolean;
  employeeId?: boolean;
  minLength?: number;
  maxLength?: number;
  custom?: (value: string) => string | null;
}

interface ValidationRules {
  [key: string]: ValidationRule;
}

interface ValidationErrors {
  [key: string]: string[];
}

export const useFormValidation = (rules: ValidationRules) => {
  const [errors, setErrors] = useState<ValidationErrors>({});

  const validateField = useCallback((fieldName: string, value: string): string[] => {
    const rule = rules[fieldName];
    if (!rule) return [];

    const fieldErrors: string[] = [];

    if (rule.required && !InputValidator.validateRequired(value)) {
      fieldErrors.push(`${fieldName} is required`);
    }

    if (value && rule.email && !InputValidator.validateEmail(value)) {
      fieldErrors.push('Please enter a valid email address');
    }

    if (value && rule.password) {
      const passwordValidation = InputValidator.validatePassword(value);
      if (!passwordValidation.isValid) {
        fieldErrors.push(...passwordValidation.errors);
      }
    }

    if (value && rule.phone && !InputValidator.validatePhoneNumber(value)) {
      fieldErrors.push('Please enter a valid phone number');
    }

    if (value && rule.employeeId && !InputValidator.validateEmployeeId(value)) {
      fieldErrors.push('Please enter a valid employee ID (e.g., EMP001)');
    }

    if (value && rule.minLength && !InputValidator.validateMinLength(value, rule.minLength)) {
      fieldErrors.push(`Minimum length is ${rule.minLength} characters`);
    }

    if (value && rule.maxLength && !InputValidator.validateMaxLength(value, rule.maxLength)) {
      fieldErrors.push(`Maximum length is ${rule.maxLength} characters`);
    }

    if (value && rule.custom) {
      const customError = rule.custom(value);
      if (customError) {
        fieldErrors.push(customError);
      }
    }

    return fieldErrors;
  }, [rules]);

  const validateForm = useCallback((formData: Record<string, string>): boolean => {
    const newErrors: ValidationErrors = {};
    let isValid = true;

    Object.keys(rules).forEach(fieldName => {
      const value = formData[fieldName] || '';
      const fieldErrors = validateField(fieldName, value);
      
      if (fieldErrors.length > 0) {
        newErrors[fieldName] = fieldErrors;
        isValid = false;
      }
    });

    setErrors(newErrors);
    return isValid;
  }, [rules, validateField]);

  const validateSingleField = useCallback((fieldName: string, value: string): boolean => {
    const fieldErrors = validateField(fieldName, value);
    
    setErrors(prev => ({
      ...prev,
      [fieldName]: fieldErrors
    }));

    return fieldErrors.length === 0;
  }, [validateField]);

  const clearErrors = useCallback(() => {
    setErrors({});
  }, []);

  const clearFieldError = useCallback((fieldName: string) => {
    setErrors(prev => {
      const newErrors = { ...prev };
      delete newErrors[fieldName];
      return newErrors;
    });
  }, []);

  return {
    errors,
    validateForm,
    validateSingleField,
    clearErrors,
    clearFieldError
  };
};
