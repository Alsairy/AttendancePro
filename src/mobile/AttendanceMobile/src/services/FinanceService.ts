import { ApiService } from './ApiService';

export interface FinancialDashboard {
  totalRevenue: number;
  totalExpenses: number;
  netProfit: number;
  cashFlow: number;
  budgetUtilization: number;
}

export interface FinancialReport {
  id: string;
  reportType: string;
  period: string;
  totalRevenue: number;
  totalExpenses: number;
  netProfit: number;
  generatedDate: string;
}

export interface BudgetAnalysis {
  totalBudget: number;
  spentAmount: number;
  remainingAmount: number;
  budgetUtilization: number;
  variance: number;
  forecastedSpend: number;
}

export class FinanceService {
  private static readonly BASE_URL = '/api/finance';

  static async getFinancialDashboard(): Promise<FinancialDashboard> {
    try {
      return {
        totalRevenue: 500000,
        totalExpenses: 350000,
        netProfit: 150000,
        cashFlow: 75000,
        budgetUtilization: 68.5,
      };
    } catch (error) {
      console.error('Error fetching financial dashboard:', error);
      return {
        totalRevenue: 500000,
        totalExpenses: 350000,
        netProfit: 150000,
        cashFlow: 75000,
        budgetUtilization: 68.5,
      };
    }
  }

  static async getFinancialReports(): Promise<FinancialReport[]> {
    try {
      return [
        {
          id: '1',
          reportType: 'Monthly P&L',
          period: '2024-06',
          totalRevenue: 150000,
          totalExpenses: 120000,
          netProfit: 30000,
          generatedDate: new Date().toISOString(),
        },
      ];
    } catch (error) {
      console.error('Error fetching financial reports:', error);
      return [
        {
          id: '1',
          reportType: 'Monthly P&L',
          period: '2024-06',
          totalRevenue: 150000,
          totalExpenses: 120000,
          netProfit: 30000,
          generatedDate: new Date().toISOString(),
        },
      ];
    }
  }

  static async getBudgetAnalysis(): Promise<BudgetAnalysis> {
    try {
      return {
        totalBudget: 500000,
        spentAmount: 320000,
        remainingAmount: 180000,
        budgetUtilization: 64.0,
        variance: -20000,
        forecastedSpend: 480000,
      };
    } catch (error) {
      console.error('Error fetching budget analysis:', error);
      return {
        totalBudget: 500000,
        spentAmount: 320000,
        remainingAmount: 180000,
        budgetUtilization: 64.0,
        variance: -20000,
        forecastedSpend: 480000,
      };
    }
  }

  static async getCashFlowAnalysis(): Promise<any> {
    try {
      return {
        openingBalance: 100000,
        cashInflows: 250000,
        cashOutflows: 180000,
        netCashFlow: 70000,
        closingBalance: 170000,
        period: '2024-06',
      };
    } catch (error) {
      console.error('Error fetching cash flow analysis:', error);
      return {
        openingBalance: 100000,
        cashInflows: 250000,
        cashOutflows: 180000,
        netCashFlow: 70000,
        closingBalance: 170000,
        period: '2024-06',
      };
    }
  }

  static async getProfitLossStatement(): Promise<any> {
    try {
      return {
        revenue: 300000,
        costOfGoodsSold: 120000,
        grossProfit: 180000,
        operatingExpenses: 100000,
        operatingIncome: 80000,
        netIncome: 75000,
        period: '2024-06',
      };
    } catch (error) {
      console.error('Error fetching profit loss statement:', error);
      return {
        revenue: 300000,
        costOfGoodsSold: 120000,
        grossProfit: 180000,
        operatingExpenses: 100000,
        operatingIncome: 80000,
        netIncome: 75000,
        period: '2024-06',
      };
    }
  }

  static async getBalanceSheet(): Promise<any> {
    try {
      return {
        totalAssets: 1000000,
        currentAssets: 400000,
        fixedAssets: 600000,
        totalLiabilities: 300000,
        currentLiabilities: 150000,
        longTermLiabilities: 150000,
        totalEquity: 700000,
        asOfDate: new Date().toISOString(),
      };
    } catch (error) {
      console.error('Error fetching balance sheet:', error);
      return {
        totalAssets: 1000000,
        currentAssets: 400000,
        fixedAssets: 600000,
        totalLiabilities: 300000,
        currentLiabilities: 150000,
        longTermLiabilities: 150000,
        totalEquity: 700000,
        asOfDate: new Date().toISOString(),
      };
    }
  }

  static async createFinancialTransaction(transaction: any): Promise<boolean> {
    try {
      return true;
    } catch (error) {
      console.error('Error creating financial transaction:', error);
      throw error;
    }
  }

  static async getExpenseReports(): Promise<any[]> {
    try {
      return [
        {
          id: '1',
          category: 'Travel',
          amount: 5000,
          description: 'Business travel expenses',
          date: new Date().toISOString(),
          status: 'Approved',
        },
      ];
    } catch (error) {
      console.error('Error fetching expense reports:', error);
      return [
        {
          id: '1',
          category: 'Travel',
          amount: 5000,
          description: 'Business travel expenses',
          date: new Date().toISOString(),
          status: 'Approved',
        },
      ];
    }
  }

  static async getRevenueAnalysis(): Promise<any> {
    try {
      return {
        totalRevenue: 500000,
        recurringRevenue: 400000,
        oneTimeRevenue: 100000,
        revenueGrowth: 15.5,
        monthlyRecurringRevenue: 33333,
        averageRevenuePerUser: 250,
      };
    } catch (error) {
      console.error('Error fetching revenue analysis:', error);
      return {
        totalRevenue: 500000,
        recurringRevenue: 400000,
        oneTimeRevenue: 100000,
        revenueGrowth: 15.5,
        monthlyRecurringRevenue: 33333,
        averageRevenuePerUser: 250,
      };
    }
  }

  static async getTaxComplianceStatus(): Promise<any> {
    try {
      return {
        complianceStatus: 'Compliant',
        lastFilingDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
        nextFilingDue: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
        taxLiability: 25000,
        taxesPaid: 20000,
        outstandingAmount: 5000,
      };
    } catch (error) {
      console.error('Error fetching tax compliance status:', error);
      return {
        complianceStatus: 'Compliant',
        lastFilingDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
        nextFilingDue: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
        taxLiability: 25000,
        taxesPaid: 20000,
        outstandingAmount: 5000,
      };
    }
  }

  static async getFinancialAuditTrail(): Promise<any> {
    try {
      return {
        totalTransactions: 1250,
        auditedTransactions: 1200,
        pendingAudits: 50,
        complianceScore: 96.0,
        lastAuditDate: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(),
        nextAuditDue: new Date(Date.now() + 23 * 24 * 60 * 60 * 1000).toISOString(),
      };
    } catch (error) {
      console.error('Error fetching financial audit trail:', error);
      return {
        totalTransactions: 1250,
        auditedTransactions: 1200,
        pendingAudits: 50,
        complianceScore: 96.0,
        lastAuditDate: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(),
        nextAuditDue: new Date(Date.now() + 23 * 24 * 60 * 60 * 1000).toISOString(),
      };
    }
  }
}
