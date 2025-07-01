import { ApiService } from './ApiService';

export interface VoiceEnrollmentResult {
  success: boolean;
  templateId?: string;
  quality: number;
  message: string;
}

export interface VoiceVerificationResult {
  isMatch: boolean;
  confidence: number;
  message: string;
}

export interface VoiceCommand {
  command: string;
  description: string;
  category: string;
}

export class VoiceRecognitionService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async startRecording(): Promise<ArrayBuffer> {
    return new Promise((resolve) => {
      setTimeout(() => {
        const mockAudioData = new ArrayBuffer(1024);
        resolve(mockAudioData);
      }, 3000);
    });
  }

  async enrollVoice(audioData: ArrayBuffer): Promise<VoiceEnrollmentResult> {
    try {
      const response = await this.apiService.post('/voice-recognition/enroll', audioData);
      return response.data;
    } catch (error) {
      return {
        success: false,
        quality: 0,
        message: 'Voice enrollment failed',
      };
    }
  }

  async verifyVoice(audioData: ArrayBuffer): Promise<VoiceVerificationResult> {
    try {
      const response = await this.apiService.post('/voice-recognition/verify', audioData);
      return response.data;
    } catch (error) {
      return {
        isMatch: false,
        confidence: 0,
        message: 'Voice verification failed',
      };
    }
  }

  async processVoiceCommand(audioData: ArrayBuffer): Promise<any> {
    try {
      const response = await this.apiService.post('/voice-recognition/command', audioData);
      return response.data;
    } catch (error) {
      throw new Error('Voice command processing failed');
    }
  }

  async getAvailableCommands(): Promise<VoiceCommand[]> {
    try {
      const response = await this.apiService.get('/voice-recognition/commands');
      return response.data;
    } catch (error) {
      return [];
    }
  }

  async getVoiceTemplates(): Promise<any[]> {
    try {
      const response = await this.apiService.get('/voice-recognition/templates');
      return response.data;
    } catch (error) {
      return [];
    }
  }

  async deleteVoiceTemplate(templateId: string): Promise<boolean> {
    try {
      await this.apiService.delete(`/voice-recognition/templates/${templateId}`);
      return true;
    } catch (error) {
      return false;
    }
  }
}
