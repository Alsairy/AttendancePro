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

interface InnovationData {
  totalProjects: number;
  activeProjects: number;
  completedProjects: number;
  innovationScore: number;
  patentApplications: number;
  researchBudget: number;
  collaborations: number;
}

const ComprehensiveInnovationScreen: React.FC = () => {
  const [innovationData, setInnovationData] = useState<InnovationData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadInnovationData();
  }, []);

  const loadInnovationData = async () => {
    try {
      setLoading(true);
      const data = {
        totalProjects: 45,
        activeProjects: 28,
        completedProjects: 17,
        innovationScore: 89.3,
        patentApplications: 12,
        researchBudget: 1250000,
        collaborations: 8,
      };
      setInnovationData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load innovation data');
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
        <Text style={styles.loadingText}>Loading Innovation Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Innovation Management</Text>
      
      {innovationData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Projects</Text>
              <Text style={styles.metricValue}>
                {innovationData.totalProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Projects</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {innovationData.activeProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Completed Projects</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {innovationData.completedProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Innovation Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {innovationData.innovationScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Patent Applications</Text>
              <Text style={[styles.metricValue, { color: '#9C27B0' }]}>
                {innovationData.patentApplications}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Research Budget</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(innovationData.researchBudget)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Collaborations</Text>
              <Text style={styles.metricValue}>
                {innovationData.collaborations}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Innovation Pipeline')}>
              <Text style={styles.actionButtonText}>Innovation Pipeline</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Research & Development')}>
              <Text style={styles.actionButtonText}>Research & Development</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Patent Management')}>
              <Text style={styles.actionButtonText}>Patent Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Innovation Labs')}>
              <Text style={styles.actionButtonText}>Innovation Labs</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Technology Scouting')}>
              <Text style={styles.actionButtonText}>Technology Scouting</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Startup Partnerships')}>
              <Text style={styles.actionButtonText}>Startup Partnerships</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Innovation Metrics')}>
              <Text style={styles.actionButtonText}>Innovation Metrics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Idea Management')}>
              <Text style={styles.actionButtonText}>Idea Management</Text>
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

export default ComprehensiveInnovationScreen;
