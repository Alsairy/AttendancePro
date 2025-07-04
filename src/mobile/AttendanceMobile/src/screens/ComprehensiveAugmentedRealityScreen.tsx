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

interface AugmentedRealityData {
  arApplications: number;
  activeUsers: number;
  arSessions: number;
  userEngagement: number;
  accuracyScore: number;
  contentCreated: number;
  deviceCompatibility: number;
}

const ComprehensiveAugmentedRealityScreen: React.FC = () => {
  const [arData, setArData] = useState<AugmentedRealityData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadArData();
  }, []);

  const loadArData = async () => {
    try {
      setLoading(true);
      const data = {
        arApplications: 35,
        activeUsers: 285,
        arSessions: 1850,
        userEngagement: 89.7,
        accuracyScore: 94.2,
        contentCreated: 125,
        deviceCompatibility: 92.5,
      };
      setArData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load AR data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Augmented Reality Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Augmented Reality Management</Text>
      
      {arData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>AR Applications</Text>
              <Text style={styles.metricValue}>
                {arData.arApplications}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Users</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {arData.activeUsers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>AR Sessions</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {arData.arSessions.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>User Engagement</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {arData.userEngagement.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Accuracy Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {arData.accuracyScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Content Created</Text>
              <Text style={styles.metricValue}>
                {arData.contentCreated}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Device Compatibility</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {arData.deviceCompatibility.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'AR Content Creation')}>
              <Text style={styles.actionButtonText}>AR Content Creation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Object Recognition')}>
              <Text style={styles.actionButtonText}>Object Recognition</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Spatial Mapping')}>
              <Text style={styles.actionButtonText}>Spatial Mapping</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'User Interface')}>
              <Text style={styles.actionButtonText}>User Interface</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Optimization')}>
              <Text style={styles.actionButtonText}>Performance Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Device Management')}>
              <Text style={styles.actionButtonText}>Device Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Analytics Dashboard')}>
              <Text style={styles.actionButtonText}>Analytics Dashboard</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'AR Reports')}>
              <Text style={styles.actionButtonText}>AR Reports</Text>
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

export default ComprehensiveAugmentedRealityScreen;
