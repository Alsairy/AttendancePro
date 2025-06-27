#!/bin/bash


set -euo pipefail

# Configuration
NAMESPACE="hudur-platform"
ROLLBACK_TIMEOUT="300s"
HEALTH_CHECK_TIMEOUT="60s"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING: $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR: $1${NC}"
}

get_current_version() {
    kubectl get configmap hudur-version-control -n $NAMESPACE -o jsonpath='{.data.current-version}' 2>/dev/null || echo "unknown"
}

get_previous_version() {
    kubectl get configmap hudur-version-control -n $NAMESPACE -o jsonpath='{.data.previous-version}' 2>/dev/null || echo "unknown"
}

is_rollback_enabled() {
    local enabled=$(kubectl get configmap hudur-version-control -n $NAMESPACE -o jsonpath='{.data.rollback-enabled}' 2>/dev/null || echo "false")
    [[ "$enabled" == "true" ]]
}

perform_health_checks() {
    log "Performing health checks..."
    
    local running_pods=$(kubectl get pods -n $NAMESPACE -l app.kubernetes.io/part-of=hudur-platform --field-selector=status.phase=Running --no-headers | wc -l)
    local total_pods=$(kubectl get pods -n $NAMESPACE -l app.kubernetes.io/part-of=hudur-platform --no-headers | wc -l)
    
    if [[ $running_pods -lt $total_pods ]]; then
        error "Not all pods are running ($running_pods/$total_pods)"
        return 1
    fi
    
    if kubectl get deployment hudur-api-gateway -n $NAMESPACE >/dev/null 2>&1; then
        local api_ready=$(kubectl get deployment hudur-api-gateway -n $NAMESPACE -o jsonpath='{.status.readyReplicas}')
        local api_desired=$(kubectl get deployment hudur-api-gateway -n $NAMESPACE -o jsonpath='{.spec.replicas}')
        
        if [[ "$api_ready" != "$api_desired" ]]; then
            error "API Gateway is not fully ready ($api_ready/$api_desired)"
            return 1
        fi
    fi
    
    log "Health checks passed"
    return 0
}

backup_current_state() {
    local version=$1
    log "Creating backup of current state (version: $version)..."
    
    kubectl create job hudur-emergency-backup-$(date +%s) \
        --from=job/hudur-version-backup \
        -n $NAMESPACE \
        --dry-run=client -o yaml | \
        sed "s/hudur-version-backup/hudur-emergency-backup-$(date +%s)/" | \
        kubectl apply -f -
    
    local backup_job="hudur-emergency-backup-$(date +%s)"
    kubectl wait --for=condition=complete job/$backup_job -n $NAMESPACE --timeout=300s
    
    log "Emergency backup completed"
}

rollback_database() {
    local target_version=$1
    log "Rolling back database to version $target_version..."
    
    local backup_file="hudur-db-backup-$target_version.sql"
    
    cat <<EOF | kubectl apply -f -
apiVersion: batch/v1
kind: Job
metadata:
  name: hudur-db-rollback-$(date +%s)
  namespace: $NAMESPACE
spec:
  template:
    spec:
      restartPolicy: OnFailure
      containers:
      - name: db-rollback
        image: postgres:15
        env:
        - name: DATABASE_CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: hudur-secrets
              key: database-connection-string
        - name: STORAGE_CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: hudur-secrets
              key: blob-storage-connection-string
        - name: TARGET_VERSION
          value: "$target_version"
        command:
        - /bin/bash
        - -c
        - |
          echo "Downloading database backup for version \$TARGET_VERSION"
          az storage blob download \\
            --connection-string "\$STORAGE_CONNECTION_STRING" \\
            --container-name version-backups \\
            --name "hudur-db-backup-\$TARGET_VERSION.sql" \\
            --file /tmp/rollback.sql
          
          echo "Restoring database from backup"
          psql "\$DATABASE_CONNECTION_STRING" < /tmp/rollback.sql
          
          echo "Database rollback completed"
EOF
    
    local db_job="hudur-db-rollback-$(date +%s)"
    kubectl wait --for=condition=complete job/$db_job -n $NAMESPACE --timeout=600s
    
    log "Database rollback completed"
}

rollback_deployments() {
    local target_version=$1
    log "Rolling back deployments to version $target_version..."
    
    local deployments=$(kubectl get deployments -n $NAMESPACE -l app.kubernetes.io/part-of=hudur-platform -o name)
    
    for deployment in $deployments; do
        local deployment_name=$(echo $deployment | cut -d'/' -f2)
        log "Rolling back $deployment_name..."
        
        kubectl patch $deployment -n $NAMESPACE -p "{\"spec\":{\"template\":{\"spec\":{\"containers\":[{\"name\":\"${deployment_name}\",\"image\":\"hudurprodacr.azurecr.io/hudur/${deployment_name}:${target_version}\"}]}}}}"
        
        kubectl rollout status $deployment -n $NAMESPACE --timeout=$ROLLBACK_TIMEOUT
    done
    
    log "Deployment rollback completed"
}

update_version_config() {
    local target_version=$1
    local current_version=$2
    
    log "Updating version configuration..."
    
    kubectl patch configmap hudur-version-control -n $NAMESPACE --type merge -p "{\"data\":{\"current-version\":\"$target_version\",\"previous-version\":\"$current_version\"}}"
    
    log "Version configuration updated"
}

send_notification() {
    local target_version=$1
    local current_version=$2
    local status=$3
    
    local webhook_url=$(kubectl get configmap hudur-rollback-config -n $NAMESPACE -o jsonpath='{.data.rollback-notification-webhook}' 2>/dev/null || echo "")
    
    if [[ -n "$webhook_url" ]]; then
        local message="Hudur Platform Rollback $status: $current_version → $target_version"
        curl -X POST -H 'Content-type: application/json' \
            --data "{\"text\":\"$message\"}" \
            "$webhook_url" || warn "Failed to send notification"
    fi
}

perform_rollback() {
    local target_version=$1
    local current_version=$(get_current_version)
    
    log "Starting rollback from version $current_version to $target_version"
    
    if ! is_rollback_enabled; then
        error "Rollback is disabled in configuration"
        exit 1
    fi
    
    backup_current_state "$current_version"
    
    rollback_database "$target_version"
    rollback_deployments "$target_version"
    update_version_config "$target_version" "$current_version"
    
    log "Waiting for system to stabilize..."
    sleep 30
    
    if perform_health_checks; then
        log "Rollback completed successfully"
        send_notification "$target_version" "$current_version" "SUCCESSFUL"
        return 0
    else
        error "Rollback failed health checks"
        send_notification "$target_version" "$current_version" "FAILED"
        return 1
    fi
}

list_versions() {
    log "Available versions for rollback:"
    kubectl get configmap hudur-version-control -n $NAMESPACE -o jsonpath='{.data.version-history}' | grep -E "^[0-9]+\.[0-9]+\.[0-9]+:" | sed 's/:$//'
}

show_status() {
    local current_version=$(get_current_version)
    local previous_version=$(get_previous_version)
    local rollback_enabled=$(kubectl get configmap hudur-version-control -n $NAMESPACE -o jsonpath='{.data.rollback-enabled}' 2>/dev/null || echo "unknown")
    
    echo "Hudur Platform Version Status:"
    echo "  Current Version: $current_version"
    echo "  Previous Version: $previous_version"
    echo "  Rollback Enabled: $rollback_enabled"
    echo ""
    
    perform_health_checks && echo "System Status: HEALTHY" || echo "System Status: UNHEALTHY"
}

case "${1:-}" in
    "rollback")
        if [[ -z "${2:-}" ]]; then
            error "Target version required. Usage: $0 rollback <version>"
            exit 1
        fi
        perform_rollback "$2"
        ;;
    "list")
        list_versions
        ;;
    "status")
        show_status
        ;;
    "health")
        perform_health_checks && echo "HEALTHY" || echo "UNHEALTHY"
        ;;
    *)
        echo "Hudur Platform Rollback Script"
        echo ""
        echo "Usage: $0 <command> [options]"
        echo ""
        echo "Commands:"
        echo "  rollback <version>  - Rollback to specified version"
        echo "  list               - List available versions"
        echo "  status             - Show current version status"
        echo "  health             - Perform health checks"
        echo ""
        echo "Examples:"
        echo "  $0 rollback 0.9.0"
        echo "  $0 list"
        echo "  $0 status"
        exit 1
        ;;
esac
