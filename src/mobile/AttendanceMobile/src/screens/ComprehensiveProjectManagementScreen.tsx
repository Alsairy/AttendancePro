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

interface ProjectData {
  totalProjects: number;
  activeProjects: number;
  completedProjects: number;
  onTimeCompletion: number;
  budgetUtilization: number;
  resourceUtilization: number;
  teamProductivity: number;
}

const ComprehensiveProjectManagementScreen: React.FC = () => {
  const [projectData, setProjectData] = useState<ProjectData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadProjectData();
  }, []);

  const loadProjectData = async () => {
    try {
      setLoading(true);
      const data = {
        totalProjects: 85,
        activeProjects: 32,
        completedProjects: 53,
        onTimeCompletion: 87.5,
        budgetUtilization: 92.3,
        resourceUtilization: 89.1,
        teamProductivity: 94.7,
      };
      setProjectData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load project data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Project Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Project Management Dashboard</Text>
      
      {projectData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Projects</Text>
              <Text style={styles.metricValue}>
                {projectData.totalProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Projects</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {projectData.activeProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Completed Projects</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {projectData.completedProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>On-Time Completion</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {projectData.onTimeCompletion.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Budget Utilization</Text>
              <Text style={styles.metricValue}>
                {projectData.budgetUtilization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Resource Utilization</Text>
              <Text style={styles.metricValue}>
                {projectData.resourceUtilization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Team Productivity</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {projectData.teamProductivity.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Project Planning')}>
              <Text style={styles.actionButtonText}>Project Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Task Management')}>
              <Text style={styles.actionButtonText}>Task Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Resource Allocation')}>
              <Text style={styles.actionButtonText}>Resource Allocation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Timeline Tracking')}>
              <Text style={styles.actionButtonText}>Timeline Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Budget Management')}>
              <Text style={styles.actionButtonText}>Budget Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Management')}>
              <Text style={styles.actionButtonText}>Risk Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Team Collaboration')}>
              <Text style={styles.actionButtonText}>Team Collaboration</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Progress Reports')}>
              <Text style={styles.actionButtonText}>Progress Reports</Text>
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

export default ComprehensiveProjectManagementScreen;
