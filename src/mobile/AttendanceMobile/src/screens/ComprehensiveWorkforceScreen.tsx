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

interface WorkforceData {
  totalEmployees: number;
  activeEmployees: number;
  remoteEmployees: number;
  productivityScore: number;
  engagementLevel: number;
  skillsGapIndex: number;
  workforceUtilization: number;
}

const ComprehensiveWorkforceScreen: React.FC = () => {
  const [workforceData, setWorkforceData] = useState<WorkforceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadWorkforceData();
  }, []);

  const loadWorkforceData = async () => {
    try {
      setLoading(true);
      const data = {
        totalEmployees: 1250,
        activeEmployees: 1180,
        remoteEmployees: 420,
        productivityScore: 87.3,
        engagementLevel: 82.1,
        skillsGapIndex: 15.7,
        workforceUtilization: 91.4,
      };
      setWorkforceData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load workforce data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Workforce Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Workforce Analytics</Text>
      
      {workforceData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Employees</Text>
              <Text style={styles.metricValue}>
                {workforceData.totalEmployees.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Employees</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {workforceData.activeEmployees.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Remote Employees</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {workforceData.remoteEmployees.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Productivity Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {workforceData.productivityScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Engagement Level</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {workforceData.engagementLevel.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Skills Gap Index</Text>
              <Text style={[styles.metricValue, { color: workforceData.skillsGapIndex < 20 ? '#4CAF50' : '#FF9800' }]}>
                {workforceData.skillsGapIndex.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Workforce Utilization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {workforceData.workforceUtilization.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Workforce Planning')}>
              <Text style={styles.actionButtonText}>Workforce Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Skills Management')}>
              <Text style={styles.actionButtonText}>Skills Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Tracking')}>
              <Text style={styles.actionButtonText}>Performance Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Engagement Surveys')}>
              <Text style={styles.actionButtonText}>Engagement Surveys</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Capacity Planning')}>
              <Text style={styles.actionButtonText}>Capacity Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Remote Work Management')}>
              <Text style={styles.actionButtonText}>Remote Work Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Workforce Forecasting')}>
              <Text style={styles.actionButtonText}>Workforce Forecasting</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Productivity Analytics')}>
              <Text style={styles.actionButtonText}>Productivity Analytics</Text>
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

export default ComprehensiveWorkforceScreen;
