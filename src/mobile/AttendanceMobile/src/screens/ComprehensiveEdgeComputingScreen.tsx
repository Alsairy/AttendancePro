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

interface EdgeComputingData {
  edgeNodes: number;
  activeNodes: number;
  edgeApplications: number;
  latency: number;
  bandwidth: number;
  dataProcessed: number;
  nodeHealth: number;
}

const ComprehensiveEdgeComputingScreen: React.FC = () => {
  const [edgeData, setEdgeData] = useState<EdgeComputingData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadEdgeData();
  }, []);

  const loadEdgeData = async () => {
    try {
      setLoading(true);
      const data = {
        edgeNodes: 125,
        activeNodes: 118,
        edgeApplications: 45,
        latency: 2.5,
        bandwidth: 850.5,
        dataProcessed: 12.8,
        nodeHealth: 96.7,
      };
      setEdgeData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load edge computing data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Edge Computing Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Edge Computing Management</Text>
      
      {edgeData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Edge Nodes</Text>
              <Text style={styles.metricValue}>
                {edgeData.edgeNodes}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Nodes</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {edgeData.activeNodes}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Edge Applications</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {edgeData.edgeApplications}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Latency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {edgeData.latency.toFixed(1)} ms
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Bandwidth</Text>
              <Text style={styles.metricValue}>
                {edgeData.bandwidth.toFixed(1)} Mbps
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Processed</Text>
              <Text style={styles.metricValue}>
                {edgeData.dataProcessed.toFixed(1)} TB
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Node Health</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {edgeData.nodeHealth.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Node Management')}>
              <Text style={styles.actionButtonText}>Node Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Application Deployment')}>
              <Text style={styles.actionButtonText}>Application Deployment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Monitoring')}>
              <Text style={styles.actionButtonText}>Performance Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Network Optimization')}>
              <Text style={styles.actionButtonText}>Network Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Synchronization')}>
              <Text style={styles.actionButtonText}>Data Synchronization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Management')}>
              <Text style={styles.actionButtonText}>Security Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Resource Allocation')}>
              <Text style={styles.actionButtonText}>Resource Allocation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Edge Analytics')}>
              <Text style={styles.actionButtonText}>Edge Analytics</Text>
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

export default ComprehensiveEdgeComputingScreen;
