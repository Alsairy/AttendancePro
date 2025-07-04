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

interface SalesData {
  totalRevenue: number;
  salesTarget: number;
  achievementRate: number;
  activePipeline: number;
  closedDeals: number;
  averageDealSize: number;
  salesCycle: number;
}

const ComprehensiveSalesScreen: React.FC = () => {
  const [salesData, setSalesData] = useState<SalesData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSalesData();
  }, []);

  const loadSalesData = async () => {
    try {
      setLoading(true);
      const data = {
        totalRevenue: 2850000,
        salesTarget: 3000000,
        achievementRate: 95.0,
        activePipeline: 1250000,
        closedDeals: 185,
        averageDealSize: 15405,
        salesCycle: 45.5,
      };
      setSalesData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load sales data');
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
        <Text style={styles.loadingText}>Loading Sales Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Sales Management</Text>
      
      {salesData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Revenue</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {formatCurrency(salesData.totalRevenue)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Sales Target</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(salesData.salesTarget)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Achievement Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {salesData.achievementRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Pipeline</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {formatCurrency(salesData.activePipeline)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Closed Deals</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {salesData.closedDeals}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Average Deal Size</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(salesData.averageDealSize)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Sales Cycle</Text>
              <Text style={styles.metricValue}>
                {salesData.salesCycle.toFixed(1)} days
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Pipeline Management')}>
              <Text style={styles.actionButtonText}>Pipeline Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Lead Qualification')}>
              <Text style={styles.actionButtonText}>Lead Qualification</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Opportunity Management')}>
              <Text style={styles.actionButtonText}>Opportunity Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sales Forecasting')}>
              <Text style={styles.actionButtonText}>Sales Forecasting</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Territory Management')}>
              <Text style={styles.actionButtonText}>Territory Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sales Analytics')}>
              <Text style={styles.actionButtonText}>Sales Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Commission Management')}>
              <Text style={styles.actionButtonText}>Commission Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sales Reports')}>
              <Text style={styles.actionButtonText}>Sales Reports</Text>
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

export default ComprehensiveSalesScreen;
