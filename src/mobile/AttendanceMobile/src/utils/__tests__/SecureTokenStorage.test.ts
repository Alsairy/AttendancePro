import SecureTokenStorage from '../SecureTokenStorage'
import * as Keychain from 'react-native-keychain'

jest.mock('react-native-keychain')

describe('SecureTokenStorage', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  describe('setToken', () => {
    it('should store token securely', async () => {
      const mockSetInternetCredentials = jest.mocked(Keychain.setInternetCredentials)
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
      const mockGetInternetCredentials = jest.mocked(Keychain.getInternetCredentials)
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
      const mockGetInternetCredentials = jest.mocked(Keychain.getInternetCredentials)
      mockGetInternetCredentials.mockResolvedValue(false)

      const token = await SecureTokenStorage.getToken()

      expect(token).toBeNull()
    })
  })

  describe('clearAll', () => {
    it('should remove all stored credentials', async () => {
      const mockResetInternetCredentials = jest.mocked(Keychain.resetInternetCredentials)
      mockResetInternetCredentials.mockResolvedValue(true)

      await SecureTokenStorage.clearAll()

      expect(mockResetInternetCredentials).toHaveBeenCalledTimes(3)
    })
  })
})
