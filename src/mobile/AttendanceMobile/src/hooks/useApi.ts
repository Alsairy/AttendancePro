import { useState, useCallback } from 'react'
import { apiService } from '../services/ApiService'

interface UseApiState<T> {
  data: T | null
  loading: boolean
  error: string | null
}

export const useApi = <T>() => {
  const [state, setState] = useState<UseApiState<T>>({
    data: null,
    loading: false,
    error: null
  })

  const execute = useCallback(async (apiCall: () => Promise<T>) => {
    setState(prev => ({ ...prev, loading: true, error: null }))
    
    try {
      const data = await apiCall()
      setState({ data, loading: false, error: null })
      return data
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'An error occurred'
      setState(prev => ({ ...prev, loading: false, error: errorMessage }))
      throw error
    }
  }, [])

  const get = useCallback((endpoint: string) => {
    return execute(() => apiService.get<T>(endpoint))
  }, [execute])

  const post = useCallback((endpoint: string, data?: any) => {
    return execute(() => apiService.post<T>(endpoint, data))
  }, [execute])

  const put = useCallback((endpoint: string, data?: any) => {
    return execute(() => apiService.put<T>(endpoint, data))
  }, [execute])

  const del = useCallback((endpoint: string) => {
    return execute(() => apiService.delete<T>(endpoint))
  }, [execute])

  return {
    ...state,
    get,
    post,
    put,
    delete: del,
    execute
  }
}
