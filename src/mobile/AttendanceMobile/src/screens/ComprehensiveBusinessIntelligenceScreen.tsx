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

interface BusinessIntelligenceData {
  totalReports: number;
  activeUsers: number;
  dataSourcesConnected: number;
  dashboardsCreated: number;
  queryPerformance: number;
  dataAccuracy: number;
  insightsGenerated: number;
}

const ComprehensiveBusinessIntelligenceScreen: React.FC = () => {
  const [biData, setBiData] = useState<BusinessIntelligenceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadBusinessIntelligenceData();
  }, []);

  const loadBusinessIntelligenceData = async () => {
    try {
      setLoading(true);
      const data = {
        totalReports: 450,
        activeUsers: 125,
        dataSourcesConnected: 15,
        dashboardsCreated: 85,
        queryPerformance: 2.3,
        dataAccuracy: 97.8,
        insightsGenerated: 1250,
      };
      setBiData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load business intelligence data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Business Intelligence Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Business Intelligence Dashboard</Text>
      
      {biData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Reports</Text>
              <Text style={styles.metricValue}>
                {biData.totalReports}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Users</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {biData.activeUsers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Sources Connected</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {biData.dataSourcesConnected}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Dashboards Created</Text>
              <Text style={styles.metricValue}>
                {biData.dashboardsCreated}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Query Performance</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {biData.queryPerformance.toFixed(1)}s
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Accuracy</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {biData.dataAccuracy.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Insights Generated</Text>
              <Text style={[styles.metricValue, { color: '#9C27B0' }]}>
                {biData.insightsGenerated.toLocaleString()}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Report Builder')}>
              <Text style={styles.actionButtonText}>Report Builder</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Dashboard Designer')}>
              <Text style={styles.actionButtonText}>Dashboard Designer</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Visualization')}>
              <Text style={styles.actionButtonText}>Data Visualization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Predictive Analytics')}>
              <Text style={styles.actionButtonText}>Predictive Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Mining')}>
              <Text style={styles.actionButtonText}>Data Mining</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Metrics')}>
              <Text style={styles.actionButtonText}>Performance Metrics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Trend Analysis')}>
              <Text style={styles.actionButtonText}>Trend Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Executive Reports')}>
              <Text style={styles.actionButtonText}>Executive Reports</Text>
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

export default ComprehensiveBusinessIntelligenceScreen;
