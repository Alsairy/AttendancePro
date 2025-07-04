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

interface CustomerServiceData {
  totalTickets: number;
  openTickets: number;
  resolvedTickets: number;
  averageResponseTime: number;
  customerSatisfaction: number;
  firstCallResolution: number;
  agentUtilization: number;
}

const ComprehensiveCustomerServiceScreen: React.FC = () => {
  const [customerServiceData, setCustomerServiceData] = useState<CustomerServiceData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadCustomerServiceData();
  }, []);

  const loadCustomerServiceData = async () => {
    try {
      setLoading(true);
      const data = {
        totalTickets: 1250,
        openTickets: 185,
        resolvedTickets: 1065,
        averageResponseTime: 2.5,
        customerSatisfaction: 4.2,
        firstCallResolution: 78.5,
        agentUtilization: 85.3,
      };
      setCustomerServiceData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load customer service data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Customer Service Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Customer Service Management</Text>
      
      {customerServiceData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Tickets</Text>
              <Text style={styles.metricValue}>
                {customerServiceData.totalTickets.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Open Tickets</Text>
              <Text style={[styles.metricValue, { color: '#FF9800' }]}>
                {customerServiceData.openTickets}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Resolved Tickets</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerServiceData.resolvedTickets.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Avg Response Time</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerServiceData.averageResponseTime.toFixed(1)} hrs
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Customer Satisfaction</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerServiceData.customerSatisfaction.toFixed(1)}/5.0
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>First Call Resolution</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerServiceData.firstCallResolution.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Agent Utilization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {customerServiceData.agentUtilization.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Ticket Management')}>
              <Text style={styles.actionButtonText}>Ticket Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Live Chat Support')}>
              <Text style={styles.actionButtonText}>Live Chat Support</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Knowledge Base')}>
              <Text style={styles.actionButtonText}>Knowledge Base</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Customer Feedback')}>
              <Text style={styles.actionButtonText}>Customer Feedback</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Agent Performance')}>
              <Text style={styles.actionButtonText}>Agent Performance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Service Analytics')}>
              <Text style={styles.actionButtonText}>Service Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Escalation Management')}>
              <Text style={styles.actionButtonText}>Escalation Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Service Reports')}>
              <Text style={styles.actionButtonText}>Service Reports</Text>
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

export default ComprehensiveCustomerServiceScreen;
