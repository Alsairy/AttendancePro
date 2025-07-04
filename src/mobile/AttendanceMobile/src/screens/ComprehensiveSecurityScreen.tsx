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

interface SecurityData {
  securityScore: number;
  threatLevel: string;
  vulnerabilities: number;
  incidentCount: number;
  complianceStatus: number;
  accessViolations: number;
  securityTraining: number;
}

const ComprehensiveSecurityScreen: React.FC = () => {
  const [securityData, setSecurityData] = useState<SecurityData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSecurityData();
  }, []);

  const loadSecurityData = async () => {
    try {
      setLoading(true);
      const data = {
        securityScore: 92.8,
        threatLevel: 'Low',
        vulnerabilities: 3,
        incidentCount: 2,
        complianceStatus: 96.5,
        accessViolations: 1,
        securityTraining: 94.2,
      };
      setSecurityData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load security data');
    } finally {
      setLoading(false);
    }
  };

  const getThreatColor = (level: string) => {
    switch (level.toLowerCase()) {
      case 'low': return '#4CAF50';
      case 'medium': return '#FF9800';
      case 'high': return '#F44336';
      default: return '#666';
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Security Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Security Management Dashboard</Text>
      
      {securityData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {securityData.securityScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Threat Level</Text>
              <Text style={[styles.metricValue, { color: getThreatColor(securityData.threatLevel) }]}>
                {securityData.threatLevel}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Vulnerabilities</Text>
              <Text style={[styles.metricValue, { color: securityData.vulnerabilities < 5 ? '#4CAF50' : '#FF9800' }]}>
                {securityData.vulnerabilities}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Incidents</Text>
              <Text style={[styles.metricValue, { color: securityData.incidentCount < 3 ? '#4CAF50' : '#FF9800' }]}>
                {securityData.incidentCount}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Compliance Status</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {securityData.complianceStatus.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Access Violations</Text>
              <Text style={[styles.metricValue, { color: securityData.accessViolations === 0 ? '#4CAF50' : '#FF9800' }]}>
                {securityData.accessViolations}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Security Training</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {securityData.securityTraining.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Threat Detection')}>
              <Text style={styles.actionButtonText}>Threat Detection</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vulnerability Scanning')}>
              <Text style={styles.actionButtonText}>Vulnerability Scanning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Access Control')}>
              <Text style={styles.actionButtonText}>Access Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Incident Response')}>
              <Text style={styles.actionButtonText}>Incident Response</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Monitoring')}>
              <Text style={styles.actionButtonText}>Security Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compliance Management')}>
              <Text style={styles.actionButtonText}>Compliance Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Training')}>
              <Text style={styles.actionButtonText}>Security Training</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Audit Logs')}>
              <Text style={styles.actionButtonText}>Audit Logs</Text>
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

export default ComprehensiveSecurityScreen;
