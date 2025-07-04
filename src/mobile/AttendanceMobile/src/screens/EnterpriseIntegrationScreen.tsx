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
import { Colors } from '../utils/colors';
import { EnterpriseIntegrationService } from '../services/EnterpriseIntegrationService';

interface IntegrationStatus {
  integrationId: string;
  integrationType: string;
  status: string;
  lastSync: string;
  health: string;
  recordCount: number;
}

interface IntegrationHealth {
  tenantId: string;
  totalIntegrations: number;
  healthyIntegrations: number;
  overallHealth: number;
  lastHealthCheck: string;
  issues: string[];
}

const EnterpriseIntegrationScreen: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [selectedTab, setSelectedTab] = useState('status');
  const [integrations, setIntegrations] = useState<IntegrationStatus[]>([]);
  const [health, setHealth] = useState<IntegrationHealth | null>(null);
  const integrationService = new EnterpriseIntegrationService();

  useEffect(() => {
    loadData();
  }, [selectedTab]);

  const loadData = async () => {
    try {
      setLoading(true);
      if (selectedTab === 'status') {
        const statusData = await integrationService.getIntegrationStatus();
        setIntegrations(statusData);
      } else if (selectedTab === 'health') {
        const healthData = await integrationService.getIntegrationHealth();
        setHealth(healthData);
      }
    } catch (error) {
      Alert.alert('Error', 'Failed to load integration data');
    } finally {
      setLoading(false);
    }
  };

  const syncIntegration = async (integrationType: string) => {
    try {
      setLoading(true);
      const result = await integrationService.syncData(integrationType);
      if (result) {
        Alert.alert('Success', `${integrationType} data synced successfully`);
        loadData();
      } else {
        Alert.alert('Error', `Failed to sync ${integrationType} data`);
      }
    } catch (error) {
      Alert.alert('Error', `Failed to sync ${integrationType} data`);
    } finally {
      setLoading(false);
    }
  };

  const disconnectIntegration = async (integrationType: string) => {
    try {
      const result = await integrationService.disconnectIntegration(integrationType);
      if (result) {
        Alert.alert('Success', `${integrationType} disconnected successfully`);
        loadData();
      } else {
        Alert.alert('Error', `Failed to disconnect ${integrationType}`);
      }
    } catch (error) {
      Alert.alert('Error', `Failed to disconnect ${integrationType}`);
    }
  };

  const renderStatus = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Integration Status</Text>
      {integrations.map((integration, index) => (
        <View key={index} style={styles.integrationCard}>
          <View style={styles.integrationHeader}>
            <Text style={styles.integrationType}>{integration.integrationType}</Text>
            <View style={[styles.statusBadge, getStatusStyle(integration.status)]}>
              <Text style={[styles.statusText, getStatusTextStyle(integration.status)]}>
                {integration.status}
              </Text>
            </View>
          </View>
          <Text style={styles.integrationHealth}>Health: {integration.health}</Text>
          <Text style={styles.integrationRecords}>Records: {integration.recordCount.toLocaleString()}</Text>
          <Text style={styles.integrationSync}>
            Last Sync: {new Date(integration.lastSync).toLocaleString()}
          </Text>
          <View style={styles.integrationActions}>
            <TouchableOpacity
              style={styles.syncButton}
              onPress={() => syncIntegration(integration.integrationType)}
            >
              <Text style={styles.syncButtonText}>Sync</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={styles.disconnectButton}
              onPress={() => disconnectIntegration(integration.integrationType)}
            >
              <Text style={styles.disconnectButtonText}>Disconnect</Text>
            </TouchableOpacity>
          </View>
        </View>
      ))}
    </View>
  );

  const renderHealth = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Integration Health</Text>
      {health && (
        <View>
          <View style={styles.healthOverview}>
            <Text style={styles.healthTitle}>Overall Health</Text>
            <Text style={[styles.healthScore, getHealthColor(health.overallHealth)]}>
              {health.overallHealth.toFixed(1)}%
            </Text>
            <Text style={styles.healthStatus}>
              {health.healthyIntegrations}/{health.totalIntegrations} Integrations Healthy
            </Text>
          </View>

          <View style={styles.healthDetails}>
            <Text style={styles.healthDetailsTitle}>Health Details</Text>
            <Text style={styles.healthDetail}>Total Integrations: {health.totalIntegrations}</Text>
            <Text style={styles.healthDetail}>Healthy Integrations: {health.healthyIntegrations}</Text>
            <Text style={styles.healthDetail}>
              Last Check: {new Date(health.lastHealthCheck).toLocaleString()}
            </Text>
          </View>

          {health.issues.length > 0 && (
            <View style={styles.issuesSection}>
              <Text style={styles.issuesTitle}>Issues</Text>
              {health.issues.map((issue, index) => (
                <Text key={index} style={styles.issueText}>{issue}</Text>
              ))}
            </View>
          )}
        </View>
      )}
    </View>
  );

  const renderConnections = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Available Integrations</Text>
      
      <View style={styles.connectionCard}>
        <Text style={styles.connectionTitle}>SAP HCM</Text>
        <Text style={styles.connectionDescription}>Connect to SAP Human Capital Management</Text>
        <TouchableOpacity style={styles.connectButton}>
          <Text style={styles.connectButtonText}>Connect</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.connectionCard}>
        <Text style={styles.connectionTitle}>Oracle HCM Cloud</Text>
        <Text style={styles.connectionDescription}>Connect to Oracle Human Capital Management</Text>
        <TouchableOpacity style={styles.connectButton}>
          <Text style={styles.connectButtonText}>Connect</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.connectionCard}>
        <Text style={styles.connectionTitle}>Salesforce</Text>
        <Text style={styles.connectionDescription}>Connect to Salesforce CRM</Text>
        <TouchableOpacity style={styles.connectButton}>
          <Text style={styles.connectButtonText}>Connect</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.connectionCard}>
        <Text style={styles.connectionTitle}>Workday</Text>
        <Text style={styles.connectionDescription}>Connect to Workday HCM</Text>
        <TouchableOpacity style={styles.connectButton}>
          <Text style={styles.connectButtonText}>Connect</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.connectionCard}>
        <Text style={styles.connectionTitle}>BambooHR</Text>
        <Text style={styles.connectionDescription}>Connect to BambooHR</Text>
        <TouchableOpacity style={styles.connectButton}>
          <Text style={styles.connectButtonText}>Connect</Text>
        </TouchableOpacity>
      </View>
    </View>
  );

  const getStatusStyle = (status: string) => {
    switch (status.toLowerCase()) {
      case 'active':
        return { backgroundColor: Colors.success };
      case 'inactive':
        return { backgroundColor: Colors.error };
      default:
        return { backgroundColor: Colors.warning };
    }
  };

  const getStatusTextStyle = (status: string) => {
    return { color: 'white' };
  };

  const getHealthColor = (health: number) => {
    if (health >= 95) return { color: Colors.success };
    if (health >= 85) return { color: Colors.warning };
    return { color: Colors.error };
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={Colors.primary} />
        <Text style={styles.loadingText}>Loading Integration Data...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Enterprise Integrations</Text>
      
      <View style={styles.tabBar}>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'status' && styles.activeTab]}
          onPress={() => setSelectedTab('status')}
        >
          <Text style={[styles.tabText, selectedTab === 'status' && styles.activeTabText]}>
            Status
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'health' && styles.activeTab]}
          onPress={() => setSelectedTab('health')}
        >
          <Text style={[styles.tabText, selectedTab === 'health' && styles.activeTabText]}>
            Health
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'connections' && styles.activeTab]}
          onPress={() => setSelectedTab('connections')}
        >
          <Text style={[styles.tabText, selectedTab === 'connections' && styles.activeTabText]}>
            Connect
          </Text>
        </TouchableOpacity>
      </View>

      <ScrollView style={styles.content}>
        {selectedTab === 'status' && renderStatus()}
        {selectedTab === 'health' && renderHealth()}
        {selectedTab === 'connections' && renderConnections()}
      </ScrollView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    padding: 20,
    color: Colors.text,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  loadingText: {
    marginTop: 16,
    fontSize: 16,
    color: Colors.textSecondary,
  },
  tabBar: {
    flexDirection: 'row',
    backgroundColor: 'white',
    marginHorizontal: 16,
    borderRadius: 8,
    padding: 4,
  },
  tab: {
    flex: 1,
    paddingVertical: 12,
    alignItems: 'center',
    borderRadius: 6,
  },
  activeTab: {
    backgroundColor: Colors.primary,
  },
  tabText: {
    fontSize: 14,
    fontWeight: '600',
    color: Colors.textSecondary,
  },
  activeTabText: {
    color: 'white',
  },
  content: {
    flex: 1,
    padding: 16,
  },
  tabContent: {
    flex: 1,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 16,
    color: Colors.text,
  },
  integrationCard: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  integrationHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  integrationType: {
    fontSize: 16,
    fontWeight: 'bold',
    color: Colors.text,
  },
  statusBadge: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 4,
  },
  statusText: {
    fontSize: 12,
    fontWeight: 'bold',
  },
  integrationHealth: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 4,
  },
  integrationRecords: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 4,
  },
  integrationSync: {
    fontSize: 12,
    color: Colors.textSecondary,
    marginBottom: 12,
  },
  integrationActions: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  syncButton: {
    backgroundColor: Colors.primary,
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 4,
    flex: 1,
    marginRight: 8,
    alignItems: 'center',
  },
  syncButtonText: {
    color: 'white',
    fontSize: 14,
    fontWeight: 'bold',
  },
  disconnectButton: {
    backgroundColor: Colors.error,
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 4,
    flex: 1,
    marginLeft: 8,
    alignItems: 'center',
  },
  disconnectButtonText: {
    color: 'white',
    fontSize: 14,
    fontWeight: 'bold',
  },
  healthOverview: {
    backgroundColor: 'white',
    padding: 20,
    marginBottom: 20,
    borderRadius: 12,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: Colors.border,
  },
  healthTitle: {
    fontSize: 16,
    color: Colors.textSecondary,
    marginBottom: 8,
  },
  healthScore: {
    fontSize: 36,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  healthStatus: {
    fontSize: 14,
    color: Colors.textSecondary,
  },
  healthDetails: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 16,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  healthDetailsTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 12,
    color: Colors.text,
  },
  healthDetail: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 4,
  },
  issuesSection: {
    backgroundColor: 'white',
    padding: 16,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  issuesTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 12,
    color: Colors.error,
  },
  issueText: {
    fontSize: 14,
    color: Colors.error,
    marginBottom: 4,
  },
  connectionCard: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  connectionTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
    color: Colors.text,
  },
  connectionDescription: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 12,
  },
  connectButton: {
    backgroundColor: Colors.success,
    padding: 12,
    borderRadius: 6,
    alignItems: 'center',
  },
  connectButtonText: {
    color: 'white',
    fontSize: 14,
    fontWeight: 'bold',
  },
});

export default EnterpriseIntegrationScreen;
