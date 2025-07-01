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

interface CybersecurityData {
  securityIncidents: number;
  threatsDetected: number;
  vulnerabilities: number;
  securityScore: number;
  complianceLevel: number;
  securityTraining: number;
  patchLevel: number;
}

const ComprehensiveCybersecurityScreen: React.FC = () => {
  const [cybersecurityData, setCybersecurityData] = useState<CybersecurityData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadCybersecurityData();
  }, []);

  const loadCybersecurityData = async () => {
    try {
      setLoading(true);
      const data = {
        securityIncidents: 3,
        threatsDetected: 125,
        vulnerabilities: 8,
        securityScore: 96.7,
        complianceLevel: 98.2,
        securityTraining: 89.5,
        patchLevel: 94.8,
      };
      setCybersecurityData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load cybersecurity data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Cybersecurity Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Cybersecurity Management</Text>
      
      {cybersecurityData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Incidents</Text>
              <Text style={[styles.metricValue, { color: cybersecurityData.securityIncidents < 5 ? '#4CAF50' : '#FF9800' }]}>
                {cybersecurityData.securityIncidents}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Threats Detected</Text>
              <Text style={[styles.metricValue, { color: '#FF9800' }]}>
                {cybersecurityData.threatsDetected}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Vulnerabilities</Text>
              <Text style={[styles.metricValue, { color: cybersecurityData.vulnerabilities < 10 ? '#4CAF50' : '#FF9800' }]}>
                {cybersecurityData.vulnerabilities}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cybersecurityData.securityScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Compliance Level</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cybersecurityData.complianceLevel.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Training</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cybersecurityData.securityTraining.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Patch Level</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {cybersecurityData.patchLevel.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Threat Detection')}>
              <Text style={styles.actionButtonText}>Threat Detection</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vulnerability Assessment')}>
              <Text style={styles.actionButtonText}>Vulnerability Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Incident Response')}>
              <Text style={styles.actionButtonText}>Incident Response</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Monitoring')}>
              <Text style={styles.actionButtonText}>Security Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Access Control')}>
              <Text style={styles.actionButtonText}>Access Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Training')}>
              <Text style={styles.actionButtonText}>Security Training</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compliance Management')}>
              <Text style={styles.actionButtonText}>Compliance Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Reports')}>
              <Text style={styles.actionButtonText}>Security Reports</Text>
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

export default ComprehensiveCybersecurityScreen;
