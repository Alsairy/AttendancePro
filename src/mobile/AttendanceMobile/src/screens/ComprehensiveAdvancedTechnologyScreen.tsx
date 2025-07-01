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

interface AdvancedTechnologyData {
  totalTechnologies: number;
  activeTechnologies: number;
  researchProjects: number;
  technologyROI: number;
  innovationIndex: number;
  patentApplications: number;
  technologyAdoption: number;
}

const ComprehensiveAdvancedTechnologyScreen: React.FC = () => {
  const [technologyData, setTechnologyData] = useState<AdvancedTechnologyData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadTechnologyData();
  }, []);

  const loadTechnologyData = async () => {
    try {
      setLoading(true);
      const data = {
        totalTechnologies: 85,
        activeTechnologies: 72,
        researchProjects: 28,
        technologyROI: 245.8,
        innovationIndex: 89.3,
        patentApplications: 15,
        technologyAdoption: 78.5,
      };
      setTechnologyData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load advanced technology data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Advanced Technology Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Advanced Technology Management</Text>
      
      {technologyData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Technologies</Text>
              <Text style={styles.metricValue}>
                {technologyData.totalTechnologies}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Technologies</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {technologyData.activeTechnologies}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Research Projects</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {technologyData.researchProjects}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Technology ROI</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {technologyData.technologyROI.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Innovation Index</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {technologyData.innovationIndex.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Patent Applications</Text>
              <Text style={styles.metricValue}>
                {technologyData.patentApplications}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Technology Adoption</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {technologyData.technologyAdoption.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Technology Roadmap')}>
              <Text style={styles.actionButtonText}>Technology Roadmap</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Research and Development')}>
              <Text style={styles.actionButtonText}>Research and Development</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Innovation Labs')}>
              <Text style={styles.actionButtonText}>Innovation Labs</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Technology Assessment')}>
              <Text style={styles.actionButtonText}>Technology Assessment</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Patent Management')}>
              <Text style={styles.actionButtonText}>Patent Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Technology Transfer')}>
              <Text style={styles.actionButtonText}>Technology Transfer</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Innovation Metrics')}>
              <Text style={styles.actionButtonText}>Innovation Metrics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Technology Reports')}>
              <Text style={styles.actionButtonText}>Technology Reports</Text>
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

export default ComprehensiveAdvancedTechnologyScreen;
