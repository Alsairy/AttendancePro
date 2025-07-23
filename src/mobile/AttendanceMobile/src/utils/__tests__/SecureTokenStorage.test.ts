import SecureTokenStorage from '../SecureTokenStorage'
import * as Keychain from 'react-native-keychain'

jest.mock('react-native-keychain')
jest.mock('react-native', () => ({
  Platform: {
    OS: 'android' // Force non-iOS for tests to avoid simulator protection
  }
}))
jest.mock('@react-native-async-storage/async-storage', () => ({
  setItem: jest.fn(),
  getItem: jest.fn(),
  removeItem: jest.fn()
}))

// Add this after jest.mock('react-native-keychain')
const mockSetInternetCredentials = Keychain.setInternetCredentials as jest.Mock
const mockGetInternetCredentials = Keychain.getInternetCredentials as jest.Mock
const mockResetInternetCredentials = Keychain.resetInternetCredentials as jest.Mock

describe('SecureTokenStorage', () => {
  beforeEach(() => {
    jest.clearAllMocks()
    mockSetInternetCredentials.mockReset()
    mockGetInternetCredentials.mockReset()
    mockResetInternetCredentials.mockReset()
  })

  describe('setToken', () => {
    it('should store token securely', async () => {
      mockSetInternetCredentials.mockResolvedValue(true)

      await SecureTokenStorage.setToken('test-token')

      expect(mockSetInternetCredentials).toHaveBeenCalledWith(
        'auth_token',
        'token',
        'test-token',
        expect.objectContaining({
          accessControl: Keychain.ACCESS_CONTROL.BIOMETRY_CURRENT_SET_OR_DEVICE_PASSCODE,
          authenticationType: Keychain.AUTHENTICATION_TYPE.DEVICE_PASSCODE_OR_BIOMETRICS
        })
      )
    })
  })

  describe('getToken', () => {
    it('should retrieve token from secure storage', async () => {
      mockGetInternetCredentials.mockResolvedValue({
        username: 'token',
        password: 'test-token',
        service: 'auth_token',
        storage: 'KC'
      })

      const token = await SecureTokenStorage.getToken()

      expect(token).toBe('test-token')
      expect(mockGetInternetCredentials).toHaveBeenCalledWith('auth_token')
    })

    it('should return null when no token exists', async () => {
      mockGetInternetCredentials.mockResolvedValue(false)

      const token = await SecureTokenStorage.getToken()

      expect(token).toBeNull()
    })
  })

  describe('clearAll', () => {
    it('should remove all stored credentials', async () => {
      mockResetInternetCredentials.mockResolvedValue(true)

      await SecureTokenStorage.clearAll()

      expect(mockResetInternetCredentials).toHaveBeenCalledTimes(3)
    })
  })
})
