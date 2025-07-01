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

interface GovernanceData {
  governanceScore: number;
  policyCompliance: number;
  boardMeetings: number;
  auditFindings: number;
  riskManagement: number;
  ethicsScore: number;
  transparencyIndex: number;
}

const ComprehensiveGovernanceScreen: React.FC = () => {
  const [governanceData, setGovernanceData] = useState<GovernanceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadGovernanceData();
  }, []);

  const loadGovernanceData = async () => {
    try {
      setLoading(true);
      const data = {
        governanceScore: 94.2,
        policyCompliance: 96.8,
        boardMeetings: 12,
        auditFindings: 3,
        riskManagement: 89.5,
        ethicsScore: 92.7,
        transparencyIndex: 87.3,
      };
      setGovernanceData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load governance data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Governance Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Corporate Governance</Text>
      
      {governanceData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Governance Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {governanceData.governanceScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Policy Compliance</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {governanceData.policyCompliance.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Board Meetings</Text>
              <Text style={styles.metricValue}>
                {governanceData.boardMeetings}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Audit Findings</Text>
              <Text style={[styles.metricValue, { color: governanceData.auditFindings < 5 ? '#4CAF50' : '#FF9800' }]}>
                {governanceData.auditFindings}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Risk Management</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {governanceData.riskManagement.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Ethics Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {governanceData.ethicsScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Transparency Index</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {governanceData.transparencyIndex.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Board Management')}>
              <Text style={styles.actionButtonText}>Board Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Policy Management')}>
              <Text style={styles.actionButtonText}>Policy Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Oversight')}>
              <Text style={styles.actionButtonText}>Risk Oversight</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Ethics Management')}>
              <Text style={styles.actionButtonText}>Ethics Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Audit Management')}>
              <Text style={styles.actionButtonText}>Audit Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Stakeholder Relations')}>
              <Text style={styles.actionButtonText}>Stakeholder Relations</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Transparency Reports')}>
              <Text style={styles.actionButtonText}>Transparency Reports</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Governance Analytics')}>
              <Text style={styles.actionButtonText}>Governance Analytics</Text>
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

export default ComprehensiveGovernanceScreen;
