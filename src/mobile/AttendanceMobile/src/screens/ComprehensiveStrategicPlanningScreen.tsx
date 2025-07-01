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

interface StrategicPlanningData {
  totalStrategies: number;
  activeInitiatives: number;
  completedGoals: number;
  strategicAlignment: number;
  performanceScore: number;
  budgetAllocation: number;
  riskAssessment: number;
}

const ComprehensiveStrategicPlanningScreen: React.FC = () => {
  const [strategicData, setStrategicData] = useState<StrategicPlanningData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStrategicData();
  }, []);

  const loadStrategicData = async () => {
    try {
      setLoading(true);
      const data = {
        totalStrategies: 25,
        activeInitiatives: 18,
        completedGoals: 42,
        strategicAlignment: 87.3,
        performanceScore: 91.5,
        budgetAllocation: 3250000,
        riskAssessment: 15.7,
      };
      setStrategicData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load strategic planning data');
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
        <Text style={styles.loadingText}>Loading Strategic Planning Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Strategic Planning Dashboard</Text>
      
      {strategicData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Strategies</Text>
              <Text style={styles.metricValue}>
                {strategicData.totalStrategies}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Initiatives</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {strategicData.activeInitiatives}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Completed Goals</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {strategicData.completedGoals}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Strategic Alignment</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {strategicData.strategicAlignment.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Performance Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {strategicData.performanceScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Budget Allocation</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(strategicData.budgetAllocation)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Risk Assessment</Text>
              <Text style={[styles.metricValue, { color: strategicData.riskAssessment < 20 ? '#4CAF50' : '#FF9800' }]}>
                {strategicData.riskAssessment.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Strategic Planning')}>
              <Text style={styles.actionButtonText}>Strategic Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Goal Management')}>
              <Text style={styles.actionButtonText}>Goal Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Tracking')}>
              <Text style={styles.actionButtonText}>Performance Tracking</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Strategic Analysis')}>
              <Text style={styles.actionButtonText}>Strategic Analysis</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Initiative Management')}>
              <Text style={styles.actionButtonText}>Initiative Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Resource Allocation')}>
              <Text style={styles.actionButtonText}>Resource Allocation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Strategic Reports')}>
              <Text style={styles.actionButtonText}>Strategic Reports</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Competitive Analysis')}>
              <Text style={styles.actionButtonText}>Competitive Analysis</Text>
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

export default ComprehensiveStrategicPlanningScreen;
