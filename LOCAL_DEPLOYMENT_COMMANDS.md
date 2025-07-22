# Hudur AttendancePro Platform - Local Deployment Commands

## Complete Command Sequence for Local Deployment

Follow these commands in order to deploy the Hudur AttendancePro platform locally on your machine.

### Prerequisites
- Git installed
- Docker and Docker Compose installed
- .NET 8 SDK installed
- Node.js (v18+) and npm installed

### Step 1: Clone the Repository
```bash
git clone https://github.com/Alsairy/AttendancePro.git
cd AttendancePro
```

### Step 2: Environment Configuration
```bash
# Copy environment configuration files
cp .env.example .env
cp src/frontend/attendancepro-frontend/.env.example src/frontend/attendancepro-frontend/.env

# Update the main .env file with required values
cat > .env << 'EOF'
# Environment Configuration
NODE_ENV=development
PORT=3000

# Database Configuration
DATABASE_URL=Server=localhost,1433;Database=Hudur;User Id=sa;Password=HudurDB2024!;Encrypt=true;TrustServerCertificate=true
REDIS_URL=redis://localhost:6379
DB_PASSWORD=HudurDB2024!

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-change-this-in-production-2024-hudur-platform
JWT_SECRET_KEY=your-super-secret-jwt-key-change-this-in-production-2024-hudur-platform
JWT_EXPIRES_IN=24h
ENCRYPTION_KEY=32-char-encryption-key-for-hudur-2024
ADMIN_PASSWORD=Admin123!HudurPlatform

# API Configuration
API_BASE_URL=http://localhost:5000
FRONTEND_URL=http://localhost:3000

# External Services
AZURE_FACE_API_KEY=your-azure-face-api-key
AZURE_FACE_ENDPOINT=your-azure-face-endpoint
SENDGRID_API_KEY=your-sendgrid-api-key
TWILIO_ACCOUNT_SID=your-twilio-account-sid
TWILIO_AUTH_TOKEN=your-twilio-auth-token

# Security
CORS_ORIGINS=http://localhost:3000,http://localhost:5173
EOF

# Update frontend environment variables
cat > src/frontend/attendancepro-frontend/.env << 'EOF'
VITE_API_BASE_URL=http://localhost:5001/api
VITE_APP_NAME=Hudur Enterprise Platform
VITE_APP_VERSION=1.0.0
VITE_ENVIRONMENT=development
EOF
```

### Step 3: Start Infrastructure Services
```bash
# Start SQL Server, Redis, and RabbitMQ using Docker Compose
docker-compose up -d sqlserver redis rabbitmq

# Wait for services to be ready (about 30 seconds)
sleep 30

# Verify infrastructure services are running
docker-compose ps
```

### Step 4: Configure Backend Services
```bash
# Navigate to Authentication service and update configuration
cd src/backend/services/Authentication/AttendancePlatform.Authentication.Api

# Update appsettings.Development.json
cat > appsettings.Development.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=Hudur;User Id=sa;Password=HudurDB2024!;Encrypt=true;TrustServerCertificate=true",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "SecretKey": "your-super-secret-jwt-key-change-this-in-production-2024-hudur-platform",
    "Issuer": "AttendancePlatform",
    "Audience": "AttendancePlatform",
    "ExpirationInMinutes": 1440
  },
  "Security": {
    "EncryptionKey": "HudurPlatform2024EncryptionKey32"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
EOF

# Return to project root
cd ../../../../..
```

### Step 5: Restore Backend Dependencies
```bash
# Restore .NET dependencies for the entire solution
dotnet restore Hudur.sln
```

### Step 6: Start Backend Services
```bash
# Start Authentication Service (in a new terminal window/tab)
cd src/backend/services/Authentication/AttendancePlatform.Authentication.Api
dotnet run --urls="http://localhost:5001"

# In another terminal, start other services as needed:
# Attendance Service
cd src/backend/services/Attendance/AttendancePlatform.Attendance.Api
dotnet run --urls="http://localhost:5002"

# User Management Service
cd src/backend/services/UserManagement/AttendancePlatform.UserManagement.Api
dotnet run --urls="http://localhost:5003"

# Tenant Management Service
cd src/backend/services/TenantManagement/AttendancePlatform.TenantManagement.Api
dotnet run --urls="http://localhost:5004"
```

### Step 7: Start Frontend Application
```bash
# In a new terminal window/tab, navigate to frontend directory
cd src/frontend/attendancepro-frontend

# Install frontend dependencies
npm install

# Start the frontend development server
npm run dev
```

### Step 8: Verify Deployment
```bash
# Check infrastructure services status
docker-compose ps

# Test backend health endpoints
curl http://localhost:5001/health  # Authentication Service
curl http://localhost:5002/health  # Attendance Service (if running)
curl http://localhost:5003/health  # User Management Service (if running)
curl http://localhost:5004/health  # Tenant Management Service (if running)

# Check database connectivity
docker exec attendance-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "HudurDB2024!" -C -Q "SELECT COUNT(*) as DatabaseExists FROM sys.databases WHERE name = 'Hudur'"
```

### Step 9: Access the Application
- **Frontend Application**: http://localhost:5173
- **Authentication Service**: http://localhost:5001
- **Attendance Service**: http://localhost:5002 (if running)
- **User Management Service**: http://localhost:5003 (if running)
- **Tenant Management Service**: http://localhost:5004 (if running)

### Alternative: Using Docker Compose for All Services
```bash
# If you prefer to run everything with Docker (requires building images first)
# Build all Docker images
docker-compose build

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f
```

### Troubleshooting Commands
```bash
# View Docker container logs
docker-compose logs sqlserver
docker-compose logs redis
docker-compose logs rabbitmq

# Restart infrastructure services
docker-compose restart sqlserver redis rabbitmq

# Clean up and restart
docker-compose down
docker-compose up -d sqlserver redis rabbitmq

# Check .NET service logs
# (Run in the respective service directory)
dotnet run --verbosity detailed

# Check frontend logs
# (Run in frontend directory)
npm run dev -- --debug
```

### Stopping Services
```bash
# Stop Docker services
docker-compose down

# Stop .NET services: Press Ctrl+C in each terminal running dotnet run

# Stop frontend: Press Ctrl+C in the terminal running npm run dev
```

## Notes
- The platform uses SQL Server on port 1433, Redis on port 6379, and RabbitMQ on port 5672
- Backend services run on ports 5001-5004
- Frontend runs on port 5173
- Make sure all required ports are available before starting services
- For production deployment, update all security keys and passwords in the environment files
