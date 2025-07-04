import * as Keychain from 'react-native-keychain';

class SecureTokenStorage {
  private static readonly TOKEN_KEY = 'auth_token';
  private static readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private static readonly USER_KEY = 'user_data';

  static async setToken(token: string): Promise<void> {
    try {
      await Keychain.setInternetCredentials(
        this.TOKEN_KEY,
        'token',
        token,
        {
          accessControl: Keychain.ACCESS_CONTROL.BIOMETRY_CURRENT_SET_OR_DEVICE_PASSCODE,
          authenticationType: Keychain.AUTHENTICATION_TYPE.DEVICE_PASSCODE_OR_BIOMETRICS,
          accessGroup: 'group.com.hudur.attendance',
          storage: Keychain.STORAGE_TYPE.KC
        }
      );
    } catch (error) {
      console.error('Error storing token:', error);
      throw error;
    }
  }

  static async getToken(): Promise<string | null> {
    try {
      const credentials = await Keychain.getInternetCredentials(this.TOKEN_KEY);
      if (credentials && credentials.password) {
        return credentials.password;
      }
      return null;
    } catch (error) {
      console.error('Error retrieving token:', error);
      return null;
    }
  }

  static async setRefreshToken(token: string): Promise<void> {
    try {
      await Keychain.setInternetCredentials(
        this.REFRESH_TOKEN_KEY,
        'refresh_token',
        token,
        {
          accessControl: Keychain.ACCESS_CONTROL.BIOMETRY_CURRENT_SET_OR_DEVICE_PASSCODE,
          authenticationType: Keychain.AUTHENTICATION_TYPE.DEVICE_PASSCODE_OR_BIOMETRICS,
          accessGroup: 'group.com.hudur.attendance',
          storage: Keychain.STORAGE_TYPE.KC
        }
      );
    } catch (error) {
      console.error('Error storing refresh token:', error);
      throw error;
    }
  }

  static async getRefreshToken(): Promise<string | null> {
    try {
      const credentials = await Keychain.getInternetCredentials(this.REFRESH_TOKEN_KEY);
      if (credentials && credentials.password) {
        return credentials.password;
      }
      return null;
    } catch (error) {
      console.error('Error retrieving refresh token:', error);
      return null;
    }
  }

  static async setUserData(userData: any): Promise<void> {
    try {
      await Keychain.setInternetCredentials(
        this.USER_KEY,
        'user',
        JSON.stringify(userData),
        {
          accessControl: Keychain.ACCESS_CONTROL.BIOMETRY_CURRENT_SET_OR_DEVICE_PASSCODE,
          authenticationType: Keychain.AUTHENTICATION_TYPE.DEVICE_PASSCODE_OR_BIOMETRICS,
          accessGroup: 'group.com.hudur.attendance',
          storage: Keychain.STORAGE_TYPE.KC
        }
      );
    } catch (error) {
      console.error('Error storing user data:', error);
      throw error;
    }
  }

  static async getUserData(): Promise<any | null> {
    try {
      const credentials = await Keychain.getInternetCredentials(this.USER_KEY);
      if (credentials && credentials.password) {
        return JSON.parse(credentials.password);
      }
      return null;
    } catch (error) {
      console.error('Error retrieving user data:', error);
      return null;
    }
  }

  static async removeToken(): Promise<void> {
    try {
      await Keychain.resetInternetCredentials(this.TOKEN_KEY);
    } catch (error) {
      console.error('Error removing token:', error);
    }
  }

  static async removeRefreshToken(): Promise<void> {
    try {
      await Keychain.resetInternetCredentials(this.REFRESH_TOKEN_KEY);
    } catch (error) {
      console.error('Error removing refresh token:', error);
    }
  }

  static async removeUserData(): Promise<void> {
    try {
      await Keychain.resetInternetCredentials(this.USER_KEY);
    } catch (error) {
      console.error('Error removing user data:', error);
    }
  }

  static async clearAll(): Promise<void> {
    await Promise.all([
      this.removeToken(),
      this.removeRefreshToken(),
      this.removeUserData()
    ]);
  }
}

export default SecureTokenStorage;
