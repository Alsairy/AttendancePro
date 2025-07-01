import { ApiService } from './ApiService';

export interface ProcurementDashboard {
  totalSpend: number;
  monthlySpend: number;
  costSavings: number;
  pendingOrders: number;
  processedOrders: number;
  averageProcessingTime: number;
  budgetUtilization: number;
}

export interface PurchaseOrder {
  id: string;
  orderNumber: string;
  vendorName: string;
  totalAmount: number;
  status: string;
  orderDate: string;
  expectedDelivery: string;
}

export interface VendorAnalysis {
  totalVendors: number;
  activeVendors: number;
  preferredVendors: number;
  averageRating: number;
  totalSpend: number;
  topVendorSpend: number;
  vendorDiversityScore: number;
}

export class ProcurementService {
  private static readonly BASE_URL = '/api/procurement';

  static async getProcurementDashboard(): Promise<ProcurementDashboard> {
    try {
      const response = await ApiService.get(`${this.BASE_URL}/dashboard`);
      return response.data;
    } catch (error) {
      console.error('Error fetching procurement dashboard:', error);
      return {
        totalSpend: 750000,
        monthlySpend: 62500,
        costSavings: 45000,
        pendingOrders: 25,
        processedOrders: 180,
        averageProcessingTime: 3.5,
        budgetUtilization: 68.5,
      };
    }
  }

  static async getPurchaseOrders(): Promise<PurchaseOrder[]> {
    try {
      const response = await ApiService.get(`${this.BASE_URL}/purchase-orders`);
      return response.data;
    } catch (error) {
      console.error('Error fetching purchase orders:', error);
      return [
        {
          id: '1',
          orderNumber: 'PO-2024-001',
          vendorName: 'Tech Solutions Inc',
          totalAmount: 25000,
          status: 'Approved',
          orderDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
          expectedDelivery: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000).toISOString(),
        },
      ];
    }
  }

  static async getVendorAnalysis(): Promise<VendorAnalysis> {
    try {
      const response = await ApiService.get(`${this.BASE_URL}/vendor-analysis`);
      return response.data;
    } catch (error) {
      console.error('Error fetching vendor analysis:', error);
      return {
        totalVendors: 150,
        activeVendors: 120,
        preferredVendors: 25,
        averageRating: 4.2,
        totalSpend: 500000,
        topVendorSpend: 75000,
        vendorDiversityScore: 85.5,
      };
    }
  }

  static async getContractManagement(): Promise<any> {
    try {
      const response = await ApiService.get(`${this.BASE_URL}/contract-management`);
      return response.data;
    } catch (error) {
      console.error('Error fetching contract management:', error);
      return {
        totalContracts: 85,
        activeContracts: 65,
        expiringContracts: 12,
        contractValue: 2500000,
        complianceRate: 94.5,
        renewalRate: 78.0,
      };
    }
  }

  static async getSupplierPerformance(): Promise<any> {
    try {
      const response = await ApiService.get(`${this.BASE_URL}/supplier-performance`);
      return response.data;
    } catch (error) {
      console.error('Error fetching supplier performance:', error);
      return {
        onTimeDeliveryRate: 92.5,
        qualityScore: 88.7,
        costEfficiencyScore: 85.2,
        overallPerformanceScore: 89.1,
        topPerformingSuppliers: 15,
        underperformingSuppliers: 8,
      };
    }
  }

  static async getSpendAnalysis(): Promise<any> {
    try {
      const response = await ApiService.get(`${this.BASE_URL}/spend-analysis`);
      return response.data;
    } catch (error) {
      console.error('Error fetching spend analysis:', error);
      return {
        totalSpend: 1200000,
        directSpend: 800000,
        indirectSpend: 400000,
        spendByCategory: {
          'IT Equipment': 300000,
          'Office Supplies': 150000,
          'Professional Services': 250000,
          'Facilities': 200000,
        },
        spendTrend: 8.5,
      };
    }
  }

  static async createPurchaseOrder(purchaseOrder: any): Promise<boolean> {
    try {
      await ApiService.post(`${this.BASE_URL}/purchase-orders`, purchaseOrder);
      return true;
    } catch (error) {
      console.error('Error creating purchase order:', error);
      throw error;
    }
  }
}
