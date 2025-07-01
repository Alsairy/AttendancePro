import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, Alert } from 'react-native';
import { ComplianceService } from '../services/ComplianceService';
import Button from '../components/ui/Button';

interface ComplianceReport {
  id: string;
  title: string;
  status: string;
  score: number;
  lastUpdated: Date;
  framework: string;
}

export const ComplianceScreen: React.FC = () => {
  const [reports, setReports] = useState<ComplianceReport[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadComplianceReports();
  }, []);

  const loadComplianceReports = async () => {
    try {
      const complianceService = new ComplianceService();
      const [gdpr, saudiPdpl, iso27001, soc2, owasp] = await Promise.all([
        complianceService.getGdprCompliance(),
        complianceService.getSaudiPdplCompliance(),
        complianceService.getIso27001Compliance(),
        complianceService.getSoc2Compliance(),
        complianceService.getOwaspCompliance()
      ]);

      const reports = [
        { id: '1', title: 'GDPR Compliance Report', status: gdpr.complianceStatus, score: gdpr.overallScore, lastUpdated: new Date(gdpr.lastAssessment), framework: 'GDPR' },
        { id: '2', title: 'Saudi PDPL Compliance Report', status: saudiPdpl.complianceStatus, score: saudiPdpl.overallScore, lastUpdated: new Date(saudiPdpl.lastAssessment), framework: 'Saudi PDPL' },
        { id: '3', title: 'ISO 27001 Compliance Report', status: iso27001.complianceStatus, score: iso27001.overallScore, lastUpdated: new Date(iso27001.lastAudit), framework: 'ISO 27001' },
        { id: '4', title: 'SOC 2 Compliance Report', status: soc2.complianceStatus, score: soc2.overallScore, lastUpdated: new Date(soc2.lastExamination), framework: 'SOC 2' },
        { id: '5', title: 'OWASP Compliance Report', status: owasp.complianceStatus, score: owasp.overallScore, lastUpdated: new Date(owasp.lastAssessment), framework: 'OWASP' }
      ];

      setReports(reports);
    } catch (error) {
      console.error('Failed to load compliance reports:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleGenerateReport = async (framework: string) => {
    try {
      const complianceService = new ComplianceService();
      await complianceService.generateComplianceAudit();
      Alert.alert('Success', `${framework} compliance report generated`);
      loadComplianceReports();
    } catch (error) {
      Alert.alert('Error', 'Failed to generate report');
    }
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'compliant': return '#10b981';
      case 'warning': return '#f59e0b';
      case 'non-compliant': return '#ef4444';
      default: return '#6b7280';
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <Text>Loading compliance data...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>Global Compliance</Text>
      
      <View style={styles.frameworkContainer}>
        <Text style={styles.sectionTitle}>Compliance Frameworks</Text>
        
        <Button
          title="Generate GDPR Report"
          onPress={() => handleGenerateReport('GDPR')}
        />
        
        <Button
          title="Generate Saudi PDPL Report"
          onPress={() => handleGenerateReport('Saudi PDPL')}
        />
        
        <Button
          title="Generate ISO 27001 Report"
          onPress={() => handleGenerateReport('ISO 27001')}
        />
        
        <Button
          title="Generate SOC 2 Report"
          onPress={() => handleGenerateReport('SOC 2')}
        />
        
        <Button
          title="Generate OWASP Report"
          onPress={() => handleGenerateReport('OWASP')}
        />
        
        <Button
          title="Generate HIPAA Report"
          onPress={() => handleGenerateReport('HIPAA')}
        />
      </View>

      <View style={styles.reportsContainer}>
        <Text style={styles.sectionTitle}>Compliance Reports</Text>
        
        {reports.map((report) => (
          <View key={report.id} style={styles.reportCard}>
            <View style={styles.reportHeader}>
              <Text style={styles.reportTitle}>{report.title}</Text>
              <View style={[styles.statusBadge, { backgroundColor: getStatusColor(report.status) }]}>
                <Text style={styles.statusText}>{report.status}</Text>
              </View>
            </View>
            
            <Text style={styles.framework}>{report.framework}</Text>
            
            <View style={styles.scoreContainer}>
              <Text style={styles.scoreLabel}>Compliance Score:</Text>
              <Text style={[styles.scoreValue, { color: report.score >= 80 ? '#10b981' : '#ef4444' }]}>
                {report.score}%
              </Text>
            </View>
            
            <Text style={styles.lastUpdated}>
              Last Updated: {report.lastUpdated.toLocaleDateString()}
            </Text>
          </View>
        ))}
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    padding: 20,
    color: '#333',
  },
  frameworkContainer: {
    padding: 20,
    gap: 10,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 15,
    color: '#333',
  },
  reportsContainer: {
    padding: 20,
  },
  reportCard: {
    backgroundColor: 'white',
    padding: 15,
    marginBottom: 15,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  reportHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 10,
  },
  reportTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#333',
    flex: 1,
  },
  statusBadge: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 12,
  },
  statusText: {
    color: 'white',
    fontSize: 12,
    fontWeight: '500',
  },
  framework: {
    fontSize: 14,
    color: '#666',
    marginBottom: 10,
  },
  scoreContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 5,
  },
  scoreLabel: {
    fontSize: 14,
    color: '#666',
    marginRight: 10,
  },
  scoreValue: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  lastUpdated: {
    fontSize: 12,
    color: '#999',
  },
});
