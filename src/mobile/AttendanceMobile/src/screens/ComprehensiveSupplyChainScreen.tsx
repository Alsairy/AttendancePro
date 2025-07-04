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

interface SupplyChainData {
  totalSuppliers: number;
  activeOrders: number;
  onTimeDelivery: number;
  qualityScore: number;
  costEfficiency: number;
  riskScore: number;
  sustainabilityScore: number;
}

const ComprehensiveSupplyChainScreen: React.FC = () => {
  const [supplyChainData, setSupplyChainData] = useState<SupplyChainData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSupplyChainData();
  }, []);

  const loadSupplyChainData = async () => {
    try {
      setLoading(true);
      const data = {
        totalSuppliers: 180,
        activeOrders: 45,
        onTimeDelivery: 94.2,
        qualityScore: 87.5,
        costEfficiency: 91.3,
        riskScore: 15.8,
        sustainabilityScore: 82.1,
      };
      setSupplyChainData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load supply chain data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Supply Chain Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Supply Chain Management</Text>
      
      {supplyChainData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Suppliers</Text>
              <Text style={styles.metricValue}>
                {supplyChainData.totalSuppliers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Orders</Text>
              <Text style={styles.metricValue}>
                {supplyChainData.activeOrders}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>On-Time Delivery</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.onTimeDelivery.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Quality Score</Text>
              <Text style={styles.metricValue}>
                {supplyChainData.qualityScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Cost Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.costEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Risk Score</Text>
              <Text style={[styles.metricValue, { color: supplyChainData.riskScore < 20 ? '#4CAF50' : '#FF9800' }]}>
                {supplyChainData.riskScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Sustainability Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.sustainabilityScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Supplier Management')}>
              <Text style={styles.actionButtonText}>Supplier Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Order Tracking')}>
              <Text style={styles.actionButtonText}>Order Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Quality Control')}>
              <Text style={styles.actionButtonText}>Quality Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Assessment')}>
              <Text style={styles.actionButtonText}>Risk Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sustainability Tracking')}>
              <Text style={styles.actionButtonText}>Sustainability Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Analytics')}>
              <Text style={styles.actionButtonText}>Performance Analytics</Text>
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

export default ComprehensiveSupplyChainScreen;
