#!/bin/bash


set -euo pipefail

# Configuration
NAMESPACE="${NAMESPACE:-hudur-production}"
APP_NAME="${APP_NAME:-hudur}"
IMAGE_TAG="${IMAGE_TAG:-latest}"
HEALTH_CHECK_TIMEOUT="${HEALTH_CHECK_TIMEOUT:-300}"
ROLLBACK_ON_FAILURE="${ROLLBACK_ON_FAILURE:-true}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

check_kubectl() {
    if ! command -v kubectl &> /dev/null; then
        error "kubectl is not installed or not in PATH"
        exit 1
    fi
    
    if ! kubectl cluster-info &> /dev/null; then
        error "Cannot connect to Kubernetes cluster"
        exit 1
    fi
}

check_helm() {
    if ! command -v helm &> /dev/null; then
        error "helm is not installed or not in PATH"
        exit 1
    fi
}

get_active_environment() {
    local current_selector
    current_selector=$(kubectl get service ${APP_NAME}-api-gateway -n ${NAMESPACE} -o jsonpath='{.spec.selector.version}' 2>/dev/null || echo "")
    
    if [[ "$current_selector" == "blue" ]]; then
        echo "blue"
    elif [[ "$current_selector" == "green" ]]; then
        echo "green"
    else
        echo "none"
    fi
}

get_inactive_environment() {
    local active_env
    active_env=$(get_active_environment)
    
    if [[ "$active_env" == "blue" ]]; then
        echo "green"
    elif [[ "$active_env" == "green" ]]; then
        echo "blue"
    else
        echo "blue"  # Default to blue if no active environment
    fi
}

deploy_to_inactive() {
    local inactive_env
    inactive_env=$(get_inactive_environment)
    
    log "Deploying to ${inactive_env} environment..."
    
    kubectl create namespace ${NAMESPACE}-${inactive_env} --dry-run=client -o yaml | kubectl apply -f -
    
    helm upgrade --install ${APP_NAME}-${inactive_env} ./helm/hudur \
        --namespace ${NAMESPACE}-${inactive_env} \
        --set image.tag=${IMAGE_TAG} \
        --set environment=production \
        --set deployment.strategy=blue-green \
        --set deployment.version=${inactive_env} \
        --set ingress.enabled=false \
        --wait --timeout=15m
    
    if [[ $? -eq 0 ]]; then
        success "Successfully deployed to ${inactive_env} environment"
        return 0
    else
        error "Failed to deploy to ${inactive_env} environment"
        return 1
    fi
}

run_health_checks() {
    local env=$1
    local namespace="${NAMESPACE}-${env}"
    
    log "Running health checks for ${env} environment..."
    
    log "Waiting for pods to be ready..."
    if ! kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=hudur -n ${namespace} --timeout=${HEALTH_CHECK_TIMEOUT}s; then
        error "Pods failed to become ready in ${env} environment"
        return 1
    fi
    
    log "Running application health checks..."
    local health_check_passed=true
    
    local services=("authentication" "attendance" "face-recognition" "leave-management" "notifications" "analytics")
    
    for service in "${services[@]}"; do
        log "Checking health of ${service} service..."
        
        if kubectl run health-check-${service}-${env} --image=curlimages/curl --rm -i --restart=Never --namespace=${namespace} -- \
            curl -f --max-time 30 http://hudur-${service}.${namespace}.svc.cluster.local/health; then
            success "${service} service is healthy"
        else
            error "${service} service health check failed"
            health_check_passed=false
        fi
    done
    
    log "Running smoke tests..."
    if kubectl run smoke-test-${env} --image=curlimages/curl --rm -i --restart=Never --namespace=${namespace} -- \
        curl -f --max-time 30 http://hudur-api-gateway.${namespace}.svc.cluster.local/api/health; then
        success "Smoke tests passed"
    else
        error "Smoke tests failed"
        health_check_passed=false
    fi
    
    if [[ "$health_check_passed" == "true" ]]; then
        success "All health checks passed for ${env} environment"
        return 0
    else
        error "Health checks failed for ${env} environment"
        return 1
    fi
}

switch_traffic() {
    local new_env=$1
    local namespace="${NAMESPACE}-${new_env}"
    
    log "Switching traffic to ${new_env} environment..."
    
    kubectl patch service ${APP_NAME}-api-gateway -n ${NAMESPACE} \
        -p "{\"spec\":{\"selector\":{\"version\":\"${new_env}\"}}}"
    
    kubectl patch ingress ${APP_NAME}-ingress -n ${NAMESPACE} \
        -p "{\"spec\":{\"rules\":[{\"host\":\"hudur.sa\",\"http\":{\"paths\":[{\"path\":\"/\",\"pathType\":\"Prefix\",\"backend\":{\"service\":{\"name\":\"${APP_NAME}-api-gateway\",\"port\":{\"number\":80}}}}]}}]}}"
    
    sleep 30
    
    log "Verifying traffic switch..."
    if curl -f --max-time 30 https://hudur.sa/api/health; then
        success "Traffic successfully switched to ${new_env} environment"
        return 0
    else
        error "Failed to verify traffic switch to ${new_env} environment"
        return 1
    fi
}

cleanup_old_environment() {
    local old_env=$1
    local namespace="${NAMESPACE}-${old_env}"
    
    log "Cleaning up ${old_env} environment..."
    
    kubectl scale deployment --all --replicas=0 -n ${namespace}
    
    sleep 60
    
    if [[ "${CLEANUP_OLD_ENV:-false}" == "true" ]]; then
        helm uninstall ${APP_NAME}-${old_env} -n ${namespace} || true
        kubectl delete namespace ${namespace} || true
        success "Cleaned up ${old_env} environment"
    else
        warning "Keeping ${old_env} environment for potential rollback"
    fi
}

rollback() {
    local current_env
    current_env=$(get_active_environment)
    
    if [[ "$current_env" == "none" ]]; then
        error "No active environment found for rollback"
        return 1
    fi
    
    local rollback_env
    if [[ "$current_env" == "blue" ]]; then
        rollback_env="green"
    else
        rollback_env="blue"
    fi
    
    warning "Rolling back from ${current_env} to ${rollback_env}..."
    
    if ! kubectl get namespace ${NAMESPACE}-${rollback_env} &> /dev/null; then
        error "Rollback environment ${rollback_env} does not exist"
        return 1
    fi
    
    if switch_traffic ${rollback_env}; then
        success "Successfully rolled back to ${rollback_env} environment"
        return 0
    else
        error "Failed to rollback to ${rollback_env} environment"
        return 1
    fi
}

main() {
    local command=${1:-deploy}
    
    case $command in
        deploy)
            log "Starting blue-green deployment for Hudur AttendancePro"
            log "Image tag: ${IMAGE_TAG}"
            log "Namespace: ${NAMESPACE}"
            
            check_kubectl
            check_helm
            
            local active_env
            active_env=$(get_active_environment)
            local inactive_env
            inactive_env=$(get_inactive_environment)
            
            log "Current active environment: ${active_env}"
            log "Deploying to inactive environment: ${inactive_env}"
            
            if ! deploy_to_inactive; then
                error "Deployment failed"
                exit 1
            fi
            
            if ! run_health_checks ${inactive_env}; then
                error "Health checks failed"
                if [[ "$ROLLBACK_ON_FAILURE" == "true" ]]; then
                    warning "Cleaning up failed deployment..."
                    helm uninstall ${APP_NAME}-${inactive_env} -n ${NAMESPACE}-${inactive_env} || true
                fi
                exit 1
            fi
            
            if ! switch_traffic ${inactive_env}; then
                error "Traffic switch failed"
                if [[ "$ROLLBACK_ON_FAILURE" == "true" ]]; then
                    warning "Attempting rollback..."
                    rollback
                fi
                exit 1
            fi
            
            if [[ "$active_env" != "none" ]]; then
                cleanup_old_environment ${active_env}
            fi
            
            success "Blue-green deployment completed successfully!"
            success "Application is now running on ${inactive_env} environment"
            ;;
            
        rollback)
            log "Starting rollback process..."
            check_kubectl
            
            if rollback; then
                success "Rollback completed successfully"
            else
                error "Rollback failed"
                exit 1
            fi
            ;;
            
        status)
            check_kubectl
            local active_env
            active_env=$(get_active_environment)
            
            log "Current deployment status:"
            log "Active environment: ${active_env}"
            
            if [[ "$active_env" != "none" ]]; then
                kubectl get pods -n ${NAMESPACE}-${active_env} -l app.kubernetes.io/name=hudur
            fi
            ;;
            
        *)
            echo "Usage: $0 {deploy|rollback|status}"
            echo ""
            echo "Commands:"
            echo "  deploy   - Perform blue-green deployment"
            echo "  rollback - Rollback to previous environment"
            echo "  status   - Show current deployment status"
            echo ""
            echo "Environment variables:"
            echo "  NAMESPACE                - Kubernetes namespace (default: hudur-production)"
            echo "  APP_NAME                 - Application name (default: hudur)"
            echo "  IMAGE_TAG                - Docker image tag (default: latest)"
            echo "  HEALTH_CHECK_TIMEOUT     - Health check timeout in seconds (default: 300)"
            echo "  ROLLBACK_ON_FAILURE      - Rollback on failure (default: true)"
            echo "  CLEANUP_OLD_ENV          - Cleanup old environment (default: false)"
            exit 1
            ;;
    esac
}

main "$@"
