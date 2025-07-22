import React, { useEffect, useState } from 'react';
import {
  SafeAreaView,
  StatusBar,
  StyleSheet,
  useColorScheme,
  Alert,
  Platform,
  PermissionsAndroid,
} from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Provider as PaperProvider } from 'react-native-paper';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { QueryClient, QueryClientProvider } from 'react-query';
import Geolocation from '@react-native-community/geolocation';
import { request, PERMISSIONS, RESULTS } from 'react-native-permissions';
import Icon from 'react-native-vector-icons/MaterialIcons';

// Screens
import LoginScreen from './src/screens/LoginScreen';
import DashboardScreen from './src/screens/DashboardScreen';
import AttendanceScreen from './src/screens/AttendanceScreen';
import FaceEnrollmentScreen from './src/screens/FaceEnrollmentScreen';
import LeaveRequestScreen from './src/screens/LeaveRequestScreen';
import ProfileScreen from './src/screens/ProfileScreen';
import SettingsScreen from './src/screens/SettingsScreen';
import OfflineScreen from './src/screens/OfflineScreen';
import LoadingScreen from './src/components/LoadingScreen';

// Services
import { AuthService } from './src/services/AuthService';
import { LocationService } from './src/services/LocationService';
import { BiometricService } from './src/services/BiometricService';
import { OfflineService } from './src/services/OfflineService';
import { NotificationService } from './src/services/NotificationService';
import { PermissionService } from './src/services/PermissionService';

// Store
import { useAuthStore } from './src/store/authStore';
import { useLocationStore } from './src/store/locationStore';
import { useOfflineStore } from './src/store/offlineStore';

// Types
import { User, LoginCredentials } from './src/types/User';
import { AppTheme } from './src/types/Theme';

// Utils
import { theme } from './src/utils/theme';
import { Colors } from './src/utils/colors';
import SecureTokenStorage from './src/utils/SecureTokenStorage';

const Stack = createStackNavigator();
const Tab = createBottomTabNavigator();
const queryClient = new QueryClient();

// Main Tab Navigator
function MainTabNavigator() {
  return (
    <Tab.Navigator
      screenOptions={{
        tabBarActiveTintColor: Colors.primary,
        tabBarInactiveTintColor: Colors.gray,
        tabBarStyle: {
          backgroundColor: Colors.white,
          borderTopWidth: 1,
          borderTopColor: Colors.lightGray,
          paddingBottom: Platform.OS === 'ios' ? 20 : 5,
          height: Platform.OS === 'ios' ? 85 : 60,
        },
        headerShown: false,
      }}
    >
      <Tab.Screen 
        name="Dashboard" 
        component={DashboardScreen}
        options={{
          tabBarIcon: ({ color, size }) => (
            <Icon name="dashboard" size={size} color={color} />
          ),
        }}
      />
      <Tab.Screen 
        name="Attendance" 
        component={AttendanceScreen}
        options={{
          tabBarIcon: ({ color, size }) => (
            <Icon name="access-time" size={size} color={color} />
          ),
        }}
      />
      <Tab.Screen 
        name="Leave" 
        component={LeaveRequestScreen}
        options={{
          tabBarIcon: ({ color, size }) => (
            <Icon name="event-note" size={size} color={color} />
          ),
        }}
      />
      <Tab.Screen 
        name="Profile" 
        component={ProfileScreen}
        options={{
          tabBarIcon: ({ color, size }) => (
            <Icon name="person" size={size} color={color} />
          ),
        }}
      />
    </Tab.Navigator>
  );
}

// Main App Component
function App(): JSX.Element {
  const isDarkMode = useColorScheme() === 'dark';
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isOffline, setIsOffline] = useState(false);
  
  const { user, setUser, clearUser } = useAuthStore();
  const { setLocation } = useLocationStore();
  const { isOfflineMode, syncPendingData } = useOfflineStore();

  useEffect(() => {
    initializeApp();
  }, []);

  const initializeApp = async () => {
    try {
      // Initialize services with individual error handling
      const results = await Promise.allSettled([
        requestPermissions(),
        checkAuthStatus(),
        initializeLocation(),
        initializeBiometrics(),
        initializeNotifications(),
        OfflineService.initialize(),
      ]);

      results.forEach((result, index) => {
        const serviceNames = ['permissions', 'auth', 'location', 'biometrics', 'notifications', 'offline'];
        if (result.status === 'rejected') {
          console.warn(`${serviceNames[index]} initialization failed:`, result.reason);
        }
      });

      // Sync offline data if connected (only if offline service initialized successfully)
      const offlineResult = results[5];
      if (offlineResult.status === 'fulfilled' && !isOfflineMode) {
        try {
          await syncPendingData();
        } catch (error) {
          console.warn('Offline sync failed:', error);
        }
      }
    } catch (error) {
      console.error('App initialization error:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const requestPermissions = async () => {
    try {
      const permissions = await PermissionService.requestAllPermissions();
      console.log('Permissions granted:', permissions);
    } catch (error) {
      console.error('Permission request error:', error);
    }
  };

  const checkAuthStatus = async () => {
    try {
      const token = await SecureTokenStorage.getToken();
      if (token) {
        const userData = await AuthService.getUser();
        if (userData) {
          setUser(userData);
          setIsAuthenticated(true);
        } else {
          await SecureTokenStorage.removeToken();
        }
      }
    } catch (error) {
      console.error('Auth check error:', error);
      await SecureTokenStorage.removeToken();
    }
  };

  const initializeLocation = async () => {
    try {
      await LocationService.initialize();
      const position = await LocationService.getCurrentPosition();
      setLocation(position);
    } catch (error) {
      console.error('Location initialization error:', error);
    }
  };

  const initializeBiometrics = async () => {
    try {
      if (__DEV__ && Platform.OS === 'ios') {
        console.log('Skipping biometric initialization on iOS simulator');
        return;
      }
      
      const isAvailable = await BiometricService.isAvailable();
      if (isAvailable) {
        await BiometricService.initialize();
      }
    } catch (error) {
      console.error('Biometric initialization error:', error);
    }
  };

  const initializeNotifications = async () => {
    try {
      await NotificationService.initialize();
      await NotificationService.requestPermissions();
    } catch (error) {
      console.error('Notification initialization error:', error);
    }
  };

  const handleLogin = async (credentials: LoginCredentials) => {
    try {
      const response = await AuthService.login(credentials);
      await SecureTokenStorage.setToken(response.token);
      if (response.refreshToken) {
        await SecureTokenStorage.setRefreshToken(response.refreshToken);
      }
      setUser(response.user);
      setIsAuthenticated(true);
    } catch (error) {
      throw error;
    }
  };

  const handleLogout = async () => {
    try {
      await AuthService.logout();
      await AsyncStorage.removeItem('authToken');
      clearUser();
      setIsAuthenticated(false);
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  if (isLoading) {
    return <LoadingScreen />;
  }

  if (isOfflineMode && !isAuthenticated) {
    return <OfflineScreen />;
  }

  return (
    <QueryClientProvider client={queryClient}>
      <PaperProvider theme={theme}>
        <NavigationContainer>
          <SafeAreaView style={styles.container}>
            <StatusBar
              barStyle={isDarkMode ? 'light-content' : 'dark-content'}
              backgroundColor={isDarkMode ? Colors.dark.background : Colors.white}
            />
            
            <Stack.Navigator screenOptions={{ headerShown: false }}>
              {!isAuthenticated ? (
                <Stack.Screen name="Login">
                  {props => <LoginScreen {...props} onLogin={handleLogin} />}
                </Stack.Screen>
              ) : (
                <>
                  <Stack.Screen name="Main" component={MainTabNavigator} />
                  <Stack.Screen 
                    name="FaceEnrollment" 
                    component={FaceEnrollmentScreen}
                    options={{ presentation: 'modal' }}
                  />
                  <Stack.Screen 
                    name="Settings" 
                    component={SettingsScreen}
                    options={{ presentation: 'modal' }}
                  />
                </>
              )}
            </Stack.Navigator>
          </SafeAreaView>
        </NavigationContainer>
      </PaperProvider>
    </QueryClientProvider>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
});

export default App;

