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

interface MaintenanceData {
  totalAssets: number;
  scheduledMaintenance: number;
  emergencyRepairs: number;
  maintenanceEfficiency: number;
  assetUptime: number;
  maintenanceCosts: number;
  preventiveRatio: number;
}

const ComprehensiveMaintenanceScreen: React.FC = () => {
  const [maintenanceData, setMaintenanceData] = useState<MaintenanceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadMaintenanceData();
  }, []);

  const loadMaintenanceData = async () => {
    try {
      setLoading(true);
      const data = {
        totalAssets: 850,
        scheduledMaintenance: 45,
        emergencyRepairs: 8,
        maintenanceEfficiency: 91.5,
        assetUptime: 96.8,
        maintenanceCosts: 125000,
        preventiveRatio: 85.2,
      };
      setMaintenanceData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load maintenance data');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Maintenance Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Maintenance Management</Text>
      
      {maintenanceData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Assets</Text>
              <Text style={styles.metricValue}>
                {maintenanceData.totalAssets}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Scheduled Maintenance</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {maintenanceData.scheduledMaintenance}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Emergency Repairs</Text>
              <Text style={[styles.metricValue, { color: maintenanceData.emergencyRepairs < 10 ? '#4CAF50' : '#FF9800' }]}>
                {maintenanceData.emergencyRepairs}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Maintenance Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {maintenanceData.maintenanceEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Asset Uptime</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {maintenanceData.assetUptime.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Maintenance Costs</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(maintenanceData.maintenanceCosts)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Preventive Ratio</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {maintenanceData.preventiveRatio.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Management')}>
              <Text style={styles.actionButtonText}>Asset Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Work Orders')}>
              <Text style={styles.actionButtonText}>Work Orders</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Preventive Maintenance')}>
              <Text style={styles.actionButtonText}>Preventive Maintenance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Equipment Tracking')}>
              <Text style={styles.actionButtonText}>Equipment Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Spare Parts Management')}>
              <Text style={styles.actionButtonText}>Spare Parts Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Maintenance Scheduling')}>
              <Text style={styles.actionButtonText}>Maintenance Scheduling</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Analytics')}>
              <Text style={styles.actionButtonText}>Performance Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Cost Analysis')}>
              <Text style={styles.actionButtonText}>Cost Analysis</Text>
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

export default ComprehensiveMaintenanceScreen;
