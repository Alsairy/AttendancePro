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

interface VendorData {
  totalVendors: number;
  activeVendors: number;
  preferredVendors: number;
  vendorSpend: number;
  averageRating: number;
  contractCompliance: number;
  onTimeDelivery: number;
}

const ComprehensiveVendorManagementScreen: React.FC = () => {
  const [vendorData, setVendorData] = useState<VendorData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadVendorData();
  }, []);

  const loadVendorData = async () => {
    try {
      setLoading(true);
      const data = {
        totalVendors: 450,
        activeVendors: 380,
        preferredVendors: 85,
        vendorSpend: 2850000,
        averageRating: 4.3,
        contractCompliance: 94.7,
        onTimeDelivery: 91.2,
      };
      setVendorData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load vendor data');
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
        <Text style={styles.loadingText}>Loading Vendor Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Vendor Management Dashboard</Text>
      
      {vendorData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Vendors</Text>
              <Text style={styles.metricValue}>
                {vendorData.totalVendors}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Vendors</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vendorData.activeVendors}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Preferred Vendors</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {vendorData.preferredVendors}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Vendor Spend</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(vendorData.vendorSpend)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Average Rating</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vendorData.averageRating.toFixed(1)}/5.0
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Contract Compliance</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vendorData.contractCompliance.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>On-Time Delivery</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {vendorData.onTimeDelivery.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vendor Directory')}>
              <Text style={styles.actionButtonText}>Vendor Directory</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vendor Onboarding')}>
              <Text style={styles.actionButtonText}>Vendor Onboarding</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Evaluation')}>
              <Text style={styles.actionButtonText}>Performance Evaluation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Contract Management')}>
              <Text style={styles.actionButtonText}>Contract Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vendor Payments')}>
              <Text style={styles.actionButtonText}>Vendor Payments</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Assessment')}>
              <Text style={styles.actionButtonText}>Risk Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vendor Analytics')}>
              <Text style={styles.actionButtonText}>Vendor Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vendor Communication')}>
              <Text style={styles.actionButtonText}>Vendor Communication</Text>
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

export default ComprehensiveVendorManagementScreen;
