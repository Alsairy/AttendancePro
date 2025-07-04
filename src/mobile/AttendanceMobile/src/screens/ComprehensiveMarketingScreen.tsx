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

interface MarketingData {
  totalCampaigns: number;
  activeCampaigns: number;
  marketingROI: number;
  leadGeneration: number;
  conversionRate: number;
  marketingBudget: number;
  brandAwareness: number;
}

const ComprehensiveMarketingScreen: React.FC = () => {
  const [marketingData, setMarketingData] = useState<MarketingData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadMarketingData();
  }, []);

  const loadMarketingData = async () => {
    try {
      setLoading(true);
      const data = {
        totalCampaigns: 45,
        activeCampaigns: 18,
        marketingROI: 285.7,
        leadGeneration: 1250,
        conversionRate: 12.8,
        marketingBudget: 850000,
        brandAwareness: 78.5,
      };
      setMarketingData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load marketing data');
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
        <Text style={styles.loadingText}>Loading Marketing Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Marketing Management</Text>
      
      {marketingData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Campaigns</Text>
              <Text style={styles.metricValue}>
                {marketingData.totalCampaigns}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Campaigns</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {marketingData.activeCampaigns}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Marketing ROI</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {marketingData.marketingROI.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Lead Generation</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {marketingData.leadGeneration.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Conversion Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {marketingData.conversionRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Marketing Budget</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(marketingData.marketingBudget)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Brand Awareness</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {marketingData.brandAwareness.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Campaign Management')}>
              <Text style={styles.actionButtonText}>Campaign Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Lead Management')}>
              <Text style={styles.actionButtonText}>Lead Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Content Marketing')}>
              <Text style={styles.actionButtonText}>Content Marketing</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Social Media')}>
              <Text style={styles.actionButtonText}>Social Media</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Email Marketing')}>
              <Text style={styles.actionButtonText}>Email Marketing</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Marketing Analytics')}>
              <Text style={styles.actionButtonText}>Marketing Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Brand Management')}>
              <Text style={styles.actionButtonText}>Brand Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Market Research')}>
              <Text style={styles.actionButtonText}>Market Research</Text>
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

export default ComprehensiveMarketingScreen;
