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
import { BusinessIntelligenceService } from '../services/BusinessIntelligenceService';

interface KpiMetric {
  name: string;
  value: number;
  target: number;
  unit: string;
}

interface BusinessReport {
  totalEmployees: number;
  attendanceRate: number;
  productivityScore: number;
  employeeTurnoverRate: number;
  complianceScore: number;
}

const BusinessIntelligenceScreen: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [kpiMetrics, setKpiMetrics] = useState<KpiMetric[]>([]);
  const [businessReport, setBusinessReport] = useState<BusinessReport | null>(null);
  const [selectedTab, setSelectedTab] = useState('overview');
  const biService = new BusinessIntelligenceService();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [metrics, report] = await Promise.all([
        biService.getKpiMetrics(),
        biService.getExecutiveDashboard(),
      ]);
      setKpiMetrics(metrics);
      setBusinessReport(report);
    } catch (error) {
      Alert.alert('Error', 'Failed to load business intelligence data');
    } finally {
      setLoading(false);
    }
  };

  const renderKpiCard = (metric: KpiMetric) => {
    const isOnTarget = metric.value >= metric.target;
    return (
      <View key={metric.name} style={styles.kpiCard}>
        <Text style={styles.kpiName}>{metric.name}</Text>
        <Text style={[styles.kpiValue, isOnTarget ? styles.onTarget : styles.offTarget]}>
          {metric.value.toFixed(1)}{metric.unit}
        </Text>
        <Text style={styles.kpiTarget}>Target: {metric.target}{metric.unit}</Text>
      </View>
    );
  };

  const renderOverview = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Key Performance Indicators</Text>
      <View style={styles.kpiGrid}>
        {kpiMetrics.map(renderKpiCard)}
      </View>
      
      {businessReport && (
        <View style={styles.summaryCard}>
          <Text style={styles.sectionTitle}>Executive Summary</Text>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Total Employees:</Text>
            <Text style={styles.summaryValue}>{businessReport.totalEmployees}</Text>
          </View>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Attendance Rate:</Text>
            <Text style={styles.summaryValue}>{businessReport.attendanceRate.toFixed(1)}%</Text>
          </View>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Productivity Score:</Text>
            <Text style={styles.summaryValue}>{businessReport.productivityScore.toFixed(1)}%</Text>
          </View>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Turnover Rate:</Text>
            <Text style={styles.summaryValue}>{businessReport.employeeTurnoverRate.toFixed(1)}%</Text>
          </View>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Compliance Score:</Text>
            <Text style={styles.summaryValue}>{businessReport.complianceScore.toFixed(1)}%</Text>
          </View>
        </View>
      )}
    </View>
  );

  const renderAnalytics = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Advanced Analytics</Text>
      <View style={styles.analyticsCard}>
        <Text style={styles.cardTitle}>Trend Analysis</Text>
        <Text style={styles.cardDescription}>
          Attendance trends show a 5% improvement over the last quarter
        </Text>
      </View>
      <View style={styles.analyticsCard}>
        <Text style={styles.cardTitle}>Predictive Insights</Text>
        <Text style={styles.cardDescription}>
          Expected 3% increase in productivity next month based on current patterns
        </Text>
      </View>
      <View style={styles.analyticsCard}>
        <Text style={styles.cardTitle}>Risk Assessment</Text>
        <Text style={styles.cardDescription}>
          Low compliance risk detected. Recommended actions: Continue current policies
        </Text>
      </View>
    </View>
  );

  const renderReports = () => (
    <View style={styles.tabContent}>
      <Text style={styles.sectionTitle}>Reports & Benchmarks</Text>
      <TouchableOpacity style={styles.reportButton}>
        <Text style={styles.reportButtonText}>Generate Monthly Report</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.reportButton}>
        <Text style={styles.reportButtonText}>Benchmark Comparison</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.reportButton}>
        <Text style={styles.reportButtonText}>Performance Metrics</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.reportButton}>
        <Text style={styles.reportButtonText}>Resource Optimization</Text>
      </TouchableOpacity>
    </View>
  );

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={Colors.primary} />
        <Text style={styles.loadingText}>Loading Business Intelligence...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Business Intelligence</Text>
      
      <View style={styles.tabBar}>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'overview' && styles.activeTab]}
          onPress={() => setSelectedTab('overview')}
        >
          <Text style={[styles.tabText, selectedTab === 'overview' && styles.activeTabText]}>
            Overview
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'analytics' && styles.activeTab]}
          onPress={() => setSelectedTab('analytics')}
        >
          <Text style={[styles.tabText, selectedTab === 'analytics' && styles.activeTabText]}>
            Analytics
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[styles.tab, selectedTab === 'reports' && styles.activeTab]}
          onPress={() => setSelectedTab('reports')}
        >
          <Text style={[styles.tabText, selectedTab === 'reports' && styles.activeTabText]}>
            Reports
          </Text>
        </TouchableOpacity>
      </View>

      <ScrollView style={styles.content}>
        {selectedTab === 'overview' && renderOverview()}
        {selectedTab === 'analytics' && renderAnalytics()}
        {selectedTab === 'reports' && renderReports()}
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
  kpiGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
    marginBottom: 24,
  },
  kpiCard: {
    width: '48%',
    backgroundColor: 'white',
    padding: 16,
    borderRadius: 8,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  kpiName: {
    fontSize: 14,
    color: Colors.textSecondary,
    marginBottom: 8,
  },
  kpiValue: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  onTarget: {
    color: Colors.success,
  },
  offTarget: {
    color: Colors.warning,
  },
  kpiTarget: {
    fontSize: 12,
    color: Colors.textSecondary,
  },
  summaryCard: {
    backgroundColor: 'white',
    padding: 16,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  summaryRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  summaryLabel: {
    fontSize: 14,
    color: Colors.textSecondary,
  },
  summaryValue: {
    fontSize: 14,
    fontWeight: '600',
    color: Colors.text,
  },
  analyticsCard: {
    backgroundColor: 'white',
    padding: 16,
    borderRadius: 8,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  cardTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
    color: Colors.text,
  },
  cardDescription: {
    fontSize: 14,
    color: Colors.textSecondary,
    lineHeight: 20,
  },
  reportButton: {
    backgroundColor: Colors.primary,
    padding: 16,
    borderRadius: 8,
    marginBottom: 12,
    alignItems: 'center',
  },
  reportButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: '600',
  },
});

export default BusinessIntelligenceScreen;
