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

interface CloudComputingData {
  totalInstances: number;
  activeInstances: number;
  cloudUtilization: number;
  costOptimization: number;
  uptime: number;
  scalingEvents: number;
  dataTransfer: number;
}

const ComprehensiveCloudComputingScreen: React.FC = () => {
  const [cloudData, setCloudData] = useState<CloudComputingData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadCloudData();
  }, []);

  const loadCloudData = async () => {
    try {
      setLoading(true);
      const data = {
        totalInstances: 185,
        activeInstances: 165,
        cloudUtilization: 78.5,
        costOptimization: 23.7,
        uptime: 99.9,
        scalingEvents: 125,
        dataTransfer: 15.8,
      };
      setCloudData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load cloud computing data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Cloud Computing Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Cloud Computing Management</Text>
      
      {cloudData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Instances</Text>
              <Text style={styles.metricValue}>
                {cloudData.totalInstances}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Instances</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cloudData.activeInstances}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Cloud Utilization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cloudData.cloudUtilization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Cost Optimization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cloudData.costOptimization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Uptime</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cloudData.uptime.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Scaling Events</Text>
              <Text style={styles.metricValue}>
                {cloudData.scalingEvents}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Transfer</Text>
              <Text style={styles.metricValue}>
                {cloudData.dataTransfer.toFixed(1)} TB
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Instance Management')}>
              <Text style={styles.actionButtonText}>Instance Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Auto Scaling')}>
              <Text style={styles.actionButtonText}>Auto Scaling</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Load Balancing')}>
              <Text style={styles.actionButtonText}>Load Balancing</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Cost Management')}>
              <Text style={styles.actionButtonText}>Cost Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Groups')}>
              <Text style={styles.actionButtonText}>Security Groups</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Backup Management')}>
              <Text style={styles.actionButtonText}>Backup Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Monitoring')}>
              <Text style={styles.actionButtonText}>Performance Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Cloud Reports')}>
              <Text style={styles.actionButtonText}>Cloud Reports</Text>
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

export default ComprehensiveCloudComputingScreen;
