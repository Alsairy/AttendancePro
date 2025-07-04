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
import { HRService } from '../services/HRService';

interface HRData {
  totalEmployees: number;
  newHires: number;
  terminations: number;
  retentionRate: number;
  turnoverRate: number;
  averageEmployeeTenure: number;
  engagementScore: number;
  satisfactionScore: number;
}

const ComprehensiveHRScreen: React.FC = () => {
  const [hrData, setHrData] = useState<HRData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadHRData();
  }, []);

  const loadHRData = async () => {
    try {
      setLoading(true);
      const data = await HRService.getHRDashboard();
      setHrData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load HR data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading HR Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>HR Management Dashboard</Text>
      
      {hrData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Employees</Text>
              <Text style={styles.metricValue}>
                {hrData.totalEmployees}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>New Hires</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {hrData.newHires}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Terminations</Text>
              <Text style={[styles.metricValue, { color: '#F44336' }]}>
                {hrData.terminations}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Retention Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {hrData.retentionRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Turnover Rate</Text>
              <Text style={styles.metricValue}>
                {hrData.turnoverRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Avg Employee Tenure</Text>
              <Text style={styles.metricValue}>
                {hrData.averageEmployeeTenure.toFixed(1)} years
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Engagement Score</Text>
              <Text style={styles.metricValue}>
                {hrData.engagementScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Satisfaction Score</Text>
              <Text style={styles.metricValue}>
                {hrData.satisfactionScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Employee Lifecycle Management')}>
              <Text style={styles.actionButtonText}>Employee Lifecycle</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Talent Acquisition')}>
              <Text style={styles.actionButtonText}>Talent Acquisition</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Management')}>
              <Text style={styles.actionButtonText}>Performance Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compensation Analysis')}>
              <Text style={styles.actionButtonText}>Compensation Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Learning & Development')}>
              <Text style={styles.actionButtonText}>Learning & Development</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Diversity & Inclusion')}>
              <Text style={styles.actionButtonText}>Diversity & Inclusion</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Workforce Analytics')}>
              <Text style={styles.actionButtonText}>Workforce Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'HR Compliance')}>
              <Text style={styles.actionButtonText}>HR Compliance</Text>
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

export default ComprehensiveHRScreen;
