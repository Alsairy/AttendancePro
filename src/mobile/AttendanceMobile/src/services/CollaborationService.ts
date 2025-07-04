import { ApiService } from './ApiService';

export interface Team {
  id: string;
  name: string;
  description: string;
  tenantId: string;
  memberCount: number;
  members?: TeamMember[];
}

export interface TeamMember {
  userId: string;
  userName: string;
  role: string;
  joinedAt: string;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  tenantId: string;
  teamId?: string;
  status: string;
  startDate?: string;
  endDate?: string;
  taskCount: number;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  projectId: string;
  assignedUserId?: string;
  status: string;
  priority: string;
  dueDate?: string;
}

export interface ChatMessage {
  id: string;
  teamId: string;
  userId: string;
  content: string;
  messageType: string;
  createdAt: string;
}

export class CollaborationService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async getTeams(): Promise<Team[]> {
    try {
      const response = await this.apiService.get('/collaboration/teams');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get teams');
    }
  }

  async getTeam(teamId: string): Promise<Team> {
    try {
      const response = await this.apiService.get(`/collaboration/teams/${teamId}`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get team');
    }
  }

  async createTeam(team: Partial<Team>): Promise<Team> {
    try {
      const response = await this.apiService.post('/collaboration/teams', team);
      return response.data;
    } catch (error) {
      throw new Error('Failed to create team');
    }
  }

  async addTeamMember(teamId: string, userId: string): Promise<boolean> {
    try {
      await this.apiService.post(`/collaboration/teams/${teamId}/members?userId=${userId}`);
      return true;
    } catch (error) {
      return false;
    }
  }

  async removeTeamMember(teamId: string, userId: string): Promise<boolean> {
    try {
      await this.apiService.delete(`/collaboration/teams/${teamId}/members/${userId}`);
      return true;
    } catch (error) {
      return false;
    }
  }

  async getProjects(): Promise<Project[]> {
    try {
      const response = await this.apiService.get('/collaboration/projects');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get projects');
    }
  }

  async createProject(project: Partial<Project>): Promise<Project> {
    try {
      const response = await this.apiService.post('/collaboration/projects', project);
      return response.data;
    } catch (error) {
      throw new Error('Failed to create project');
    }
  }

  async getTasks(projectId: string): Promise<Task[]> {
    try {
      const response = await this.apiService.get(`/collaboration/projects/${projectId}/tasks`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get tasks');
    }
  }

  async createTask(task: Partial<Task>): Promise<Task> {
    try {
      const response = await this.apiService.post('/collaboration/tasks', task);
      return response.data;
    } catch (error) {
      throw new Error('Failed to create task');
    }
  }

  async updateTaskStatus(taskId: string, status: string): Promise<boolean> {
    try {
      await this.apiService.put(`/collaboration/tasks/${taskId}/status`, status);
      return true;
    } catch (error) {
      return false;
    }
  }

  async getMessages(teamId: string): Promise<ChatMessage[]> {
    try {
      const response = await this.apiService.get(`/collaboration/teams/${teamId}/messages`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get messages');
    }
  }

  async sendMessage(message: Partial<ChatMessage>): Promise<ChatMessage> {
    try {
      const response = await this.apiService.post('/collaboration/messages', message);
      return response.data;
    } catch (error) {
      throw new Error('Failed to send message');
    }
  }

  async getDocuments(projectId: string): Promise<any[]> {
    try {
      const response = await this.apiService.get(`/collaboration/projects/${projectId}/documents`);
      return response.data;
    } catch (error) {
      throw new Error('Failed to get documents');
    }
  }

  async createDocument(document: any): Promise<any> {
    try {
      const response = await this.apiService.post('/collaboration/documents', document);
      return response.data;
    } catch (error) {
      throw new Error('Failed to create document');
    }
  }

  async shareDocument(documentId: string, userIds: string[]): Promise<boolean> {
    try {
      await this.apiService.post(`/collaboration/documents/${documentId}/share`, userIds);
      return true;
    } catch (error) {
      return false;
    }
  }
}
