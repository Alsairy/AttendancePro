import { ApiService } from './ApiService';

export interface IntegrationStatus {
  integrationId: string;
  integrationType: string;
  status: string;
  lastSync: string;
  health: string;
  recordCount: number;
}

export interface IntegrationHealth {
  tenantId: string;
  totalIntegrations: number;
  healthyIntegrations: number;
  overallHealth: number;
  lastHealthCheck: string;
  issues: string[];
}

export interface SapConnection {
  connectionString: string;
  username: string;
  password: string;
  systemId: string;
}

export interface SapIntegration {
  integrationId: string;
  status: string;
  lastSync: string;
  employeeCount: number;
  departmentCount: number;
  message: string;
}

export class EnterpriseIntegrationService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async getIntegrationStatus(): Promise<IntegrationStatus[]> {
    try {
      const response = await this.apiService.get<IntegrationStatus[]>('/enterprise-integration/status');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get integration status');
    }
  }

  async getIntegrationHealth(): Promise<IntegrationHealth> {
    try {
      const response = await this.apiService.get<IntegrationHealth>('/enterprise-integration/health');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get integration health');
    }
  }

  async connectToSap(connection: SapConnection): Promise<SapIntegration> {
    try {
      const response = await this.apiService.post<SapIntegration>('/enterprise-integration/sap', connection);
      return response.data;
    } catch (error) {
      throw new Error('Failed to connect to SAP');
    }
  }

  async connectToOracle(connection: any): Promise<any> {
    try {
      const response = await this.apiService.post('/enterprise-integration/oracle', connection);
      return response.data;
    } catch (error) {
      throw new Error('Failed to connect to Oracle');
    }
  }

  async connectToSalesforce(connection: any): Promise<any> {
    try {
      const response = await this.apiService.post('/enterprise-integration/salesforce', connection);
      return response.data;
    } catch (error) {
      throw new Error('Failed to connect to Salesforce');
    }
  }

  async connectToWorkday(connection: any): Promise<any> {
    try {
      const response = await this.apiService.post('/enterprise-integration/workday', connection);
      return response.data;
    } catch (error) {
      throw new Error('Failed to connect to Workday');
    }
  }

  async connectToBambooHr(connection: any): Promise<any> {
    try {
      const response = await this.apiService.post('/enterprise-integration/bamboohr', connection);
      return response.data;
    } catch (error) {
      throw new Error('Failed to connect to BambooHR');
    }
  }

  async syncData(integrationName: string): Promise<boolean> {
    try {
      await this.apiService.post(`/enterprise-integration/sync/${integrationName}`);
      return true;
    } catch (error) {
      throw new Error(`Failed to sync ${integrationName} data`);
    }
  }

  async disconnectIntegration(integrationName: string): Promise<boolean> {
    try {
      await this.apiService.delete(`/enterprise-integration/${integrationName}`);
      return true;
    } catch (error) {
      throw new Error(`Failed to disconnect ${integrationName}`);
    }
  }

  async getIntegrationLogs(integrationName: string): Promise<any[]> {
    try {
      const response = await this.apiService.get(`/enterprise-integration/logs/${integrationName}`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to get ${integrationName} logs`);
    }
  }
}
