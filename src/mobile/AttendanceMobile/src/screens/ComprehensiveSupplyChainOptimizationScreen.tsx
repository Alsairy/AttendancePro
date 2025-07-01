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

interface SupplyChainOptimizationData {
  totalSuppliers: number;
  activeSuppliers: number;
  supplyChainEfficiency: number;
  inventoryTurnover: number;
  leadTime: number;
  costOptimization: number;
  riskScore: number;
}

const ComprehensiveSupplyChainOptimizationScreen: React.FC = () => {
  const [supplyChainData, setSupplyChainData] = useState<SupplyChainOptimizationData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSupplyChainData();
  }, []);

  const loadSupplyChainData = async () => {
    try {
      setLoading(true);
      const data = {
        totalSuppliers: 185,
        activeSuppliers: 165,
        supplyChainEfficiency: 89.3,
        inventoryTurnover: 12.5,
        leadTime: 15.8,
        costOptimization: 23.7,
        riskScore: 18.5,
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
      <Text style={styles.title}>Supply Chain Optimization</Text>
      
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
              <Text style={styles.metricLabel}>Active Suppliers</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.activeSuppliers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Supply Chain Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.supplyChainEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Inventory Turnover</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.inventoryTurnover.toFixed(1)}x
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Lead Time</Text>
              <Text style={styles.metricValue}>
                {supplyChainData.leadTime.toFixed(1)} days
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Cost Optimization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {supplyChainData.costOptimization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Risk Score</Text>
              <Text style={[styles.metricValue, { color: supplyChainData.riskScore < 25 ? '#4CAF50' : '#FF9800' }]}>
                {supplyChainData.riskScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Supplier Management')}>
              <Text style={styles.actionButtonText}>Supplier Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Demand Planning')}>
              <Text style={styles.actionButtonText}>Demand Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Inventory Optimization')}>
              <Text style={styles.actionButtonText}>Inventory Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Logistics Management')}>
              <Text style={styles.actionButtonText}>Logistics Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Risk Management')}>
              <Text style={styles.actionButtonText}>Risk Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Analytics')}>
              <Text style={styles.actionButtonText}>Performance Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Cost Analysis')}>
              <Text style={styles.actionButtonText}>Cost Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Supply Chain Reports')}>
              <Text style={styles.actionButtonText}>Supply Chain Reports</Text>
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

export default ComprehensiveSupplyChainOptimizationScreen;
