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

interface DigitalTransformationData {
  digitalMaturity: number;
  automationLevel: number;
  cloudAdoption: number;
  dataDigitization: number;
  processOptimization: number;
  technologyInvestment: number;
  digitalSkills: number;
}

const ComprehensiveDigitalTransformationScreen: React.FC = () => {
  const [digitalData, setDigitalData] = useState<DigitalTransformationData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDigitalData();
  }, []);

  const loadDigitalData = async () => {
    try {
      setLoading(true);
      const data = {
        digitalMaturity: 78.5,
        automationLevel: 65.2,
        cloudAdoption: 89.7,
        dataDigitization: 82.3,
        processOptimization: 74.8,
        technologyInvestment: 2850000,
        digitalSkills: 71.4,
      };
      setDigitalData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load digital transformation data');
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
        <Text style={styles.loadingText}>Loading Digital Transformation Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Digital Transformation</Text>
      
      {digitalData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Digital Maturity</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {digitalData.digitalMaturity.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Automation Level</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {digitalData.automationLevel.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Cloud Adoption</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {digitalData.cloudAdoption.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Data Digitization</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {digitalData.dataDigitization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Process Optimization</Text>
              <Text style={styles.metricValue}>
                {digitalData.processOptimization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Technology Investment</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(digitalData.technologyInvestment)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Digital Skills</Text>
              <Text style={styles.metricValue}>
                {digitalData.digitalSkills.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Digital Strategy')}>
              <Text style={styles.actionButtonText}>Digital Strategy</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Process Automation')}>
              <Text style={styles.actionButtonText}>Process Automation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Cloud Migration')}>
              <Text style={styles.actionButtonText}>Cloud Migration</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Data Analytics')}>
              <Text style={styles.actionButtonText}>Data Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Digital Skills Training')}>
              <Text style={styles.actionButtonText}>Digital Skills Training</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Technology Roadmap')}>
              <Text style={styles.actionButtonText}>Technology Roadmap</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Change Management')}>
              <Text style={styles.actionButtonText}>Change Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Digital Metrics')}>
              <Text style={styles.actionButtonText}>Digital Metrics</Text>
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

export default ComprehensiveDigitalTransformationScreen;
