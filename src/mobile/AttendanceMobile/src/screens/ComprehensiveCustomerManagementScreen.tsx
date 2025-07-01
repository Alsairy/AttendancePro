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

interface CustomerData {
  totalCustomers: number;
  activeCustomers: number;
  newCustomers: number;
  customerSatisfaction: number;
  retentionRate: number;
  lifetimeValue: number;
  supportTickets: number;
}

const ComprehensiveCustomerManagementScreen: React.FC = () => {
  const [customerData, setCustomerData] = useState<CustomerData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadCustomerData();
  }, []);

  const loadCustomerData = async () => {
    try {
      setLoading(true);
      const data = {
        totalCustomers: 2450,
        activeCustomers: 2180,
        newCustomers: 125,
        customerSatisfaction: 4.3,
        retentionRate: 89.2,
        lifetimeValue: 15750,
        supportTickets: 45,
      };
      setCustomerData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load customer data');
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
        <Text style={styles.loadingText}>Loading Customer Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Customer Relationship Management</Text>
      
      {customerData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Customers</Text>
              <Text style={styles.metricValue}>
                {customerData.totalCustomers.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Customers</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerData.activeCustomers.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>New Customers</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {customerData.newCustomers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Customer Satisfaction</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerData.customerSatisfaction.toFixed(1)}/5.0
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Retention Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerData.retentionRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Customer Lifetime Value</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(customerData.lifetimeValue)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Open Support Tickets</Text>
              <Text style={[styles.metricValue, { color: customerData.supportTickets > 50 ? '#FF9800' : '#4CAF50' }]}>
                {customerData.supportTickets}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Customer Profiles')}>
              <Text style={styles.actionButtonText}>Customer Profiles</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sales Pipeline')}>
              <Text style={styles.actionButtonText}>Sales Pipeline</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Support Management')}>
              <Text style={styles.actionButtonText}>Support Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Marketing Campaigns')}>
              <Text style={styles.actionButtonText}>Marketing Campaigns</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Customer Analytics')}>
              <Text style={styles.actionButtonText}>Customer Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Loyalty Programs')}>
              <Text style={styles.actionButtonText}>Loyalty Programs</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Feedback Management')}>
              <Text style={styles.actionButtonText}>Feedback Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Communication Hub')}>
              <Text style={styles.actionButtonText}>Communication Hub</Text>
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

export default ComprehensiveCustomerManagementScreen;
