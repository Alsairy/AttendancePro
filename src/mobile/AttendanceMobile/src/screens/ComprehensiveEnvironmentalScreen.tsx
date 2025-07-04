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

interface EnvironmentalData {
  carbonFootprint: number;
  energyConsumption: number;
  wasteReduction: number;
  sustainabilityScore: number;
  recyclingRate: number;
  waterUsage: number;
  greenInitiatives: number;
}

const ComprehensiveEnvironmentalScreen: React.FC = () => {
  const [environmentalData, setEnvironmentalData] = useState<EnvironmentalData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadEnvironmentalData();
  }, []);

  const loadEnvironmentalData = async () => {
    try {
      setLoading(true);
      const data = {
        carbonFootprint: 1250.5,
        energyConsumption: 85420,
        wasteReduction: 23.8,
        sustainabilityScore: 87.3,
        recyclingRate: 78.5,
        waterUsage: 45200,
        greenInitiatives: 15,
      };
      setEnvironmentalData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load environmental data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Environmental Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Environmental Management</Text>
      
      {environmentalData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Carbon Footprint</Text>
              <Text style={styles.metricValue}>
                {environmentalData.carbonFootprint.toFixed(1)} tons CO2
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Energy Consumption</Text>
              <Text style={styles.metricValue}>
                {environmentalData.energyConsumption.toLocaleString()} kWh
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Waste Reduction</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {environmentalData.wasteReduction.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Sustainability Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {environmentalData.sustainabilityScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Recycling Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {environmentalData.recyclingRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Water Usage</Text>
              <Text style={styles.metricValue}>
                {environmentalData.waterUsage.toLocaleString()} gallons
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Green Initiatives</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {environmentalData.greenInitiatives}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Carbon Tracking')}>
              <Text style={styles.actionButtonText}>Carbon Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Energy Management')}>
              <Text style={styles.actionButtonText}>Energy Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Waste Management')}>
              <Text style={styles.actionButtonText}>Waste Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Sustainability Reporting')}>
              <Text style={styles.actionButtonText}>Sustainability Reporting</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Environmental Compliance')}>
              <Text style={styles.actionButtonText}>Environmental Compliance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Green Initiatives')}>
              <Text style={styles.actionButtonText}>Green Initiatives</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Resource Optimization')}>
              <Text style={styles.actionButtonText}>Resource Optimization</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Environmental Analytics')}>
              <Text style={styles.actionButtonText}>Environmental Analytics</Text>
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

export default ComprehensiveEnvironmentalScreen;
