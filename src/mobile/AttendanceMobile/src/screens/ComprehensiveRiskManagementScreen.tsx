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

interface RiskData {
  overallRiskScore: number;
  highRiskItems: number;
  mediumRiskItems: number;
  lowRiskItems: number;
  mitigationActions: number;
  riskTrend: number;
  complianceScore: number;
}

const ComprehensiveRiskManagementScreen: React.FC = () => {
  const [riskData, setRiskData] = useState<RiskData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadRiskData();
  }, []);

  const loadRiskData = async () => {
    try {
      setLoading(true);
      const data = {
        overallRiskScore: 23.5,
        highRiskItems: 8,
        mediumRiskItems: 25,
        lowRiskItems: 67,
        mitigationActions: 15,
        riskTrend: -5.2,
        complianceScore: 94.8,
      };
      setRiskData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load risk data');
    } finally {
      setLoading(false);
    }
  };

  const getRiskColor = (score: number) => {
    if (score < 20) return '#4CAF50';
    if (score < 40) return '#FF9800';
    return '#F44336';
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Risk Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Risk Management Dashboard</Text>
      
      {riskData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Overall Risk Score</Text>
              <Text style={[styles.metricValue, { color: getRiskColor(riskData.overallRiskScore) }]}>
                {riskData.overallRiskScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>High Risk Items</Text>
              <Text style={[styles.metricValue, { color: '#F44336' }]}>
                {riskData.highRiskItems}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Medium Risk Items</Text>
              <Text style={[styles.metricValue, { color: '#FF9800' }]}>
                {riskData.mediumRiskItems}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Low Risk Items</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {riskData.lowRiskItems}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Mitigation Actions</Text>
              <Text style={styles.metricValue}>
                {riskData.mitigationActions}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Risk Trend</Text>
              <Text style={[styles.metricValue, { color: riskData.riskTrend < 0 ? '#4CAF50' : '#F44336' }]}>
                {riskData.riskTrend > 0 ? '+' : ''}{riskData.riskTrend.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Compliance Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {riskData.complianceScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Assessment')}>
              <Text style={styles.actionButtonText}>Risk Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Register')}>
              <Text style={styles.actionButtonText}>Risk Register</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Mitigation Plans')}>
              <Text style={styles.actionButtonText}>Mitigation Plans</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Incident Management')}>
              <Text style={styles.actionButtonText}>Incident Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Business Continuity')}>
              <Text style={styles.actionButtonText}>Business Continuity</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Monitoring')}>
              <Text style={styles.actionButtonText}>Risk Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compliance Tracking')}>
              <Text style={styles.actionButtonText}>Compliance Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Reports')}>
              <Text style={styles.actionButtonText}>Risk Reports</Text>
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

export default ComprehensiveRiskManagementScreen;
