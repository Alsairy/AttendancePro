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

interface KnowledgeData {
  totalArticles: number;
  activeUsers: number;
  searchQueries: number;
  knowledgeScore: number;
  contentUpdates: number;
  userContributions: number;
  expertiseAreas: number;
}

const ComprehensiveKnowledgeManagementScreen: React.FC = () => {
  const [knowledgeData, setKnowledgeData] = useState<KnowledgeData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadKnowledgeData();
  }, []);

  const loadKnowledgeData = async () => {
    try {
      setLoading(true);
      const data = {
        totalArticles: 2450,
        activeUsers: 850,
        searchQueries: 15420,
        knowledgeScore: 92.8,
        contentUpdates: 125,
        userContributions: 380,
        expertiseAreas: 45,
      };
      setKnowledgeData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load knowledge data');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Knowledge Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Knowledge Management System</Text>
      
      {knowledgeData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Articles</Text>
              <Text style={styles.metricValue}>
                {knowledgeData.totalArticles.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Users</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {knowledgeData.activeUsers}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Search Queries</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {knowledgeData.searchQueries.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Knowledge Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {knowledgeData.knowledgeScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Content Updates</Text>
              <Text style={styles.metricValue}>
                {knowledgeData.contentUpdates}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>User Contributions</Text>
              <Text style={[styles.metricValue, { color: '#9C27B0' }]}>
                {knowledgeData.userContributions}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Expertise Areas</Text>
              <Text style={styles.metricValue}>
                {knowledgeData.expertiseAreas}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Knowledge Base')}>
              <Text style={styles.actionButtonText}>Knowledge Base</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Expert Directory')}>
              <Text style={styles.actionButtonText}>Expert Directory</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Content Creation')}>
              <Text style={styles.actionButtonText}>Content Creation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Knowledge Search')}>
              <Text style={styles.actionButtonText}>Knowledge Search</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Best Practices')}>
              <Text style={styles.actionButtonText}>Best Practices</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Learning Resources')}>
              <Text style={styles.actionButtonText}>Learning Resources</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Knowledge Analytics')}>
              <Text style={styles.actionButtonText}>Knowledge Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Collaboration Hub')}>
              <Text style={styles.actionButtonText}>Collaboration Hub</Text>
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

export default ComprehensiveKnowledgeManagementScreen;
