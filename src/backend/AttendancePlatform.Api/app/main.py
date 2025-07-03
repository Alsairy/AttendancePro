import uvicorn
import os
import sys
from fastapi import FastAPI, HTTPException, Depends, status
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from pydantic import BaseModel
from typing import Optional, List, Dict, Any
import jwt
import bcrypt
from datetime import datetime, timedelta
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Create FastAPI application
app = FastAPI(
    title="Hudur Enterprise Platform API",
    description="Comprehensive Enterprise Attendance and Business Management Platform",
    version="1.0.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

security = HTTPBearer()
JWT_SECRET = "your-secret-key-here"
JWT_ALGORITHM = "HS256"

class LoginRequest(BaseModel):
    email: str
    password: str

class LoginResponse(BaseModel):
    token: str
    user: dict

class UserInfo(BaseModel):
    id: int
    email: str
    name: str
    role: str
    department: Optional[str] = None

TEST_USERS = {
    "admin@test.com": {
        "id": 1,
        "email": "admin@test.com",
        "name": "Admin User",
        "role": "admin",
        "department": "IT",
        "password_hash": bcrypt.hashpw("AdminPassword123!".encode('utf-8'), bcrypt.gensalt()).decode('utf-8')
    },
    "manager@test.com": {
        "id": 2,
        "email": "manager@test.com",
        "name": "Manager User",
        "role": "manager",
        "department": "Operations",
        "password_hash": bcrypt.hashpw("ManagerPassword123!".encode('utf-8'), bcrypt.gensalt()).decode('utf-8')
    },
    "john.doe@test.com": {
        "id": 3,
        "email": "john.doe@test.com",
        "name": "John Doe",
        "role": "employee",
        "department": "Sales",
        "password_hash": bcrypt.hashpw("TestPassword123!".encode('utf-8'), bcrypt.gensalt()).decode('utf-8')
    }
}

def verify_password(plain_password: str, hashed_password: str) -> bool:
    return bcrypt.checkpw(plain_password.encode('utf-8'), hashed_password.encode('utf-8'))

def create_jwt_token(user_data: dict) -> str:
    payload = {
        "user_id": user_data["id"],
        "email": user_data["email"],
        "role": user_data["role"],
        "exp": datetime.utcnow() + timedelta(hours=24)
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

def verify_jwt_token(token: str) -> dict:
    try:
        payload = jwt.decode(token, JWT_SECRET, algorithms=[JWT_ALGORITHM])
        return payload
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")

def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)):
    return verify_jwt_token(credentials.credentials)

@app.get("/")
async def root():
    return {
        "message": "Hudur Enterprise Platform API",
        "version": "1.0.0",
        "status": "running",
        "timestamp": datetime.utcnow().isoformat()
    }

@app.get("/health")
async def health_check():
    return {
        "status": "healthy",
        "timestamp": datetime.utcnow().isoformat(),
        "service": "Hudur Enterprise Platform API"
    }

@app.post("/api/auth/login", response_model=LoginResponse)
async def login(request: LoginRequest):
    user = TEST_USERS.get(request.email)
    if not user:
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    if not verify_password(request.password, user["password_hash"]):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    token = create_jwt_token(user)
    
    user_info = {k: v for k, v in user.items() if k != "password_hash"}
    
    return LoginResponse(
        token=token,
        user=user_info
    )

@app.post("/api/auth/logout")
async def logout():
    return {"message": "Logged out successfully"}

@app.get("/api/auth/me")
async def get_user_info(current_user: dict = Depends(get_current_user)):
    user = TEST_USERS.get(current_user["email"])
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    user_info = {k: v for k, v in user.items() if k != "password_hash"}
    return user_info

@app.post("/api/auth/refresh")
async def refresh_token(current_user: dict = Depends(get_current_user)):
    user = TEST_USERS.get(current_user["email"])
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    new_token = create_jwt_token(user)
    return {"token": new_token}

@app.get("/api/attendance/records")
async def get_attendance_records(current_user: dict = Depends(get_current_user)):
    return {
        "records": [],
        "message": "Attendance records retrieved successfully"
    }

@app.get("/api/users")
async def get_users(current_user: dict = Depends(get_current_user)):
    if current_user["role"] not in ["admin", "manager"]:
        raise HTTPException(status_code=403, detail="Insufficient permissions")
    
    users = []
    for email, user_data in TEST_USERS.items():
        user_info = {k: v for k, v in user_data.items() if k != "password_hash"}
        users.append(user_info)
    
    return {
        "users": users,
        "total": len(users)
    }

@app.get("/api/finance/dashboard")
async def get_finance_dashboard(current_user: dict = Depends(get_current_user)):
    return {
        "revenue": {"current": 1250000, "previous": 1180000, "growth": 5.9},
        "expenses": {"current": 850000, "previous": 820000, "growth": 3.7},
        "profit": {"current": 400000, "previous": 360000, "growth": 11.1},
        "cash_flow": {"current": 320000, "previous": 280000, "growth": 14.3}
    }

@app.get("/api/finance/reports")
async def get_financial_reports(current_user: dict = Depends(get_current_user)):
    return {
        "reports": [
            {"id": 1, "name": "Monthly P&L", "type": "profit_loss", "period": "2024-06"},
            {"id": 2, "name": "Balance Sheet", "type": "balance_sheet", "period": "2024-Q2"},
            {"id": 3, "name": "Cash Flow", "type": "cash_flow", "period": "2024-06"}
        ]
    }

@app.get("/api/finance/budget-analysis")
async def get_budget_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "budget_vs_actual": {
            "revenue": {"budget": 1200000, "actual": 1250000, "variance": 4.2},
            "expenses": {"budget": 900000, "actual": 850000, "variance": -5.6},
            "profit": {"budget": 300000, "actual": 400000, "variance": 33.3}
        }
    }

@app.get("/api/finance/cash-flow-analysis")
async def get_cash_flow_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "operating_cash_flow": 450000,
        "investing_cash_flow": -120000,
        "financing_cash_flow": -80000,
        "net_cash_flow": 250000,
        "cash_position": 1200000
    }

@app.get("/api/finance/profit-loss")
async def get_profit_loss_statement(current_user: dict = Depends(get_current_user)):
    return {
        "revenue": 1250000,
        "cost_of_goods_sold": 500000,
        "gross_profit": 750000,
        "operating_expenses": 350000,
        "operating_income": 400000,
        "net_income": 320000
    }

@app.get("/api/finance/balance-sheet")
async def get_balance_sheet(current_user: dict = Depends(get_current_user)):
    return {
        "assets": {
            "current_assets": 800000,
            "fixed_assets": 1200000,
            "total_assets": 2000000
        },
        "liabilities": {
            "current_liabilities": 300000,
            "long_term_liabilities": 500000,
            "total_liabilities": 800000
        },
        "equity": {
            "shareholders_equity": 1200000,
            "total_equity": 1200000
        }
    }

@app.get("/api/finance/expense-reports")
async def get_expense_reports(current_user: dict = Depends(get_current_user)):
    return {
        "categories": [
            {"category": "Salaries", "amount": 450000, "percentage": 53.0},
            {"category": "Marketing", "amount": 120000, "percentage": 14.1},
            {"category": "Operations", "amount": 180000, "percentage": 21.2},
            {"category": "Technology", "amount": 100000, "percentage": 11.8}
        ],
        "total_expenses": 850000
    }

@app.get("/api/finance/revenue-analysis")
async def get_revenue_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "revenue_streams": [
            {"stream": "Product Sales", "amount": 800000, "percentage": 64.0},
            {"stream": "Services", "amount": 300000, "percentage": 24.0},
            {"stream": "Subscriptions", "amount": 150000, "percentage": 12.0}
        ],
        "total_revenue": 1250000
    }

@app.get("/api/finance/tax-compliance")
async def get_tax_compliance_status(current_user: dict = Depends(get_current_user)):
    return {
        "status": "compliant",
        "next_filing_date": "2024-07-15",
        "estimated_tax_liability": 85000,
        "quarterly_payments": {"q1": 20000, "q2": 22000, "q3": 21000, "q4": 22000}
    }

@app.get("/api/finance/audit-trail")
async def get_financial_audit_trail(current_user: dict = Depends(get_current_user)):
    return {
        "transactions": [
            {"id": 1, "date": "2024-06-01", "type": "revenue", "amount": 50000, "description": "Product sales"},
            {"id": 2, "date": "2024-06-02", "type": "expense", "amount": -15000, "description": "Marketing campaign"}
        ]
    }

@app.get("/api/procurement/dashboard")
async def get_procurement_dashboard(current_user: dict = Depends(get_current_user)):
    return {
        "total_spend": 2500000,
        "active_contracts": 45,
        "pending_approvals": 12,
        "cost_savings": 180000
    }

@app.get("/api/procurement/purchase-orders")
async def get_purchase_orders(current_user: dict = Depends(get_current_user)):
    return {
        "orders": [
            {"id": "PO-001", "vendor": "Tech Solutions Inc", "amount": 25000, "status": "approved"},
            {"id": "PO-002", "vendor": "Office Supplies Co", "amount": 5000, "status": "pending"},
            {"id": "PO-003", "vendor": "Marketing Agency", "amount": 15000, "status": "delivered"}
        ]
    }

@app.get("/api/procurement/vendor-analysis")
async def get_vendor_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "top_vendors": [
            {"name": "Tech Solutions Inc", "spend": 450000, "performance_score": 92},
            {"name": "Office Supplies Co", "spend": 180000, "performance_score": 88},
            {"name": "Marketing Agency", "spend": 320000, "performance_score": 85}
        ],
        "vendor_diversity": {"certified_minority": 35, "women_owned": 28, "small_business": 42}
    }

@app.get("/api/procurement/contracts")
async def get_contract_management(current_user: dict = Depends(get_current_user)):
    return {
        "active_contracts": 45,
        "expiring_soon": 8,
        "renewal_pipeline": 12,
        "contract_value": 3200000
    }

@app.get("/api/procurement/supplier-performance")
async def get_supplier_performance(current_user: dict = Depends(get_current_user)):
    return {
        "performance_metrics": [
            {"supplier": "Tech Solutions Inc", "on_time_delivery": 95, "quality_score": 92, "cost_efficiency": 88},
            {"supplier": "Office Supplies Co", "on_time_delivery": 88, "quality_score": 90, "cost_efficiency": 85}
        ]
    }

@app.get("/api/procurement/spend-analysis")
async def get_spend_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "spend_by_category": [
            {"category": "Technology", "amount": 800000, "percentage": 32.0},
            {"category": "Professional Services", "amount": 600000, "percentage": 24.0},
            {"category": "Office Supplies", "amount": 400000, "percentage": 16.0},
            {"category": "Marketing", "amount": 700000, "percentage": 28.0}
        ],
        "total_spend": 2500000
    }

@app.get("/api/procurement/rfq")
async def get_request_for_quotations(current_user: dict = Depends(get_current_user)):
    return {
        "active_rfqs": [
            {"id": "RFQ-001", "title": "IT Equipment Procurement", "responses": 5, "deadline": "2024-07-15"},
            {"id": "RFQ-002", "title": "Marketing Services", "responses": 3, "deadline": "2024-07-20"}
        ]
    }

@app.get("/api/procurement/compliance")
async def get_procurement_compliance(current_user: dict = Depends(get_current_user)):
    return {
        "compliance_score": 94,
        "policy_adherence": 96,
        "audit_findings": 2,
        "corrective_actions": 1
    }

@app.get("/api/procurement/cost-savings")
async def get_cost_savings_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "total_savings": 180000,
        "savings_by_initiative": [
            {"initiative": "Vendor Consolidation", "savings": 75000},
            {"initiative": "Contract Renegotiation", "savings": 65000},
            {"initiative": "Process Optimization", "savings": 40000}
        ]
    }

@app.post("/api/procurement/purchase-order")
async def create_purchase_order(current_user: dict = Depends(get_current_user)):
    return {
        "message": "Purchase order created successfully",
        "po_number": "PO-004"
    }

@app.get("/api/hr/dashboard")
async def get_hr_dashboard(current_user: dict = Depends(get_current_user)):
    return {
        "total_employees": 250,
        "new_hires": 12,
        "turnover_rate": 8.5,
        "employee_satisfaction": 4.2
    }

@app.get("/api/hr/employee-lifecycle")
async def get_employee_lifecycle(current_user: dict = Depends(get_current_user)):
    return {
        "onboarding": {"active": 8, "completed": 45},
        "performance_reviews": {"due": 15, "completed": 180},
        "career_development": {"in_progress": 32, "completed": 78},
        "offboarding": {"active": 3, "completed": 12}
    }

@app.get("/api/hr/talent-acquisition")
async def get_talent_acquisition(current_user: dict = Depends(get_current_user)):
    return {
        "open_positions": 18,
        "applications_received": 245,
        "interviews_scheduled": 32,
        "offers_extended": 8,
        "time_to_hire": 28
    }

@app.get("/api/hr/performance-management")
async def get_performance_management(current_user: dict = Depends(get_current_user)):
    return {
        "performance_ratings": {"excellent": 45, "good": 120, "satisfactory": 75, "needs_improvement": 10},
        "goal_completion": 78,
        "development_plans": 85
    }

@app.get("/api/hr/compensation-analysis")
async def get_compensation_analysis(current_user: dict = Depends(get_current_user)):
    return {
        "salary_bands": [
            {"level": "Entry", "min": 45000, "max": 65000, "avg": 55000},
            {"level": "Mid", "min": 65000, "max": 95000, "avg": 80000},
            {"level": "Senior", "min": 95000, "max": 130000, "avg": 112000}
        ],
        "pay_equity_score": 92
    }

@app.get("/api/hr/employee-engagement")
async def get_employee_engagement(current_user: dict = Depends(get_current_user)):
    return {
        "engagement_score": 4.2,
        "survey_participation": 89,
        "key_drivers": ["Career Development", "Work-Life Balance", "Recognition"],
        "action_items": 12
    }

@app.get("/api/hr/succession-planning")
async def get_succession_planning(current_user: dict = Depends(get_current_user)):
    return {
        "key_positions": 25,
        "identified_successors": 18,
        "readiness_levels": {"ready_now": 8, "ready_1_year": 15, "ready_2_years": 22},
        "development_programs": 12
    }

@app.get("/api/hr/learning-development")
async def get_learning_development(current_user: dict = Depends(get_current_user)):
    return {
        "training_programs": 45,
        "completion_rate": 87,
        "skill_assessments": 156,
        "certifications_earned": 78,
        "learning_hours": 2340
    }

@app.get("/api/hr/diversity-inclusion")
async def get_diversity_inclusion(current_user: dict = Depends(get_current_user)):
    return {
        "diversity_metrics": {
            "gender": {"male": 52, "female": 45, "other": 3},
            "ethnicity": {"white": 45, "hispanic": 25, "black": 15, "asian": 12, "other": 3},
            "age_groups": {"under_30": 35, "30_50": 45, "over_50": 20}
        },
        "inclusion_score": 4.1,
        "diversity_initiatives": 8
    }

@app.get("/api/hr/workforce-analytics")
async def get_workforce_analytics(current_user: dict = Depends(get_current_user)):
    return {
        "headcount_trends": [
            {"month": "Jan", "count": 235},
            {"month": "Feb", "count": 240},
            {"month": "Mar", "count": 245},
            {"month": "Apr", "count": 248},
            {"month": "May", "count": 250}
        ],
        "productivity_metrics": {"revenue_per_employee": 500000, "profit_per_employee": 128000}
    }

@app.get("/api/hr/compliance")
async def get_hr_compliance(current_user: dict = Depends(get_current_user)):
    return {
        "compliance_score": 96,
        "policy_acknowledgments": 98,
        "training_completions": 94,
        "audit_findings": 1,
        "corrective_actions": 0
    }

@app.get("/api/supply-chain")
async def get_supply_chain_data(current_user: dict = Depends(get_current_user)):
    return {
        "inventory_levels": 85,
        "supplier_performance": 92,
        "logistics_efficiency": 88,
        "demand_forecast": "stable"
    }

@app.get("/api/business-intelligence")
async def get_business_intelligence_data(current_user: dict = Depends(get_current_user)):
    return {
        "kpis": {"revenue_growth": 12.5, "customer_satisfaction": 4.3, "market_share": 18.2},
        "trends": ["increasing_digital_adoption", "remote_work_preference", "sustainability_focus"]
    }

@app.get("/api/project-management")
async def get_project_management_data(current_user: dict = Depends(get_current_user)):
    return {
        "active_projects": 25,
        "on_time_delivery": 88,
        "budget_adherence": 92,
        "resource_utilization": 85
    }

@app.get("/api/customer-management")
async def get_customer_management_data(current_user: dict = Depends(get_current_user)):
    return {
        "total_customers": 1250,
        "customer_lifetime_value": 45000,
        "churn_rate": 5.2,
        "satisfaction_score": 4.4
    }

@app.get("/api/quality-assurance")
async def get_quality_assurance_data(current_user: dict = Depends(get_current_user)):
    return {
        "quality_score": 96,
        "defect_rate": 0.8,
        "customer_complaints": 12,
        "process_improvements": 18
    }

@app.get("/api/risk-management")
async def get_risk_management_data(current_user: dict = Depends(get_current_user)):
    return {
        "risk_score": "medium",
        "identified_risks": 25,
        "mitigation_plans": 22,
        "compliance_status": "compliant"
    }

@app.get("/api/cybersecurity")
async def get_cybersecurity_data(current_user: dict = Depends(get_current_user)):
    return {
        "security_score": 94,
        "threats_detected": 156,
        "incidents_resolved": 154,
        "vulnerability_assessments": 12
    }

@app.get("/api/ai-ml")
async def get_ai_ml_services(current_user: dict = Depends(get_current_user)):
    return {
        "models_deployed": 8,
        "prediction_accuracy": 94.2,
        "data_processed": "2.5TB",
        "automation_savings": 320000,
        "services": [
            "Predictive Analytics", "Natural Language Processing", "Computer Vision",
            "Recommendation Systems", "Fraud Detection", "Demand Forecasting"
        ]
    }

@app.get("/api/blockchain")
async def get_blockchain_services(current_user: dict = Depends(get_current_user)):
    return {
        "smart_contracts": 15,
        "transactions_processed": 25000,
        "network_uptime": 99.9,
        "cost_reduction": 180000,
        "use_cases": [
            "Supply Chain Transparency", "Digital Identity", "Smart Contracts",
            "Asset Tokenization", "Decentralized Finance", "Audit Trail"
        ]
    }

@app.get("/api/iot")
async def get_iot_services(current_user: dict = Depends(get_current_user)):
    return {
        "connected_devices": 1250,
        "data_points_collected": 5000000,
        "uptime": 98.5,
        "energy_savings": 25,
        "applications": [
            "Smart Building Management", "Asset Tracking", "Environmental Monitoring",
            "Predictive Maintenance", "Energy Optimization", "Security Systems"
        ]
    }

@app.get("/api/quantum-computing")
async def get_quantum_computing_services(current_user: dict = Depends(get_current_user)):
    return {
        "quantum_algorithms": 5,
        "optimization_problems": 12,
        "speedup_achieved": "1000x",
        "research_projects": 3,
        "applications": [
            "Cryptography", "Optimization", "Machine Learning",
            "Financial Modeling", "Drug Discovery", "Supply Chain"
        ]
    }

@app.get("/api/cloud-computing")
async def get_cloud_computing_services(current_user: dict = Depends(get_current_user)):
    return {
        "cloud_adoption": 85,
        "cost_optimization": 35,
        "uptime": 99.95,
        "scalability_factor": 10,
        "services": [
            "Infrastructure as a Service", "Platform as a Service", "Software as a Service",
            "Serverless Computing", "Container Orchestration", "Multi-Cloud Management"
        ]
    }

@app.get("/api/edge-computing")
async def get_edge_computing_services(current_user: dict = Depends(get_current_user)):
    return {
        "edge_nodes": 45,
        "latency_reduction": 75,
        "bandwidth_savings": 60,
        "real_time_processing": 95,
        "applications": [
            "Real-time Analytics", "Autonomous Systems", "AR/VR Applications",
            "Industrial IoT", "Smart Cities", "Content Delivery"
        ]
    }

@app.get("/api/robotics")
async def get_robotics_services(current_user: dict = Depends(get_current_user)):
    return {
        "robots_deployed": 25,
        "automation_rate": 65,
        "productivity_increase": 40,
        "safety_improvements": 85,
        "applications": [
            "Manufacturing Automation", "Warehouse Operations", "Quality Control",
            "Maintenance Tasks", "Customer Service", "Security Patrol"
        ]
    }

@app.get("/api/virtual-reality")
async def get_virtual_reality_services(current_user: dict = Depends(get_current_user)):
    return {
        "vr_applications": 12,
        "training_modules": 25,
        "user_engagement": 92,
        "cost_savings": 150000,
        "use_cases": [
            "Employee Training", "Product Visualization", "Remote Collaboration",
            "Safety Simulations", "Customer Experience", "Design Review"
        ]
    }

@app.get("/api/augmented-reality")
async def get_augmented_reality_services(current_user: dict = Depends(get_current_user)):
    return {
        "ar_applications": 8,
        "maintenance_efficiency": 45,
        "training_effectiveness": 60,
        "error_reduction": 35,
        "applications": [
            "Maintenance Assistance", "Assembly Instructions", "Quality Inspection",
            "Remote Support", "Training Overlays", "Navigation Systems"
        ]
    }

@app.get("/api/voice-recognition")
async def get_voice_recognition_services(current_user: dict = Depends(get_current_user)):
    return {
        "accuracy_rate": 96.5,
        "languages_supported": 25,
        "voice_commands": 150,
        "user_adoption": 78,
        "features": [
            "Voice Commands", "Speech-to-Text", "Voice Authentication",
            "Language Translation", "Voice Analytics", "Conversational AI"
        ]
    }

@app.get("/api/collaboration")
async def get_collaboration_services(current_user: dict = Depends(get_current_user)):
    return {
        "active_users": 250,
        "collaboration_score": 4.3,
        "meeting_efficiency": 85,
        "document_sharing": 95,
        "tools": [
            "Video Conferencing", "Document Collaboration", "Project Management",
            "Team Chat", "Screen Sharing", "Virtual Whiteboard"
        ]
    }

@app.get("/api/comprehensive")
async def get_comprehensive_services(current_user: dict = Depends(get_current_user)):
    return {
        "platform_overview": {
            "total_services": 81,
            "active_modules": 75,
            "user_satisfaction": 4.4,
            "system_uptime": 99.8,
            "data_processed": "10TB/day",
            "transactions": 1000000,
            "integrations": 45,
            "compliance_score": 96
        },
        "business_domains": [
            "Finance & Accounting", "Human Resources", "Procurement", "Supply Chain",
            "Customer Management", "Project Management", "Quality Assurance", "Risk Management",
            "Cybersecurity", "Business Intelligence", "Operations", "Compliance"
        ],
        "technology_stack": [
            "Artificial Intelligence", "Machine Learning", "Blockchain", "IoT",
            "Quantum Computing", "Cloud Computing", "Edge Computing", "Robotics",
            "Virtual Reality", "Augmented Reality", "Voice Recognition", "Collaboration"
        ],
        "enterprise_features": [
            "Multi-tenant Architecture", "Role-based Access Control", "Audit Trail",
            "Data Encryption", "API Management", "Workflow Automation", "Real-time Analytics",
            "Mobile Applications", "Integration Hub", "Disaster Recovery"
        ]
    }

def start():
    """Start the FastAPI application with uvicorn"""
    try:
        port = int(os.environ.get("PORT", 8080))
        logger.info(f"Starting Hudur Enterprise Platform on port {port}")
        uvicorn.run(
            "main:app", 
            host="0.0.0.0", 
            port=port,
            log_level="info",
            reload=False
        )
    except Exception as e:
        logger.error(f"Failed to start application: {e}")
        sys.exit(1)

if __name__ == "__main__":
    start()
