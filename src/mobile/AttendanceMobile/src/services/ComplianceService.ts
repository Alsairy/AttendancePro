import { ApiService } from './ApiService';

export interface GdprCompliance {
  tenantId: string;
  overallScore: number;
  dataProcessingLawfulness: number;
  consentManagement: number;
  dataSubjectRights: number;
  dataProtectionByDesign: number;
  dataBreachNotification: number;
  dataProtectionOfficer: number;
  internationalTransfers: number;
  recordsOfProcessing: number;
  complianceStatus: string;
  lastAssessment: string;
  nextAssessment: string;
  recommendations: string[];
}

export interface SaudiPdplCompliance {
  tenantId: string;
  overallScore: number;
  dataControllerObligations: number;
  dataProcessorRequirements: number;
  consentRequirements: number;
  dataSubjectRights: number;
  crossBorderTransfers: number;
  dataBreachNotification: number;
  localizationRequirements: number;
  complianceStatus: string;
  lastAssessment: string;
  nextAssessment: string;
  recommendations: string[];
}

export interface Iso27001Compliance {
  tenantId: string;
  overallScore: number;
  informationSecurityPolicies: number;
  organizationOfInformationSecurity: number;
  humanResourceSecurity: number;
  assetManagement: number;
  accessControl: number;
  cryptography: number;
  physicalEnvironmentalSecurity: number;
  operationsSecurity: number;
  communicationsSecurity: number;
  systemAcquisition: number;
  supplierRelationships: number;
  incidentManagement: number;
  businessContinuity: number;
  compliance: number;
  complianceStatus: string;
  certificationStatus: string;
  lastAudit: string;
  nextAudit: string;
  recommendations: string[];
}

export interface Soc2Compliance {
  tenantId: string;
  overallScore: number;
  securityCriteria: number;
  availabilityCriteria: number;
  processingIntegrityCriteria: number;
  confidentialityCriteria: number;
  privacyCriteria: number;
  complianceStatus: string;
  reportType: string;
  lastExamination: string;
  nextExamination: string;
  recommendations: string[];
}

export interface OwaspCompliance {
  tenantId: string;
  overallScore: number;
  injectionPrevention: number;
  brokenAuthentication: number;
  sensitiveDataExposure: number;
  xxeProtection: number;
  brokenAccessControl: number;
  securityMisconfiguration: number;
  xssProtection: number;
  insecureDeserialization: number;
  vulnerableComponents: number;
  insufficientLogging: number;
  complianceStatus: string;
  lastAssessment: string;
  nextAssessment: string;
  recommendations: string[];
}

export interface ComplianceAudit {
  tenantId: string;
  auditId: string;
  auditType: string;
  overallScore: number;
  complianceFrameworks: string[];
  passedControls: number;
  failedControls: number;
  totalControls: number;
  criticalFindings: number;
  highFindings: number;
  mediumFindings: number;
  lowFindings: number;
  auditStartDate: string;
  auditEndDate: string;
  nextAuditDate: string;
  auditor: string;
  recommendations: string[];
}

export interface ComplianceViolation {
  id: string;
  framework: string;
  control: string;
  severity: string;
  description: string;
  detectedAt: string;
  status: string;
  remediationSteps: string[];
}

export class ComplianceService {
  private apiService: ApiService;

  constructor() {
    this.apiService = new ApiService();
  }

  async getGdprCompliance(): Promise<GdprCompliance> {
    try {
      const response = await this.apiService.get<GdprCompliance>('/global-compliance/gdpr');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get GDPR compliance');
    }
  }

  async getSaudiPdplCompliance(): Promise<SaudiPdplCompliance> {
    try {
      const response = await this.apiService.get<SaudiPdplCompliance>('/global-compliance/saudi-pdpl');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get Saudi PDPL compliance');
    }
  }

  async getIso27001Compliance(): Promise<Iso27001Compliance> {
    try {
      const response = await this.apiService.get<Iso27001Compliance>('/global-compliance/iso27001');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get ISO 27001 compliance');
    }
  }

  async getSoc2Compliance(): Promise<Soc2Compliance> {
    try {
      const response = await this.apiService.get<Soc2Compliance>('/global-compliance/soc2');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get SOC 2 compliance');
    }
  }

  async getOwaspCompliance(): Promise<OwaspCompliance> {
    try {
      const response = await this.apiService.get<OwaspCompliance>('/global-compliance/owasp');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get OWASP compliance');
    }
  }

  async generateComplianceAudit(): Promise<ComplianceAudit> {
    try {
      const response = await this.apiService.post<ComplianceAudit>('/global-compliance/audit');
      return response.data;
    } catch (error) {
      throw new Error('Failed to generate compliance audit');
    }
  }

  async getComplianceViolations(): Promise<ComplianceViolation[]> {
    try {
      const response = await this.apiService.get<ComplianceViolation[]>('/global-compliance/violations');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get compliance violations');
    }
  }

  async remediateViolation(violationId: string): Promise<boolean> {
    try {
      await this.apiService.post(`/global-compliance/violations/${violationId}/remediate`);
      return true;
    } catch (error) {
      throw new Error('Failed to remediate violation');
    }
  }

  async scheduleComplianceReport(schedule: any): Promise<boolean> {
    try {
      await this.apiService.post('/global-compliance/schedule', schedule);
      return true;
    } catch (error) {
      throw new Error('Failed to schedule compliance report');
    }
  }

  async getComplianceTraining(): Promise<any> {
    try {
      const response = await this.apiService.get('/global-compliance/training');
      return response.data;
    } catch (error) {
      throw new Error('Failed to get compliance training');
    }
  }
}
