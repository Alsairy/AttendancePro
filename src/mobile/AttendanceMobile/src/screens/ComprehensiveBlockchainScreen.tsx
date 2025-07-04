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

interface BlockchainData {
  totalTransactions: number;
  activeContracts: number;
  blockchainNodes: number;
  transactionThroughput: number;
  networkSecurity: number;
  consensusEfficiency: number;
  smartContractDeployments: number;
}

const ComprehensiveBlockchainScreen: React.FC = () => {
  const [blockchainData, setBlockchainData] = useState<BlockchainData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadBlockchainData();
  }, []);

  const loadBlockchainData = async () => {
    try {
      setLoading(true);
      const data = {
        totalTransactions: 1250000,
        activeContracts: 185,
        blockchainNodes: 25,
        transactionThroughput: 2500,
        networkSecurity: 98.7,
        consensusEfficiency: 94.2,
        smartContractDeployments: 45,
      };
      setBlockchainData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load blockchain data');
    } finally {
      setLoading(false);
    }
  };

  const formatNumber = (num: number) => {
    if (num >= 1000000) {
      return (num / 1000000).toFixed(1) + 'M';
    }
    if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'K';
    }
    return num.toString();
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007AFF" />
        <Text style={styles.loadingText}>Loading Blockchain Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Blockchain Management</Text>
      
      {blockchainData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Transactions</Text>
              <Text style={styles.metricValue}>
                {formatNumber(blockchainData.totalTransactions)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Contracts</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {blockchainData.activeContracts}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Blockchain Nodes</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {blockchainData.blockchainNodes}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Transaction Throughput</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {formatNumber(blockchainData.transactionThroughput)} TPS
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Network Security</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {blockchainData.networkSecurity.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Consensus Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {blockchainData.consensusEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Smart Contract Deployments</Text>
              <Text style={styles.metricValue}>
                {blockchainData.smartContractDeployments}
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Smart Contracts')}>
              <Text style={styles.actionButtonText}>Smart Contracts</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Transaction Management')}>
              <Text style={styles.actionButtonText}>Transaction Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Node Management')}>
              <Text style={styles.actionButtonText}>Node Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Consensus Monitoring')}>
              <Text style={styles.actionButtonText}>Consensus Monitoring</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Wallet Management')}>
              <Text style={styles.actionButtonText}>Wallet Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Security Auditing')}>
              <Text style={styles.actionButtonText}>Security Auditing</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Performance Analytics')}>
              <Text style={styles.actionButtonText}>Performance Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Blockchain Reports')}>
              <Text style={styles.actionButtonText}>Blockchain Reports</Text>
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

export default ComprehensiveBlockchainScreen;
