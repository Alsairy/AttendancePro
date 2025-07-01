import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
} from 'react-native';

interface IoTData {
  connectedDevices: number;
  activeDevices: number;
  dataPoints: number;
  networkUptime: number;
  deviceHealth: number;
  energyEfficiency: number;
  securityScore: number;
}

const ComprehensiveIoTScreen: React.FC = () => {
  const [iotData, setIoTData] = useState<IoTData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadIoTData();
  }, []);

  const loadIoTData = async () => {
    try {
      setLoading(true);
      const data = {
        connectedDevices: 2850,
        activeDevices: 2650,
        dataPoints: 15420000,
        networkUptime: 99.7,
        deviceHealth: 94.2,
        energyEfficiency: 87.5,
        securityScore: 96.3,
      };
      setIoTData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load IoT data');
    } finally {
      setLoading(false);
    }
  };

  const formatNumber = (num: number) => {
    if (num >= 1000000) {
      return (num / 1000000).toFixed(1) + 'M';
    }
    if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'K';
    }
    return num.toString();
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading IoT Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>IoT Management</Text>
      
      {iotData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Connected Devices</Text>
              <Text style={styles.metricValue}>
                {formatNumber(iotData.connectedDevices)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Devices</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {formatNumber(iotData.activeDevices)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Points</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {formatNumber(iotData.dataPoints)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Network Uptime</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {iotData.networkUptime.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Device Health</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {iotData.deviceHealth.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Energy Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {iotData.energyEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {iotData.securityScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Device Management')}>
              <Text style={styles.actionButtonText}>Device Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sensor Monitoring')}>
              <Text style={styles.actionButtonText}>Sensor Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Analytics')}>
              <Text style={styles.actionButtonText}>Data Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Network Management')}>
              <Text style={styles.actionButtonText}>Network Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Management')}>
              <Text style={styles.actionButtonText}>Security Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Energy Optimization')}>
              <Text style={styles.actionButtonText}>Energy Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Predictive Maintenance')}>
              <Text style={styles.actionButtonText}>Predictive Maintenance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'IoT Reports')}>
              <Text style={styles.actionButtonText}>IoT Reports</Text>
            </TouchableOpacity>
          </View>
        </>
      )}
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
    padding: 16,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f5f5f5',
  },
  loadingText: {
    marginTop: 16,
    fontSize: 16,
    color: '#666',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 20,
    textAlign: 'center',
  },
  metricsContainer: {
    marginBottom: 24,
  },
  metricCard: {
    backgroundColor: '#fff',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  metricLabel: {
    fontSize: 14,
    color: '#666',
    marginBottom: 4,
  },
  metricValue: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
  },
  actionsContainer: {
    gap: 12,
  },
  actionButton: {
    backgroundColor: '#007AFF',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
  },
  actionButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});

export default ComprehensiveIoTScreen;
