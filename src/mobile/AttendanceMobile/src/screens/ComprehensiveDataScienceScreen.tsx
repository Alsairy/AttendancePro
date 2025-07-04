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

interface DataScienceData {
  totalDatasets: number;
  activeModels: number;
  dataProcessed: number;
  modelAccuracy: number;
  predictiveInsights: number;
  dataQuality: number;
  analysisJobs: number;
}

const ComprehensiveDataScienceScreen: React.FC = () => {
  const [dataScienceData, setDataScienceData] = useState<DataScienceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDataScienceData();
  }, []);

  const loadDataScienceData = async () => {
    try {
      setLoading(true);
      const data = {
        totalDatasets: 450,
        activeModels: 85,
        dataProcessed: 25.8,
        modelAccuracy: 94.7,
        predictiveInsights: 1250,
        dataQuality: 96.3,
        analysisJobs: 185,
      };
      setDataScienceData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load data science data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Data Science Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Data Science Management</Text>
      
      {dataScienceData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Datasets</Text>
              <Text style={styles.metricValue}>
                {dataScienceData.totalDatasets}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Models</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {dataScienceData.activeModels}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Processed</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {dataScienceData.dataProcessed.toFixed(1)} TB
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Model Accuracy</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {dataScienceData.modelAccuracy.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Predictive Insights</Text>
              <Text style={styles.metricValue}>
                {dataScienceData.predictiveInsights.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Quality</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {dataScienceData.dataQuality.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Analysis Jobs</Text>
              <Text style={styles.metricValue}>
                {dataScienceData.analysisJobs}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Exploration')}>
              <Text style={styles.actionButtonText}>Data Exploration</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Model Development')}>
              <Text style={styles.actionButtonText}>Model Development</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Statistical Analysis')}>
              <Text style={styles.actionButtonText}>Statistical Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Visualization')}>
              <Text style={styles.actionButtonText}>Data Visualization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Predictive Modeling')}>
              <Text style={styles.actionButtonText}>Predictive Modeling</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Mining')}>
              <Text style={styles.actionButtonText}>Data Mining</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Machine Learning')}>
              <Text style={styles.actionButtonText}>Machine Learning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Reports')}>
              <Text style={styles.actionButtonText}>Data Reports</Text>
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

export default ComprehensiveDataScienceScreen;
