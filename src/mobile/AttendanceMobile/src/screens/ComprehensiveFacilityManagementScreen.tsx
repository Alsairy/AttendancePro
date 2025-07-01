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

interface FacilityData {
  totalFacilities: number;
  occupancyRate: number;
  maintenanceRequests: number;
  energyEfficiency: number;
  spaceUtilization: number;
  facilityCosts: number;
  safetyScore: number;
}

const ComprehensiveFacilityManagementScreen: React.FC = () => {
  const [facilityData, setFacilityData] = useState<FacilityData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadFacilityData();
  }, []);

  const loadFacilityData = async () => {
    try {
      setLoading(true);
      const data = {
        totalFacilities: 25,
        occupancyRate: 87.3,
        maintenanceRequests: 45,
        energyEfficiency: 92.1,
        spaceUtilization: 78.5,
        facilityCosts: 850000,
        safetyScore: 96.8,
      };
      setFacilityData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load facility data');
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
        <Text style={styles.loadingText}>Loading Facility Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Facility Management Dashboard</Text>
      
      {facilityData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Facilities</Text>
              <Text style={styles.metricValue}>
                {facilityData.totalFacilities}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Occupancy Rate</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {facilityData.occupancyRate.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Maintenance Requests</Text>
              <Text style={[styles.metricValue, { color: facilityData.maintenanceRequests < 50 ? '#4CAF50' : '#FF9800' }]}>
                {facilityData.maintenanceRequests}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Energy Efficiency</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {facilityData.energyEfficiency.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Space Utilization</Text>
              <Text style={styles.metricValue}>
                {facilityData.spaceUtilization.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Facility Costs</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(facilityData.facilityCosts)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Safety Score</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {facilityData.safetyScore.toFixed(1)}%
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Space Management')}>
              <Text style={styles.actionButtonText}>Space Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Maintenance Scheduling')}>
              <Text style={styles.actionButtonText}>Maintenance Scheduling</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Energy Management')}>
              <Text style={styles.actionButtonText}>Energy Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Safety Management')}>
              <Text style={styles.actionButtonText}>Safety Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Booking System')}>
              <Text style={styles.actionButtonText}>Booking System</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Cost Management')}>
              <Text style={styles.actionButtonText}>Cost Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Environmental Controls')}>
              <Text style={styles.actionButtonText}>Environmental Controls</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Facility Reports')}>
              <Text style={styles.actionButtonText}>Facility Reports</Text>
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

export default ComprehensiveFacilityManagementScreen;
