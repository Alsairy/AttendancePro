# Required GitHub Secrets Configuration

This document outlines the GitHub repository secrets that must be configured for the CI/CD pipeline to function properly.

## Azure Container Registry Secrets

These secrets are required for Docker image builds and deployments:

- `AZURE_CLIENT_ID`: Azure service principal client ID for ACR authentication
- `AZURE_CLIENT_SECRET`: Azure service principal client secret for ACR authentication  
- `AZURE_CREDENTIALS`: Azure credentials JSON for deployment authentication

## Database Secrets

- `DB_PASSWORD`: Database password for SQL Server connections
- `POSTGRES_PASSWORD`: PostgreSQL password for test database connections

## Security Scanning Secrets

- `GITLEAKS_LICENSE`: License key for GitLeaks secrets scanning (if using commercial version)

## JWT and Encryption Secrets

- `JWT_SECRET_KEY`: Secret key for JWT token signing and validation

## SMTP Configuration Secrets

- `SMTP_PASSWORD`: Password for SMTP email service authentication
- `SMTP_USERNAME`: Username for SMTP email service authentication

## SMS Configuration Secrets

- `SMS_ACCOUNT_SID`: Twilio account SID for SMS notifications
- `SMS_AUTH_TOKEN`: Twilio authentication token for SMS service
- `SMS_FROM_NUMBER`: Phone number for sending SMS notifications

## Push Notification Secrets

- `FIREBASE_SERVER_KEY`: Firebase server key for push notifications
- `FIREBASE_SENDER_ID`: Firebase sender ID for push notifications
- `APPLE_KEY_ID`: Apple Push Notification service key ID
- `APPLE_TEAM_ID`: Apple developer team ID
- `APPLE_BUNDLE_ID`: iOS app bundle identifier
- `APPLE_PRIVATE_KEY`: Apple Push Notification service private key

## Configuration Instructions

1. Navigate to your GitHub repository settings
2. Go to "Secrets and variables" → "Actions"
3. Add each secret listed above with appropriate values
4. Ensure secrets are available to the repository and not restricted to specific environments

## Security Notes

- Never commit these values to the repository
- Use strong, randomly generated passwords and keys
- Rotate secrets regularly according to your security policy
- Limit access to secrets to only necessary personnel

## Azure Service Principal Setup

To create the Azure service principal for ACR access:

```bash
az ad sp create-for-rbac --name "hudur-acr-sp" --role "AcrPush" --scopes "/subscriptions/{subscription-id}/resourceGroups/{resource-group}/providers/Microsoft.ContainerRegistry/registries/hudurprodacr"
```

Use the output to populate `AZURE_CLIENT_ID` and `AZURE_CLIENT_SECRET`.

## Microsoft Graph Configuration Secrets

- `MICROSOFT_GRAPH_CLIENT_ID`: Azure AD application client ID for Microsoft Graph API
- `MICROSOFT_GRAPH_CLIENT_SECRET`: Azure AD application client secret for Microsoft Graph API  
- `MICROSOFT_GRAPH_TENANT_ID`: Azure AD tenant ID for Microsoft Graph API
- `AZURE_FACE_API_ENDPOINT`: Azure Face API endpoint for face recognition service
- `AZURE_FACE_API_KEY`: Azure Face API subscription key
