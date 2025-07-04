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

interface AnalyticsData {
  totalDataPoints: number;
  realTimeStreams: number;
  dataAccuracy: number;
  processingSpeed: number;
  storageUtilization: number;
  predictiveModels: number;
  insightsGenerated: number;
}

const ComprehensiveDataAnalyticsScreen: React.FC = () => {
  const [analyticsData, setAnalyticsData] = useState<AnalyticsData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadAnalyticsData();
  }, []);

  const loadAnalyticsData = async () => {
    try {
      setLoading(true);
      const data = {
        totalDataPoints: 15420000,
        realTimeStreams: 45,
        dataAccuracy: 97.8,
        processingSpeed: 2.3,
        storageUtilization: 73.5,
        predictiveModels: 12,
        insightsGenerated: 1250,
      };
      setAnalyticsData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load analytics data');
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
        <Text style={styles.loadingText}>Loading Analytics Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Data Analytics Dashboard</Text>
      
      {analyticsData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Data Points</Text>
              <Text style={styles.metricValue}>
                {formatNumber(analyticsData.totalDataPoints)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Real-Time Streams</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {analyticsData.realTimeStreams}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Accuracy</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {analyticsData.dataAccuracy.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Processing Speed</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {analyticsData.processingSpeed.toFixed(1)}s
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Storage Utilization</Text>
              <Text style={styles.metricValue}>
                {analyticsData.storageUtilization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Predictive Models</Text>
              <Text style={[styles.metricValue, { color: '#9C27B0' }]}>
                {analyticsData.predictiveModels}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Insights Generated</Text>
              <Text style={[styles.metricValue, { color: '#FF9800' }]}>
                {formatNumber(analyticsData.insightsGenerated)}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Visualization')}>
              <Text style={styles.actionButtonText}>Data Visualization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Predictive Analytics')}>
              <Text style={styles.actionButtonText}>Predictive Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Real-Time Monitoring')}>
              <Text style={styles.actionButtonText}>Real-Time Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Mining')}>
              <Text style={styles.actionButtonText}>Data Mining</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Machine Learning')}>
              <Text style={styles.actionButtonText}>Machine Learning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Statistical Analysis')}>
              <Text style={styles.actionButtonText}>Statistical Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Business Intelligence')}>
              <Text style={styles.actionButtonText}>Business Intelligence</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Governance')}>
              <Text style={styles.actionButtonText}>Data Governance</Text>
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

export default ComprehensiveDataAnalyticsScreen;
