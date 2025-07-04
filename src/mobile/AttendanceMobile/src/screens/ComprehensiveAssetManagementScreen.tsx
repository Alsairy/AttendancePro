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

interface AssetData {
  totalAssets: number;
  activeAssets: number;
  assetValue: number;
  depreciation: number;
  utilizationRate: number;
  maintenanceCosts: number;
  assetTurnover: number;
}

const ComprehensiveAssetManagementScreen: React.FC = () => {
  const [assetData, setAssetData] = useState<AssetData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadAssetData();
  }, []);

  const loadAssetData = async () => {
    try {
      setLoading(true);
      const data = {
        totalAssets: 2450,
        activeAssets: 2180,
        assetValue: 15750000,
        depreciation: 1250000,
        utilizationRate: 87.3,
        maintenanceCosts: 450000,
        assetTurnover: 2.8,
      };
      setAssetData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load asset data');
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
        <Text style={styles.loadingText}>Loading Asset Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Asset Management Dashboard</Text>
      
      {assetData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Assets</Text>
              <Text style={styles.metricValue}>
                {assetData.totalAssets.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Assets</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {assetData.activeAssets.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Asset Value</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(assetData.assetValue)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Annual Depreciation</Text>
              <Text style={[styles.metricValue, { color: '#FF9800' }]}>
                {formatCurrency(assetData.depreciation)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Utilization Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {assetData.utilizationRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Maintenance Costs</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(assetData.maintenanceCosts)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Asset Turnover</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {assetData.assetTurnover.toFixed(1)}x
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Registry')}>
              <Text style={styles.actionButtonText}>Asset Registry</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Tracking')}>
              <Text style={styles.actionButtonText}>Asset Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Depreciation Management')}>
              <Text style={styles.actionButtonText}>Depreciation Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Lifecycle')}>
              <Text style={styles.actionButtonText}>Asset Lifecycle</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Valuation')}>
              <Text style={styles.actionButtonText}>Asset Valuation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Disposal')}>
              <Text style={styles.actionButtonText}>Asset Disposal</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Reports')}>
              <Text style={styles.actionButtonText}>Asset Reports</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Asset Optimization')}>
              <Text style={styles.actionButtonText}>Asset Optimization</Text>
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

export default ComprehensiveAssetManagementScreen;
