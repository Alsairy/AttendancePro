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

interface TrainingData {
  totalPrograms: number;
  activePrograms: number;
  enrolledEmployees: number;
  completionRate: number;
  trainingHours: number;
  certifications: number;
  trainingCosts: number;
}

const ComprehensiveTrainingScreen: React.FC = () => {
  const [trainingData, setTrainingData] = useState<TrainingData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadTrainingData();
  }, []);

  const loadTrainingData = async () => {
    try {
      setLoading(true);
      const data = {
        totalPrograms: 85,
        activePrograms: 65,
        enrolledEmployees: 420,
        completionRate: 87.3,
        trainingHours: 2450,
        certifications: 125,
        trainingCosts: 185000,
      };
      setTrainingData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load training data');
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
        <Text style={styles.loadingText}>Loading Training Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Training & Development</Text>
      
      {trainingData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Programs</Text>
              <Text style={styles.metricValue}>
                {trainingData.totalPrograms}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Programs</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {trainingData.activePrograms}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Enrolled Employees</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {trainingData.enrolledEmployees}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Completion Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {trainingData.completionRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Training Hours</Text>
              <Text style={styles.metricValue}>
                {trainingData.trainingHours.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Certifications Earned</Text>
              <Text style={[styles.metricValue, { color: '#9C27B0' }]}>
                {trainingData.certifications}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Training Costs</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(trainingData.trainingCosts)}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Course Catalog')}>
              <Text style={styles.actionButtonText}>Course Catalog</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Learning Paths')}>
              <Text style={styles.actionButtonText}>Learning Paths</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Skills Assessment')}>
              <Text style={styles.actionButtonText}>Skills Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Certification Tracking')}>
              <Text style={styles.actionButtonText}>Certification Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Training Calendar')}>
              <Text style={styles.actionButtonText}>Training Calendar</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Analytics')}>
              <Text style={styles.actionButtonText}>Performance Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Training Reports')}>
              <Text style={styles.actionButtonText}>Training Reports</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Learning Management')}>
              <Text style={styles.actionButtonText}>Learning Management</Text>
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

export default ComprehensiveTrainingScreen;
