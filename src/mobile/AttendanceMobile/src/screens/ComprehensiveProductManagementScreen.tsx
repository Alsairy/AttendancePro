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

interface ProductData {
  totalProducts: number;
  activeProducts: number;
  productRevenue: number;
  developmentProjects: number;
  timeToMarket: number;
  customerSatisfaction: number;
  marketShare: number;
}

const ComprehensiveProductManagementScreen: React.FC = () => {
  const [productData, setProductData] = useState<ProductData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadProductData();
  }, []);

  const loadProductData = async () => {
    try {
      setLoading(true);
      const data = {
        totalProducts: 25,
        activeProducts: 22,
        productRevenue: 1850000,
        developmentProjects: 8,
        timeToMarket: 185.5,
        customerSatisfaction: 4.3,
        marketShare: 15.7,
      };
      setProductData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load product data');
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
        <Text style={styles.loadingText}>Loading Product Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Product Management</Text>
      
      {productData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Products</Text>
              <Text style={styles.metricValue}>
                {productData.totalProducts}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Products</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {productData.activeProducts}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Product Revenue</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {formatCurrency(productData.productRevenue)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Development Projects</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {productData.developmentProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Time to Market</Text>
              <Text style={styles.metricValue}>
                {productData.timeToMarket.toFixed(0)} days
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Customer Satisfaction</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {productData.customerSatisfaction.toFixed(1)}/5.0
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Market Share</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {productData.marketShare.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Product Roadmap')}>
              <Text style={styles.actionButtonText}>Product Roadmap</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Feature Management')}>
              <Text style={styles.actionButtonText}>Feature Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Product Analytics')}>
              <Text style={styles.actionButtonText}>Product Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Market Research')}>
              <Text style={styles.actionButtonText}>Market Research</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Competitive Analysis')}>
              <Text style={styles.actionButtonText}>Competitive Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'User Feedback')}>
              <Text style={styles.actionButtonText}>User Feedback</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Product Metrics')}>
              <Text style={styles.actionButtonText}>Product Metrics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Launch Management')}>
              <Text style={styles.actionButtonText}>Launch Management</Text>
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

export default ComprehensiveProductManagementScreen;
