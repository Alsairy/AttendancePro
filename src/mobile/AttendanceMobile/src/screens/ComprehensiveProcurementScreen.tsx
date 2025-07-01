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
import { ProcurementService } from '../services/ProcurementService';

interface ProcurementData {
  totalSpend: number;
  monthlySpend: number;
  costSavings: number;
  pendingOrders: number;
  processedOrders: number;
  averageProcessingTime: number;
  budgetUtilization: number;
}

const ComprehensiveProcurementScreen: React.FC = () => {
  const [procurementData, setProcurementData] = useState<ProcurementData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadProcurementData();
  }, []);

  const loadProcurementData = async () => {
    try {
      setLoading(true);
      const data = await ProcurementService.getProcurementDashboard();
      setProcurementData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load procurement data');
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
        <Text style={styles.loadingText}>Loading Procurement Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Procurement Dashboard</Text>
      
      {procurementData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Spend</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(procurementData.totalSpend)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Monthly Spend</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(procurementData.monthlySpend)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Cost Savings</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {formatCurrency(procurementData.costSavings)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Pending Orders</Text>
              <Text style={styles.metricValue}>
                {procurementData.pendingOrders}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Processed Orders</Text>
              <Text style={styles.metricValue}>
                {procurementData.processedOrders}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Avg Processing Time</Text>
              <Text style={styles.metricValue}>
                {procurementData.averageProcessingTime} days
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Budget Utilization</Text>
              <Text style={styles.metricValue}>
                {procurementData.budgetUtilization.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'View Purchase Orders')}>
              <Text style={styles.actionButtonText}>Purchase Orders</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Vendor Analysis')}>
              <Text style={styles.actionButtonText}>Vendor Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Contract Management')}>
              <Text style={styles.actionButtonText}>Contract Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Supplier Performance')}>
              <Text style={styles.actionButtonText}>Supplier Performance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Spend Analysis')}>
              <Text style={styles.actionButtonText}>Spend Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Compliance Report')}>
              <Text style={styles.actionButtonText}>Compliance Report</Text>
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

export default ComprehensiveProcurementScreen;
