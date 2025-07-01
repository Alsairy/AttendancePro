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

interface ComplianceData {
  overallScore: number;
  regulatoryCompliance: number;
  policyAdherence: number;
  auditReadiness: number;
  riskAssessment: number;
  trainingCompletion: number;
  documentationScore: number;
}

const ComprehensiveComplianceScreen: React.FC = () => {
  const [complianceData, setComplianceData] = useState<ComplianceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadComplianceData();
  }, []);

  const loadComplianceData = async () => {
    try {
      setLoading(true);
      const data = {
        overallScore: 94.2,
        regulatoryCompliance: 96.8,
        policyAdherence: 91.5,
        auditReadiness: 89.3,
        riskAssessment: 92.7,
        trainingCompletion: 87.4,
        documentationScore: 95.1,
      };
      setComplianceData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load compliance data');
    } finally {
      setLoading(false);
    }
  };

  const getScoreColor = (score: number) => {
    if (score >= 90) return '#4CAF50';
    if (score >= 80) return '#FF9800';
    return '#F44336';
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Compliance Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Compliance Management</Text>
      
      {complianceData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Overall Compliance Score</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.overallScore) }]}>
                {complianceData.overallScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Regulatory Compliance</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.regulatoryCompliance) }]}>
                {complianceData.regulatoryCompliance.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Policy Adherence</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.policyAdherence) }]}>
                {complianceData.policyAdherence.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Audit Readiness</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.auditReadiness) }]}>
                {complianceData.auditReadiness.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Risk Assessment</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.riskAssessment) }]}>
                {complianceData.riskAssessment.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Training Completion</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.trainingCompletion) }]}>
                {complianceData.trainingCompletion.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Documentation Score</Text>
              <Text style={[styles.metricValue, { color: getScoreColor(complianceData.documentationScore) }]}>
                {complianceData.documentationScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Regulatory Framework')}>
              <Text style={styles.actionButtonText}>Regulatory Framework</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Policy Management')}>
              <Text style={styles.actionButtonText}>Policy Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Audit Management')}>
              <Text style={styles.actionButtonText}>Audit Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Assessment')}>
              <Text style={styles.actionButtonText}>Risk Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Training Programs')}>
              <Text style={styles.actionButtonText}>Training Programs</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Document Control')}>
              <Text style={styles.actionButtonText}>Document Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Incident Reporting')}>
              <Text style={styles.actionButtonText}>Incident Reporting</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compliance Reports')}>
              <Text style={styles.actionButtonText}>Compliance Reports</Text>
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

export default ComprehensiveComplianceScreen;
