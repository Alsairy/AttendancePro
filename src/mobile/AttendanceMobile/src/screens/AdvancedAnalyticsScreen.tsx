import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
  Dimensions,
} from 'react-native';
import { Colors } from '../utils/colors';
import { AdvancedAnalyticsService, PredictiveData, BehaviorPattern, AnomalyData } from '../services/AdvancedAnalyticsService';

const AdvancedAnalyticsScreen: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [selectedTab, setSelectedTab] = useState('predictive');
  const [predictiveData, setPredictiveData] = useState<PredictiveData[]>([]);
  const [behaviorPatterns, setBehaviorPatterns] = useState<BehaviorPattern[]>([]);
  const [anomalies, setAnomalies] = useState<AnomalyData[]>([]);
  const [realTimeData, setRealTimeData] = useState<any>(null);
  const analyticsService = new AdvancedAnalyticsService();

  useEffect(() => {
    loadData();
  }, [selectedTab]);

  const loadData = async () => {
    try {
      setLoading(true);
      switch (selectedTab) {
        case 'predictive':
          const predictive = await analyticsService.getPredictiveAnalytics();
          setPredictiveData(predictive.predictions);
          break;
        case 'behavioral':
          const behavioral = await analyticsService.getBehavioralAnalytics();
          setBehaviorPatterns(behavioral.behaviorPatterns);
          break;
        case 'anomaly':
          const anomalyData = await analyticsService.getAnomalyDetection();
          setAnomalies(anomalyData.anomalies);
          break;
        case 'realtime':
          const realTime = await analyticsService.getRealTimeAnalytics();
          setRealTimeData(realTime);
          break;
      }
    } catch (error) {
      Alert.alert('Error', 'Failed to load analytics data');
    } finally {
      setLoading(false);
    }
  };

  const renderPredictiveAnalytics = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Predictive Analytics</Text>
      {predictiveData.map((item, index) => (
        <View key={index} style={styles.predictionCard}>
          <Text style={styles.predictionMetric}>{item.metric}</Text>
          <Text style={styles.predictionValue}>{item.predictedValue.toFixed(1)}</Text>
          <View style={styles.predictionMeta}>
            <Text style={styles.confidence}>Confidence: {item.confidence.toFixed(1)}%</Text>
            <Text style={styles.timeFrame}>{item.timeFrame}</Text>
          </View>
        </View>
      ))}
    </View>
  );

  const renderBehavioralAnalytics = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Behavioral Analytics</Text>
      {behaviorPatterns.map((pattern, index) => (
        <View key={index} style={styles.behaviorCard}>
          <Text style={styles.behaviorPattern}>{pattern.pattern}</Text>
          <Text style={styles.behaviorFrequency}>Frequency: {pattern.frequency.toFixed(1)}%</Text>
          <Text style={styles.behaviorTrend}>Trend: {pattern.trend}</Text>
        </View>
      ))}
    </View>
  );

  const renderAnomalyDetection = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Anomaly Detection</Text>
      {anomalies.map((anomaly, index) => (
        <View key={index} style={[styles.anomalyCard, getSeverityStyle(anomaly.severity)]}>
          <Text style={styles.anomalyType}>{anomaly.type}</Text>
          <Text style={styles.anomalyDescription}>{anomaly.description}</Text>
          <View style={styles.anomalyMeta}>
            <Text style={styles.anomalySeverity}>Severity: {anomaly.severity}</Text>
            <Text style={styles.anomalyTime}>
              {new Date(anomaly.detectedAt).toLocaleString()}
            </Text>
          </View>
        </View>
      ))}
    </View>
  );

  const renderRealTimeAnalytics = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Real-Time Analytics</Text>
      {realTimeData && (
        <View style={styles.realTimeContainer}>
          <View style={styles.realTimeCard}>
            <Text style={styles.realTimeLabel}>Currently Present</Text>
            <Text style={styles.realTimeValue}>{realTimeData.currentlyPresent}</Text>
          </View>
          <View style={styles.realTimeCard}>
            <Text style={styles.realTimeLabel}>Total Employees</Text>
            <Text style={styles.realTimeValue}>{realTimeData.totalEmployees}</Text>
          </View>
          <View style={styles.realTimeCard}>
            <Text style={styles.realTimeLabel}>Attendance Rate</Text>
            <Text style={styles.realTimeValue}>{realTimeData.attendanceRate.toFixed(1)}%</Text>
          </View>
          <View style={styles.realTimeCard}>
            <Text style={styles.realTimeLabel}>Today's Check-ins</Text>
            <Text style={styles.realTimeValue}>{realTimeData.todayCheckIns}</Text>
          </View>
        </View>
      )}
    </View>
  );

  const getSeverityStyle = (severity: string) => {
    switch (severity.toLowerCase()) {
      case 'high':
        return { borderLeftColor: Colors.error, borderLeftWidth: 4 };
      case 'medium':
        return { borderLeftColor: Colors.warning, borderLeftWidth: 4 };
      default:
        return { borderLeftColor: Colors.success, borderLeftWidth: 4 };
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={Colors.primary} />
        <Text style={styles.loadingText}>Loading Advanced Analytics...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Advanced Analytics</Text>
      
      <View style={styles.tabBar}>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'predictive' && styles.activeTab]}
          onPress={() => setSelectedTab('predictive')}
        >
          <Text style={[styles.tabText, selectedTab === 'predictive' && styles.activeTabText]}>
            Predictive
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'behavioral' && styles.activeTab]}
          onPress={() => setSelectedTab('behavioral')}
        >
          <Text style={[styles.tabText, selectedTab === 'behavioral' && styles.activeTabText]}>
            Behavioral
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'anomaly' && styles.activeTab]}
          onPress={() => setSelectedTab('anomaly')}
        >
          <Text style={[styles.tabText, selectedTab === 'anomaly' && styles.activeTabText]}>
            Anomaly
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'realtime' && styles.activeTab]}
          onPress={() => setSelectedTab('realtime')}
        >
          <Text style={[styles.tabText, selectedTab === 'realtime' && styles.activeTabText]}>
            Real-time
          </Text>
        </TouchableOpacity>
      </View>

      <ScrollView style={styles.content}>
        {selectedTab === 'predictive' && renderPredictiveAnalytics()}
        {selectedTab === 'behavioral' && renderBehavioralAnalytics()}
        {selectedTab === 'anomaly' && renderAnomalyDetection()}
        {selectedTab === 'realtime' && renderRealTimeAnalytics()}
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
  predictionCard: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  predictionMetric: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
    color: Colors.text,
  },
  predictionValue: {
    fontSize: 24,
    fontWeight: 'bold',
    color: Colors.primary,
    marginBottom: 8,
  },
  predictionMeta: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  confidence: {
    fontSize: 12,
    color: Colors.success,
  },
  timeFrame: {
    fontSize: 12,
    color: Colors.textSecondary,
  },
  behaviorCard: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  behaviorPattern: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
    color: Colors.text,
  },
  behaviorFrequency: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 4,
  },
  behaviorTrend: {
    fontSize: 14,
    color: Colors.primary,
  },
  anomalyCard: {
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  anomalyType: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
    color: Colors.text,
  },
  anomalyDescription: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 12,
  },
  anomalyMeta: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  anomalySeverity: {
    fontSize: 12,
    fontWeight: 'bold',
    color: Colors.error,
  },
  anomalyTime: {
    fontSize: 12,
    color: Colors.textSecondary,
  },
  realTimeContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
  },
  realTimeCard: {
    width: '48%',
    backgroundColor: 'white',
    padding: 16,
    marginBottom: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
    alignItems: 'center',
  },
  realTimeLabel: {
    fontSize: 12,
    color: Colors.textSecondary,
    marginBottom: 8,
  },
  realTimeValue: {
    fontSize: 20,
    fontWeight: 'bold',
    color: Colors.primary,
  },
});

export default AdvancedAnalyticsScreen;
