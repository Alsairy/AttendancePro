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

interface LegalData {
  totalContracts: number;
  activeContracts: number;
  expiringContracts: number;
  complianceScore: number;
  legalRisks: number;
  legalCosts: number;
  documentReviews: number;
}

const ComprehensiveLegalManagementScreen: React.FC = () => {
  const [legalData, setLegalData] = useState<LegalData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLegalData();
  }, []);

  const loadLegalData = async () => {
    try {
      setLoading(true);
      const data = {
        totalContracts: 185,
        activeContracts: 165,
        expiringContracts: 12,
        complianceScore: 94.7,
        legalRisks: 8,
        legalCosts: 285000,
        documentReviews: 45,
      };
      setLegalData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load legal data');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Legal Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Legal Management Dashboard</Text>
      
      {legalData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Contracts</Text>
              <Text style={styles.metricValue}>
                {legalData.totalContracts}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Contracts</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {legalData.activeContracts}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Expiring Contracts</Text>
              <Text style={[styles.metricValue, { color: legalData.expiringContracts < 15 ? '#4CAF50' : '#FF9800' }]}>
                {legalData.expiringContracts}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Compliance Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {legalData.complianceScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Legal Risks</Text>
              <Text style={[styles.metricValue, { color: legalData.legalRisks < 10 ? '#4CAF50' : '#FF9800' }]}>
                {legalData.legalRisks}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Legal Costs</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(legalData.legalCosts)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Document Reviews</Text>
              <Text style={styles.metricValue}>
                {legalData.documentReviews}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Contract Management')}>
              <Text style={styles.actionButtonText}>Contract Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Legal Research')}>
              <Text style={styles.actionButtonText}>Legal Research</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compliance Tracking')}>
              <Text style={styles.actionButtonText}>Compliance Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Assessment')}>
              <Text style={styles.actionButtonText}>Risk Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Document Review')}>
              <Text style={styles.actionButtonText}>Document Review</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Legal Analytics')}>
              <Text style={styles.actionButtonText}>Legal Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Litigation Management')}>
              <Text style={styles.actionButtonText}>Litigation Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Legal Reports')}>
              <Text style={styles.actionButtonText}>Legal Reports</Text>
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

export default ComprehensiveLegalManagementScreen;
