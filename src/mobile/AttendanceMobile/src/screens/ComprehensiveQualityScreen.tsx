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

interface QualityData {
  qualityScore: number;
  defectRate: number;
  customerSatisfaction: number;
  auditCompliance: number;
  processEfficiency: number;
  correctionActions: number;
  qualityTraining: number;
}

const ComprehensiveQualityScreen: React.FC = () => {
  const [qualityData, setQualityData] = useState<QualityData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadQualityData();
  }, []);

  const loadQualityData = async () => {
    try {
      setLoading(true);
      const data = {
        qualityScore: 94.7,
        defectRate: 0.8,
        customerSatisfaction: 4.6,
        auditCompliance: 98.2,
        processEfficiency: 91.5,
        correctionActions: 15,
        qualityTraining: 87.3,
      };
      setQualityData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load quality data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Quality Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Quality Assurance Dashboard</Text>
      
      {qualityData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Quality Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {qualityData.qualityScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Defect Rate</Text>
              <Text style={[styles.metricValue, { color: qualityData.defectRate < 1 ? '#4CAF50' : '#FF9800' }]}>
                {qualityData.defectRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Customer Satisfaction</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {qualityData.customerSatisfaction.toFixed(1)}/5.0
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Audit Compliance</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {qualityData.auditCompliance.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Process Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {qualityData.processEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Corrective Actions</Text>
              <Text style={styles.metricValue}>
                {qualityData.correctionActions}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Quality Training</Text>
              <Text style={styles.metricValue}>
                {qualityData.qualityTraining.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Quality Control')}>
              <Text style={styles.actionButtonText}>Quality Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Inspection Management')}>
              <Text style={styles.actionButtonText}>Inspection Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Non-Conformance')}>
              <Text style={styles.actionButtonText}>Non-Conformance</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Audit Management')}>
              <Text style={styles.actionButtonText}>Audit Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Document Control')}>
              <Text style={styles.actionButtonText}>Document Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Training Records')}>
              <Text style={styles.actionButtonText}>Training Records</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Statistical Analysis')}>
              <Text style={styles.actionButtonText}>Statistical Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Continuous Improvement')}>
              <Text style={styles.actionButtonText}>Continuous Improvement</Text>
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

export default ComprehensiveQualityScreen;
