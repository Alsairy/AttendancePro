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

interface RoboticsData {
  totalRobots: number;
  activeRobots: number;
  automationTasks: number;
  efficiency: number;
  uptime: number;
  maintenanceScheduled: number;
  productivityGains: number;
}

const ComprehensiveRoboticsScreen: React.FC = () => {
  const [roboticsData, setRoboticsData] = useState<RoboticsData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadRoboticsData();
  }, []);

  const loadRoboticsData = async () => {
    try {
      setLoading(true);
      const data = {
        totalRobots: 45,
        activeRobots: 42,
        automationTasks: 185,
        efficiency: 94.7,
        uptime: 98.5,
        maintenanceScheduled: 8,
        productivityGains: 35.8,
      };
      setRoboticsData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load robotics data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Robotics Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Robotics Management</Text>
      
      {roboticsData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Robots</Text>
              <Text style={styles.metricValue}>
                {roboticsData.totalRobots}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Robots</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {roboticsData.activeRobots}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Automation Tasks</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {roboticsData.automationTasks}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {roboticsData.efficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Uptime</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {roboticsData.uptime.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Maintenance Scheduled</Text>
              <Text style={styles.metricValue}>
                {roboticsData.maintenanceScheduled}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Productivity Gains</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {roboticsData.productivityGains.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Robot Fleet Management')}>
              <Text style={styles.actionButtonText}>Robot Fleet Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Task Automation')}>
              <Text style={styles.actionButtonText}>Task Automation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Monitoring')}>
              <Text style={styles.actionButtonText}>Performance Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Predictive Maintenance')}>
              <Text style={styles.actionButtonText}>Predictive Maintenance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Safety Management')}>
              <Text style={styles.actionButtonText}>Safety Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Process Optimization')}>
              <Text style={styles.actionButtonText}>Process Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Quality Control')}>
              <Text style={styles.actionButtonText}>Quality Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Robotics Analytics')}>
              <Text style={styles.actionButtonText}>Robotics Analytics</Text>
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

export default ComprehensiveRoboticsScreen;
