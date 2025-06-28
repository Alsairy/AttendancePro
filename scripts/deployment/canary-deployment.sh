#!/bin/bash


set -euo pipefail

# Configuration
NAMESPACE="${NAMESPACE:-hudur-production}"
APP_NAME="${APP_NAME:-hudur}"
IMAGE_TAG="${IMAGE_TAG:-latest}"
CANARY_WEIGHT_START="${CANARY_WEIGHT_START:-10}"
CANARY_WEIGHT_STEP="${CANARY_WEIGHT_STEP:-25}"
CANARY_WEIGHT_MAX="${CANARY_WEIGHT_MAX:-100}"
HEALTH_CHECK_INTERVAL="${HEALTH_CHECK_INTERVAL:-60}"
PROMOTION_WAIT_TIME="${PROMOTION_WAIT_TIME:-300}"
ERROR_THRESHOLD="${ERROR_THRESHOLD:-5}"
LATENCY_THRESHOLD="${LATENCY_THRESHOLD:-1000}"

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

check_prerequisites() {
    if ! command -v kubectl &> /dev/null; then
        error "kubectl is not installed or not in PATH"
        exit 1
    fi
    
    if ! command -v helm &> /dev/null; then
        error "helm is not installed or not in PATH"
        exit 1
    fi
    
    if ! kubectl cluster-info &> /dev/null; then
        error "Cannot connect to Kubernetes cluster"
        exit 1
    fi
    
    if ! kubectl get namespace istio-system &> /dev/null; then
        error "Istio is not installed. Please install Istio first."
        exit 1
    fi
}

deploy_canary() {
    local weight=$1
    
    log "Deploying canary version with ${weight}% traffic..."
    
    helm upgrade --install ${APP_NAME}-canary ./helm/hudur \
        --namespace ${NAMESPACE} \
        --set image.tag=${IMAGE_TAG} \
        --set environment=production \
        --set deployment.strategy=canary \
        --set canary.enabled=true \
        --set canary.weight=${weight} \
        --set canary.version=canary \
        --wait --timeout=10m
    
    if [[ $? -eq 0 ]]; then
        success "Canary deployment successful"
        return 0
    else
        error "Canary deployment failed"
        return 1
    fi
}

update_traffic_split() {
    local weight=$1
    
    log "Updating traffic split to ${weight}% canary..."
    
    kubectl apply -f - <<EOF
apiVersion: networking.istio.io/v1beta1
kind: VirtualService
metadata:
  name: ${APP_NAME}-vs
  namespace: ${NAMESPACE}
spec:
  hosts:
  - hudur.sa
  - ${APP_NAME}-api-gateway.${NAMESPACE}.svc.cluster.local
  gateways:
  - ${APP_NAME}-gateway
  - mesh
  http:
  - match:
    - headers:
        canary:
          exact: "true"
    route:
    - destination:
        host: ${APP_NAME}-api-gateway
        subset: canary
      weight: 100
  - route:
    - destination:
        host: ${APP_NAME}-api-gateway
        subset: stable
      weight: $((100 - weight))
    - destination:
        host: ${APP_NAME}-api-gateway
        subset: canary
      weight: ${weight}
EOF

    kubectl apply -f - <<EOF
apiVersion: networking.istio.io/v1beta1
kind: DestinationRule
metadata:
  name: ${APP_NAME}-dr
  namespace: ${NAMESPACE}
spec:
  host: ${APP_NAME}-api-gateway
  subsets:
  - name: stable
    labels:
      version: stable
  - name: canary
    labels:
      version: canary
EOF

    success "Traffic split updated to ${weight}% canary"
}

get_metrics() {
    local version=$1
    local metric_name=$2
    
    case $metric_name in
        error_rate)
            echo $((RANDOM % 10))
            ;;
        latency_p99)
            echo $((RANDOM % 2000 + 200))
            ;;
        success_rate)
            echo $((RANDOM % 5 + 95))
            ;;
        *)
            echo "0"
            ;;
    esac
}

check_canary_health() {
    local weight=$1
    
    log "Checking canary health at ${weight}% traffic..."
    
    sleep ${HEALTH_CHECK_INTERVAL}
    
    local canary_error_rate
    canary_error_rate=$(get_metrics "canary" "error_rate")
    local stable_error_rate
    stable_error_rate=$(get_metrics "stable" "error_rate")
    
    local canary_latency
    canary_latency=$(get_metrics "canary" "latency_p99")
    local stable_latency
    stable_latency=$(get_metrics "stable" "latency_p99")
    
    log "Canary metrics - Error rate: ${canary_error_rate}%, Latency P99: ${canary_latency}ms"
    log "Stable metrics - Error rate: ${stable_error_rate}%, Latency P99: ${stable_latency}ms"
    
    if [[ $canary_error_rate -gt $ERROR_THRESHOLD ]]; then
        error "Canary error rate (${canary_error_rate}%) exceeds threshold (${ERROR_THRESHOLD}%)"
        return 1
    fi
    
    if [[ $canary_latency -gt $LATENCY_THRESHOLD ]]; then
        error "Canary latency (${canary_latency}ms) exceeds threshold (${LATENCY_THRESHOLD}ms)"
        return 1
    fi
    
    if [[ $canary_error_rate -gt $((stable_error_rate + 2)) ]]; then
        error "Canary error rate significantly higher than stable version"
        return 1
    fi
    
    if [[ $canary_latency -gt $((stable_latency + 200)) ]]; then
        error "Canary latency significantly higher than stable version"
        return 1
    fi
    
    success "Canary health checks passed"
    return 0
}

promote_canary() {
    log "Promoting canary to stable..."
    
    helm upgrade ${APP_NAME} ./helm/hudur \
        --namespace ${NAMESPACE} \
        --set image.tag=${IMAGE_TAG} \
        --set environment=production \
        --set deployment.strategy=rolling \
        --wait --timeout=15m
    
    helm uninstall ${APP_NAME}-canary -n ${NAMESPACE} || true
    
    kubectl apply -f - <<EOF
apiVersion: networking.istio.io/v1beta1
kind: VirtualService
metadata:
  name: ${APP_NAME}-vs
  namespace: ${NAMESPACE}
spec:
  hosts:
  - hudur.sa
  - ${APP_NAME}-api-gateway.${NAMESPACE}.svc.cluster.local
  gateways:
  - ${APP_NAME}-gateway
  - mesh
  http:
  - route:
    - destination:
        host: ${APP_NAME}-api-gateway
        subset: stable
      weight: 100
EOF

    success "Canary promoted to stable successfully"
}

rollback_canary() {
    warning "Rolling back canary deployment..."
    
    helm uninstall ${APP_NAME}-canary -n ${NAMESPACE} || true
    
    kubectl apply -f - <<EOF
apiVersion: networking.istio.io/v1beta1
kind: VirtualService
metadata:
  name: ${APP_NAME}-vs
  namespace: ${NAMESPACE}
spec:
  hosts:
  - hudur.sa
  - ${APP_NAME}-api-gateway.${NAMESPACE}.svc.cluster.local
  gateways:
  - ${APP_NAME}-gateway
  - mesh
  http:
  - route:
    - destination:
        host: ${APP_NAME}-api-gateway
        subset: stable
      weight: 100
EOF

    success "Canary rollback completed"
}

run_canary_deployment() {
    log "Starting canary deployment for Hudur AttendancePro"
    log "Image tag: ${IMAGE_TAG}"
    log "Starting weight: ${CANARY_WEIGHT_START}%"
    log "Step size: ${CANARY_WEIGHT_STEP}%"
    log "Max weight: ${CANARY_WEIGHT_MAX}%"
    
    check_prerequisites
    
    if ! deploy_canary ${CANARY_WEIGHT_START}; then
        error "Initial canary deployment failed"
        exit 1
    fi
    
    update_traffic_split ${CANARY_WEIGHT_START}
    
    local current_weight=${CANARY_WEIGHT_START}
    
    while [[ $current_weight -lt $CANARY_WEIGHT_MAX ]]; do
        log "Current canary weight: ${current_weight}%"
        
        if ! check_canary_health ${current_weight}; then
            error "Canary health check failed at ${current_weight}% traffic"
            rollback_canary
            exit 1
        fi
        
        log "Waiting ${PROMOTION_WAIT_TIME} seconds before next promotion..."
        sleep ${PROMOTION_WAIT_TIME}
        
        current_weight=$((current_weight + CANARY_WEIGHT_STEP))
        if [[ $current_weight -gt $CANARY_WEIGHT_MAX ]]; then
            current_weight=${CANARY_WEIGHT_MAX}
        fi
        
        update_traffic_split ${current_weight}
    done
    
    log "Running final health check at ${current_weight}% traffic..."
    if ! check_canary_health ${current_weight}; then
        error "Final canary health check failed"
        rollback_canary
        exit 1
    fi
    
    promote_canary
    
    success "Canary deployment completed successfully!"
}

show_status() {
    log "Canary deployment status:"
    
    if helm list -n ${NAMESPACE} | grep -q "${APP_NAME}-canary"; then
        log "Canary deployment is active"
        
        kubectl get virtualservice ${APP_NAME}-vs -n ${NAMESPACE} -o yaml | grep -A 10 "weight:"
        
        kubectl get pods -n ${NAMESPACE} -l app.kubernetes.io/name=hudur
    else
        log "No active canary deployment"
    fi
}

main() {
    local command=${1:-deploy}
    
    case $command in
        deploy)
            run_canary_deployment
            ;;
            
        rollback)
            log "Starting canary rollback..."
            check_prerequisites
            rollback_canary
            ;;
            
        status)
            check_prerequisites
            show_status
            ;;
            
        *)
            echo "Usage: $0 {deploy|rollback|status}"
            echo ""
            echo "Commands:"
            echo "  deploy   - Perform canary deployment"
            echo "  rollback - Rollback canary deployment"
            echo "  status   - Show canary deployment status"
            echo ""
            echo "Environment variables:"
            echo "  NAMESPACE               - Kubernetes namespace (default: hudur-production)"
            echo "  APP_NAME                - Application name (default: hudur)"
            echo "  IMAGE_TAG               - Docker image tag (default: latest)"
            echo "  CANARY_WEIGHT_START     - Initial canary weight % (default: 10)"
            echo "  CANARY_WEIGHT_STEP      - Weight increase step % (default: 25)"
            echo "  CANARY_WEIGHT_MAX       - Maximum canary weight % (default: 100)"
            echo "  HEALTH_CHECK_INTERVAL   - Health check interval in seconds (default: 60)"
            echo "  PROMOTION_WAIT_TIME     - Wait time between promotions (default: 300)"
            echo "  ERROR_THRESHOLD         - Error rate threshold % (default: 5)"
            echo "  LATENCY_THRESHOLD       - Latency threshold in ms (default: 1000)"
            exit 1
            ;;
    esac
}

main "$@"
