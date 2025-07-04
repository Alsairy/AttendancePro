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

interface DocumentData {
  totalDocuments: number;
  activeDocuments: number;
  documentCategories: number;
  storageUsed: number;
  accessRequests: number;
  complianceScore: number;
  versionControl: number;
}

const ComprehensiveDocumentManagementScreen: React.FC = () => {
  const [documentData, setDocumentData] = useState<DocumentData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDocumentData();
  }, []);

  const loadDocumentData = async () => {
    try {
      setLoading(true);
      const data = {
        totalDocuments: 15420,
        activeDocuments: 14850,
        documentCategories: 45,
        storageUsed: 2.8,
        accessRequests: 1250,
        complianceScore: 96.7,
        versionControl: 98.2,
      };
      setDocumentData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load document data');
    } finally {
      setLoading(false);
    }
  };

  const formatStorage = (gb: number) => {
    return `${gb.toFixed(1)} GB`;
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Document Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Document Management System</Text>
      
      {documentData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Documents</Text>
              <Text style={styles.metricValue}>
                {documentData.totalDocuments.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Documents</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {documentData.activeDocuments.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Document Categories</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {documentData.documentCategories}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Storage Used</Text>
              <Text style={styles.metricValue}>
                {formatStorage(documentData.storageUsed)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Access Requests</Text>
              <Text style={styles.metricValue}>
                {documentData.accessRequests.toLocaleString()}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Compliance Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {documentData.complianceScore.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Version Control</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {documentData.versionControl.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Document Library')}>
              <Text style={styles.actionButtonText}>Document Library</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'File Upload')}>
              <Text style={styles.actionButtonText}>File Upload</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Version Control')}>
              <Text style={styles.actionButtonText}>Version Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Access Control')}>
              <Text style={styles.actionButtonText}>Access Control</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Document Search')}>
              <Text style={styles.actionButtonText}>Document Search</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Workflow Automation')}>
              <Text style={styles.actionButtonText}>Workflow Automation</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Audit Trail')}>
              <Text style={styles.actionButtonText}>Audit Trail</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Collaboration Tools')}>
              <Text style={styles.actionButtonText}>Collaboration Tools</Text>
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

export default ComprehensiveDocumentManagementScreen;
