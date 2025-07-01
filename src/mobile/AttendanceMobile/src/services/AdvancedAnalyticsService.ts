import { ApiService } from './ApiService';

export interface PredictiveAnalytics {
  tenantId: string;
  predictions: PredictiveData[];
  modelAccuracy: number;
  lastTrainingDate: string;
  generatedAt: string;
}

export interface PredictiveData {
  metric: string;
  predictedValue: number;
  confidence: number;
  timeFrame: string;
}

export interface BehavioralAnalytics {
  tenantId: string;
  behaviorPatterns: BehaviorPattern[];
  overallEngagement: number;
  workLifeBalance: number;
  generatedAt: string;
}

export interface BehaviorPattern {
  pattern: string;
  frequency: number;
  trend: string;
}

export interface AnomalyDetection {
  tenantId: string;
  anomalies: AnomalyData[];
  totalAnomalies: number;
  criticalAnomalies: number;
  generatedAt: string;
}

export interface AnomalyData {
  type: string;
  description: string;
  severity: string;
  detectedAt: string;
}

export interface RealTimeAnalytics {
  tenantId: string;
  currentlyPresent: number;
  totalEmployees: number;
  attendanceRate: number;
  todayCheckIns: number;
  averageCheckInTime: string;
  lastUpdated: string;
}

export class AdvancedAnalyticsService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async getPredictiveAnalytics(): Promise<PredictiveAnalytics> {
    try {
      const response = await this.apiService.get<PredictiveAnalytics>('/advanced-analytics/predictive');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get predictive analytics');
    }
  }

  async getBehavioralAnalytics(): Promise<BehavioralAnalytics> {
    try {
      const response = await this.apiService.get<BehavioralAnalytics>('/advanced-analytics/behavioral');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get behavioral analytics');
    }
  }

  async getSentimentAnalysis(): Promise<any> {
    try {
      const response = await this.apiService.get('/advanced-analytics/sentiment');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get sentiment analysis');
    }
  }

  async getAnomalyDetection(): Promise<AnomalyDetection> {
    try {
      const response = await this.apiService.get<AnomalyDetection>('/advanced-analytics/anomaly');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get anomaly detection');
    }
  }

  async getForecasting(daysAhead: number = 30): Promise<any> {
    try {
      const response = await this.apiService.get(`/advanced-analytics/forecasting?daysAhead=${daysAhead}`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get forecasting');
    }
  }

  async getCorrelationAnalysis(): Promise<any> {
    try {
      const response = await this.apiService.get('/advanced-analytics/correlation');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get correlation analysis');
    }
  }

  async getClusterAnalysis(): Promise<any> {
    try {
      const response = await this.apiService.get('/advanced-analytics/clustering');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get cluster analysis');
    }
  }

  async getTimeSeriesAnalysis(fromDate: string, toDate: string): Promise<any> {
    try {
      const response = await this.apiService.get(`/advanced-analytics/timeseries?fromDate=${fromDate}&toDate=${toDate}`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get time series analysis');
    }
  }

  async getMachineLearningInsights(): Promise<any> {
    try {
      const response = await this.apiService.get('/advanced-analytics/ml-insights');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get ML insights');
    }
  }

  async getRealTimeAnalytics(): Promise<RealTimeAnalytics> {
    try {
      const response = await this.apiService.get<RealTimeAnalytics>('/advanced-analytics/realtime');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get real-time analytics');
    }
  }
}
