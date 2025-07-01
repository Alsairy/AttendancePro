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

interface OperationsData {
  totalOperations: number;
  activeProcesses: number;
  efficiency: number;
  uptime: number;
  throughput: number;
  errorRate: number;
  resourceUtilization: number;
}

const ComprehensiveOperationsScreen: React.FC = () => {
  const [operationsData, setOperationsData] = useState<OperationsData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadOperationsData();
  }, []);

  const loadOperationsData = async () => {
    try {
      setLoading(true);
      const data = {
        totalOperations: 15420,
        activeProcesses: 125,
        efficiency: 94.7,
        uptime: 99.2,
        throughput: 1250,
        errorRate: 0.8,
        resourceUtilization: 87.3,
      };
      setOperationsData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load operations data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Operations Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Operations Management</Text>
      
      {operationsData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Operations</Text>
              <Text style={styles.metricValue}>
                {operationsData.totalOperations.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Processes</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {operationsData.activeProcesses}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {operationsData.efficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>System Uptime</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {operationsData.uptime.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Throughput</Text>
              <Text style={styles.metricValue}>
                {operationsData.throughput}/hr
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Error Rate</Text>
              <Text style={[styles.metricValue, { color: operationsData.errorRate < 1 ? '#4CAF50' : '#FF9800' }]}>
                {operationsData.errorRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Resource Utilization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {operationsData.resourceUtilization.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Process Management')}>
              <Text style={styles.actionButtonText}>Process Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Resource Planning')}>
              <Text style={styles.actionButtonText}>Resource Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Monitoring')}>
              <Text style={styles.actionButtonText}>Performance Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Capacity Management')}>
              <Text style={styles.actionButtonText}>Capacity Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Workflow Optimization')}>
              <Text style={styles.actionButtonText}>Workflow Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Incident Management')}>
              <Text style={styles.actionButtonText}>Incident Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Service Level Management')}>
              <Text style={styles.actionButtonText}>Service Level Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Operations Reports')}>
              <Text style={styles.actionButtonText}>Operations Reports</Text>
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

export default ComprehensiveOperationsScreen;
