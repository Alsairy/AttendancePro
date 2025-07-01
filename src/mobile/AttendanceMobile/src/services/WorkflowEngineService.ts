import { ApiService } from './ApiService';

export interface WorkflowDefinition {
  id: string;
  name: string;
  description: string;
  tenantId: string;
  isActive: boolean;
  version: number;
  createdAt: string;
}

export interface WorkflowInstance {
  id: string;
  workflowDefinitionId: string;
  status: string;
  startedAt: string;
  completedAt?: string;
  currentStep: string;
  workflowName?: string;
  parameters?: Record<string, any>;
}

export interface WorkflowTask {
  id: string;
  name: string;
  description: string;
  status: string;
  createdAt: string;
  dueDate?: string;
  priority: string;
}

export interface WorkflowExecutionReport {
  instanceId: string;
  workflowName: string;
  status: string;
  startedAt: string;
  completedAt?: string;
  duration?: string;
  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;
  failedTasks: number;
}

export class WorkflowEngineService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async createWorkflow(workflow: Partial<WorkflowDefinition>): Promise<WorkflowDefinition> {
    try {
      const response = await this.apiService.post('/workflow-engine/definitions', workflow);
      return response.data;
    } catch (error) {
      throw new Error('Failed to create workflow');
    }
  }

  async startWorkflow(workflowId: string, parameters: Record<string, any>): Promise<WorkflowInstance> {
    try {
      const response = await this.apiService.post(`/workflow-engine/instances/start?workflowId=${workflowId}`, parameters);
      return response.data;
    } catch (error) {
      throw new Error('Failed to start workflow');
    }
  }

  async getWorkflowInstance(instanceId: string): Promise<WorkflowInstance> {
    try {
      const response = await this.apiService.get(`/workflow-engine/instances/${instanceId}`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get workflow instance');
    }
  }

  async getActiveWorkflows(): Promise<WorkflowInstance[]> {
    try {
      const response = await this.apiService.get('/workflow-engine/instances/active');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get active workflows');
    }
  }

  async completeTask(taskId: string, outputs: Record<string, any>): Promise<boolean> {
    try {
      await this.apiService.post(`/workflow-engine/tasks/${taskId}/complete`, outputs);
      return true;
    } catch (error) {
      return false;
    }
  }

  async cancelWorkflow(instanceId: string): Promise<boolean> {
    try {
      await this.apiService.post(`/workflow-engine/instances/${instanceId}/cancel`);
      return true;
    } catch (error) {
      return false;
    }
  }

  async getPendingTasks(): Promise<WorkflowTask[]> {
    try {
      const response = await this.apiService.get('/workflow-engine/tasks/pending');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get pending tasks');
    }
  }

  async getWorkflowDefinitions(): Promise<WorkflowDefinition[]> {
    try {
      const response = await this.apiService.get('/workflow-engine/definitions');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get workflow definitions');
    }
  }

  async getExecutionReport(instanceId: string): Promise<WorkflowExecutionReport> {
    try {
      const response = await this.apiService.get(`/workflow-engine/instances/${instanceId}/report`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get execution report');
    }
  }

  async updateWorkflowDefinition(workflowId: string, workflow: Partial<WorkflowDefinition>): Promise<boolean> {
    try {
      await this.apiService.put(`/workflow-engine/definitions/${workflowId}`, workflow);
      return true;
    } catch (error) {
      return false;
    }
  }
}
