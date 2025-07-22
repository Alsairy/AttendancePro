import * as Keychain from 'react-native-keychain';
import { Platform } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';

class SecureTokenStorage {
  private static readonly TOKEN_KEY = 'auth_token';
  private static readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private static readonly USER_KEY = 'user_data';

  static async setToken(token: string): Promise<void> {
    try {
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for token on iOS simulator');
        await AsyncStorage.setItem(this.TOKEN_KEY, token);
        return;
      }
      
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
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for token retrieval on iOS simulator');
        return await AsyncStorage.getItem(this.TOKEN_KEY);
      }
      
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
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for refresh token on iOS simulator');
        await AsyncStorage.setItem(this.REFRESH_TOKEN_KEY, token);
        return;
      }
      
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
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for refresh token retrieval on iOS simulator');
        return await AsyncStorage.getItem(this.REFRESH_TOKEN_KEY);
      }
      
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
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for user data on iOS simulator');
        await AsyncStorage.setItem(this.USER_KEY, JSON.stringify(userData));
        return;
      }
      
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
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for user data retrieval on iOS simulator');
        const userData = await AsyncStorage.getItem(this.USER_KEY);
        return userData ? JSON.parse(userData) : null;
      }
      
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
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for token removal on iOS simulator');
        await AsyncStorage.removeItem(this.TOKEN_KEY);
        return;
      }
      
      await Keychain.resetInternetCredentials(this.TOKEN_KEY);
    } catch (error) {
      console.error('Error removing token:', error);
    }
  }

  static async removeRefreshToken(): Promise<void> {
    try {
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for refresh token removal on iOS simulator');
        await AsyncStorage.removeItem(this.REFRESH_TOKEN_KEY);
        return;
      }
      
      await Keychain.resetInternetCredentials(this.REFRESH_TOKEN_KEY);
    } catch (error) {
      console.error('Error removing refresh token:', error);
    }
  }

  static async removeUserData(): Promise<void> {
    try {
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Using AsyncStorage for user data removal on iOS simulator');
        await AsyncStorage.removeItem(this.USER_KEY);
        return;
      }
      
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
