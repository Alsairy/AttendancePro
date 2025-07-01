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

interface EventData {
  totalEvents: number;
  upcomingEvents: number;
  activeRegistrations: number;
  eventAttendance: number;
  eventBudget: number;
  venueBookings: number;
  eventSatisfaction: number;
}

const ComprehensiveEventManagementScreen: React.FC = () => {
  const [eventData, setEventData] = useState<EventData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadEventData();
  }, []);

  const loadEventData = async () => {
    try {
      setLoading(true);
      const data = {
        totalEvents: 125,
        upcomingEvents: 35,
        activeRegistrations: 850,
        eventAttendance: 92.5,
        eventBudget: 450000,
        venueBookings: 15,
        eventSatisfaction: 4.6,
      };
      setEventData(data);
    } catch (error) {
      Alert.alert('Error', 'Failed to load event data');
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
        <Text style={styles.loadingText}>Loading Event Data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Event Management Dashboard</Text>
      
      {eventData && (
        <>
          <View style={styles.metricsContainer}>
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Total Events</Text>
              <Text style={styles.metricValue}>
                {eventData.totalEvents}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Upcoming Events</Text>
              <Text style={[styles.metricValue, { color: '#007AFF' }]}>
                {eventData.upcomingEvents}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Active Registrations</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {eventData.activeRegistrations}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Event Attendance</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {eventData.eventAttendance.toFixed(1)}%
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Event Budget</Text>
              <Text style={styles.metricValue}>
                {formatCurrency(eventData.eventBudget)}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Venue Bookings</Text>
              <Text style={styles.metricValue}>
                {eventData.venueBookings}
              </Text>
            </View>
            
            <View style={styles.metricCard}>
              <Text style={styles.metricLabel}>Event Satisfaction</Text>
              <Text style={[styles.metricValue, { color: '#4CAF50' }]}>
                {eventData.eventSatisfaction.toFixed(1)}/5.0
              </Text>
            </View>
          </View>
          
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Event Planning')}>
              <Text style={styles.actionButtonText}>Event Planning</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Registration Management')}>
              <Text style={styles.actionButtonText}>Registration Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Venue Management')}>
              <Text style={styles.actionButtonText}>Venue Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Event Marketing')}>
              <Text style={styles.actionButtonText}>Event Marketing</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Budget Management')}>
              <Text style={styles.actionButtonText}>Budget Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Attendee Management')}>
              <Text style={styles.actionButtonText}>Attendee Management</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Event Analytics')}>
              <Text style={styles.actionButtonText}>Event Analytics</Text>
            </TouchableOpacity>
            
            <TouchableOpacity style={styles.actionButton} onPress={() => Alert.alert('Feature', 'Post-Event Reports')}>
              <Text style={styles.actionButtonText}>Post-Event Reports</Text>
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

export default ComprehensiveEventManagementScreen;
