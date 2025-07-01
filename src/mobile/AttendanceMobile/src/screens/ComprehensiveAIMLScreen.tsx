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

interface AIMLData {
  totalModels: number;
  activeModels: number;
  modelAccuracy: number;
  trainingJobs: number;
  inferenceRequests: number;
  computeUtilization: number;
  dataProcessed: number;
}

const ComprehensiveAIMLScreen: React.FC = () => {
  const [aimlData, setAimlData] = useState<AIMLData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadAIMLData();
  }, []);

  const loadAIMLData = async () => {
    try {
      setLoading(true);
      const data = {
        totalModels: 45,
        activeModels: 38,
        modelAccuracy: 94.7,
        trainingJobs: 125,
        inferenceRequests: 15420000,
        computeUtilization: 78.5,
        dataProcessed: 2.8,
      };
      setAimlData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load AI/ML data');
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
        <Text style={styles.loadingText}>Loading AI/ML Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>AI/ML Management</Text>
      
      {aimlData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Models</Text>
              <Text style={styles.metricValue}>
                {aimlData.totalModels}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Models</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {aimlData.activeModels}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Model Accuracy</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {aimlData.modelAccuracy.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Training Jobs</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {aimlData.trainingJobs}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Inference Requests</Text>
              <Text style={styles.metricValue}>
                {formatNumber(aimlData.inferenceRequests)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Compute Utilization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {aimlData.computeUtilization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Processed</Text>
              <Text style={styles.metricValue}>
                {aimlData.dataProcessed.toFixed(1)} TB
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Model Management')}>
              <Text style={styles.actionButtonText}>Model Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Training Pipeline')}>
              <Text style={styles.actionButtonText}>Training Pipeline</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Preprocessing')}>
              <Text style={styles.actionButtonText}>Data Preprocessing</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Model Deployment')}>
              <Text style={styles.actionButtonText}>Model Deployment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Monitoring')}>
              <Text style={styles.actionButtonText}>Performance Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'AutoML')}>
              <Text style={styles.actionButtonText}>AutoML</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Feature Engineering')}>
              <Text style={styles.actionButtonText}>Feature Engineering</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Model Versioning')}>
              <Text style={styles.actionButtonText}>Model Versioning</Text>
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

export default ComprehensiveAIMLScreen;
