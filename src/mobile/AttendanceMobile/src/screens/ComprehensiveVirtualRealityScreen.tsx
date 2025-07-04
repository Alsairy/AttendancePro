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

interface VirtualRealityData {
  vrApplications: number;
  activeUsers: number;
  trainingModules: number;
  immersionScore: number;
  userEngagement: number;
  vrSessions: number;
  contentLibrary: number;
}

const ComprehensiveVirtualRealityScreen: React.FC = () => {
  const [vrData, setVrData] = useState<VirtualRealityData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadVrData();
  }, []);

  const loadVrData = async () => {
    try {
      setLoading(true);
      const data = {
        vrApplications: 25,
        activeUsers: 185,
        trainingModules: 45,
        immersionScore: 92.8,
        userEngagement: 87.5,
        vrSessions: 1250,
        contentLibrary: 125,
      };
      setVrData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load VR data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Virtual Reality Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Virtual Reality Management</Text>
      
      {vrData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>VR Applications</Text>
              <Text style={styles.metricValue}>
                {vrData.vrApplications}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Users</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vrData.activeUsers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Training Modules</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {vrData.trainingModules}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Immersion Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vrData.immersionScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>User Engagement</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vrData.userEngagement.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>VR Sessions</Text>
              <Text style={styles.metricValue}>
                {vrData.vrSessions.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Content Library</Text>
              <Text style={styles.metricValue}>
                {vrData.contentLibrary}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'VR Training')}>
              <Text style={styles.actionButtonText}>VR Training</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Content Management')}>
              <Text style={styles.actionButtonText}>Content Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'User Experience')}>
              <Text style={styles.actionButtonText}>User Experience</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Analytics')}>
              <Text style={styles.actionButtonText}>Performance Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Simulation Management')}>
              <Text style={styles.actionButtonText}>Simulation Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Hardware Management')}>
              <Text style={styles.actionButtonText}>Hardware Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Collaboration Tools')}>
              <Text style={styles.actionButtonText}>Collaboration Tools</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'VR Reports')}>
              <Text style={styles.actionButtonText}>VR Reports</Text>
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

export default ComprehensiveVirtualRealityScreen;
