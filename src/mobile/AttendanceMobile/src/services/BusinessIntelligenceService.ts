import { ApiService } from './ApiService';

export interface BusinessIntelligenceReport {
  tenantId: string;
  generatedAt: string;
  totalEmployees: number;
  attendanceRate: number;
  productivityScore: number;
  costPerEmployee: number;
  revenuePerEmployee: number;
  employeeTurnoverRate: number;
  employeeSatisfactionScore: number;
  complianceScore: number;
}

export interface KpiMetric {
  name: string;
  value: number;
  target: number;
  unit: string;
}

export interface TrendAnalysis {
  date: string;
  metric: string;
  value: number;
  trendDirection: string;
}

export interface PredictiveInsight {
  category: string;
  prediction: string;
  confidence: number;
  impact: string;
  recommendedAction: string;
}

export class BusinessIntelligenceService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async getExecutiveDashboard(): Promise<BusinessIntelligenceReport> {
    try {
      const response = await this.apiService.get('/business-intelligence/executive-dashboard');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get executive dashboard');
    }
  }

  async getKpiMetrics(): Promise<KpiMetric[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/kpi-metrics');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get KPI metrics');
    }
  }

  async getTrendAnalysis(fromDate: string, toDate: string): Promise<TrendAnalysis[]> {
    try {
      const response = await this.apiService.get(
        `/business-intelligence/trend-analysis?fromDate=${fromDate}&toDate=${toDate}`
      );
      return response.data;
    } catch (error) {
      throw new Error('Failed to get trend analysis');
    }
  }

  async getPredictiveInsights(): Promise<PredictiveInsight[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/predictive-insights');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get predictive insights');
    }
  }

  async getBenchmarkComparison(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/benchmark-comparison');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get benchmark comparison');
    }
  }

  async getRiskAssessment(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/risk-assessment');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get risk assessment');
    }
  }

  async getOpportunityAnalysis(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/opportunity-analysis');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get opportunity analysis');
    }
  }

  async getPerformanceMetrics(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/performance-metrics');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get performance metrics');
    }
  }

  async getResourceOptimization(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/resource-optimization');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get resource optimization');
    }
  }

  async getCompetitiveAnalysis(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/business-intelligence/competitive-analysis');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get competitive analysis');
    }
  }
}
