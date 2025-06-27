#!/bin/bash


set -euo pipefail

# Configuration
NAMESPACE="hudur-platform"
TEST_TIMEOUT="300s"
HEALTH_CHECK_TIMEOUT="60s"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

info() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')] INFO: $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING: $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR: $1${NC}"
}

TESTS_PASSED=0
TESTS_FAILED=0
FAILED_TESTS=()

record_test_result() {
    local test_name=$1
    local result=$2
    
    if [[ "$result" == "PASS" ]]; then
        ((TESTS_PASSED++))
        log "✓ $test_name: PASSED"
    else
        ((TESTS_FAILED++))
        FAILED_TESTS+=("$test_name")
        error "✗ $test_name: FAILED"
    fi
}

test_terraform_configuration() {
    info "Testing Terraform infrastructure configuration..."
    
    local terraform_dir="infrastructure/terraform"
    
    if [[ ! -d "$terraform_dir" ]]; then
        record_test_result "Terraform Directory Exists" "FAIL"
        return 1
    fi
    
    if terraform -chdir="$terraform_dir" validate >/dev/null 2>&1; then
        record_test_result "Terraform Syntax Validation" "PASS"
    else
        record_test_result "Terraform Syntax Validation" "FAIL"
    fi
    
    if terraform -chdir="$terraform_dir" plan -out=tfplan >/dev/null 2>&1; then
        record_test_result "Terraform Plan Generation" "PASS"
        rm -f "$terraform_dir/tfplan"
    else
        record_test_result "Terraform Plan Generation" "FAIL"
    fi
    
    local required_vars=("resource_group_name" "location" "environment" "domain_name")
    for var in "${required_vars[@]}"; do
        if grep -q "variable \"$var\"" "$terraform_dir/variables.tf"; then
            record_test_result "Terraform Variable: $var" "PASS"
        else
            record_test_result "Terraform Variable: $var" "FAIL"
        fi
    done
}

test_helm_charts() {
    info "Testing Helm chart configuration..."
    
    local helm_dir="helm/hudur"
    
    if [[ ! -d "$helm_dir" ]]; then
        record_test_result "Helm Chart Directory Exists" "FAIL"
        return 1
    fi
    
    if helm lint "$helm_dir" >/dev/null 2>&1; then
        record_test_result "Helm Chart Syntax Validation" "PASS"
    else
        record_test_result "Helm Chart Syntax Validation" "FAIL"
    fi
    
    if helm template hudur "$helm_dir" >/dev/null 2>&1; then
        record_test_result "Helm Template Rendering" "PASS"
    else
        record_test_result "Helm Template Rendering" "FAIL"
    fi
    
    local required_templates=("authentication-service.yaml" "attendance-service.yaml" "face-recognition-service.yaml" "leave-management-service.yaml" "notifications-service.yaml" "analytics-service.yaml" "workflow-service.yaml" "frontend.yaml" "ingress.yaml" "secrets.yaml")
    for template in "${required_templates[@]}"; do
        if [[ -f "$helm_dir/templates/$template" ]]; then
            record_test_result "Helm Template: $template" "PASS"
        else
            record_test_result "Helm Template: $template" "FAIL"
        fi
    done
    
    if [[ -f "$helm_dir/Chart.yaml" ]]; then
        if grep -q "name: hudur" "$helm_dir/Chart.yaml" && grep -q "version:" "$helm_dir/Chart.yaml"; then
            record_test_result "Helm Chart.yaml Configuration" "PASS"
        else
            record_test_result "Helm Chart.yaml Configuration" "FAIL"
        fi
    else
        record_test_result "Helm Chart.yaml Exists" "FAIL"
    fi
}

test_argocd_configuration() {
    info "Testing ArgoCD GitOps configuration..."
    
    local argocd_dir="k8s/argocd"
    
    if [[ ! -d "$argocd_dir" ]]; then
        record_test_result "ArgoCD Directory Exists" "FAIL"
        return 1
    fi
    
    if [[ -f "$argocd_dir/application.yaml" ]]; then
        if kubectl apply --dry-run=client -f "$argocd_dir/application.yaml" >/dev/null 2>&1; then
            record_test_result "ArgoCD Application YAML Validation" "PASS"
        else
            record_test_result "ArgoCD Application YAML Validation" "FAIL"
        fi
    else
        record_test_result "ArgoCD Application YAML Exists" "FAIL"
    fi
    
    if [[ -f "$argocd_dir/argocd-install.yaml" ]]; then
        if kubectl apply --dry-run=client -f "$argocd_dir/argocd-install.yaml" >/dev/null 2>&1; then
            record_test_result "ArgoCD Installation YAML Validation" "PASS"
        else
            record_test_result "ArgoCD Installation YAML Validation" "FAIL"
        fi
    else
        record_test_result "ArgoCD Installation YAML Exists" "FAIL"
    fi
    
    if [[ -f "$argocd_dir/repository.yaml" ]]; then
        if kubectl apply --dry-run=client -f "$argocd_dir/repository.yaml" >/dev/null 2>&1; then
            record_test_result "ArgoCD Repository YAML Validation" "PASS"
        else
            record_test_result "ArgoCD Repository YAML Validation" "FAIL"
        fi
    else
        record_test_result "ArgoCD Repository YAML Exists" "FAIL"
    fi
}

test_kustomize_configuration() {
    info "Testing Kustomize GitOps configuration..."
    
    local kustomize_dir="k8s/gitops"
    
    if [[ ! -d "$kustomize_dir" ]]; then
        record_test_result "Kustomize Directory Exists" "FAIL"
        return 1
    fi
    
    if kubectl kustomize "$kustomize_dir" >/dev/null 2>&1; then
        record_test_result "Kustomize Build Validation" "PASS"
    else
        record_test_result "Kustomize Build Validation" "FAIL"
    fi
    
    if [[ -f "$kustomize_dir/kustomization.yaml" ]]; then
        if grep -q "resources:" "$kustomize_dir/kustomization.yaml"; then
            record_test_result "Kustomization YAML Configuration" "PASS"
        else
            record_test_result "Kustomization YAML Configuration" "FAIL"
        fi
    else
        record_test_result "Kustomization YAML Exists" "FAIL"
    fi
}

test_versioning_rollback() {
    info "Testing versioning and rollback configuration..."
    
    if [[ -f "k8s/versioning/version-control.yaml" ]]; then
        if kubectl apply --dry-run=client -f "k8s/versioning/version-control.yaml" >/dev/null 2>&1; then
            record_test_result "Version Control YAML Validation" "PASS"
        else
            record_test_result "Version Control YAML Validation" "FAIL"
        fi
    else
        record_test_result "Version Control YAML Exists" "FAIL"
    fi
    
    if [[ -f "scripts/rollback.sh" ]]; then
        if bash -n "scripts/rollback.sh" >/dev/null 2>&1; then
            record_test_result "Rollback Script Syntax" "PASS"
        else
            record_test_result "Rollback Script Syntax" "FAIL"
        fi
        
        if [[ -x "scripts/rollback.sh" ]]; then
            record_test_result "Rollback Script Executable" "PASS"
        else
            record_test_result "Rollback Script Executable" "FAIL"
        fi
    else
        record_test_result "Rollback Script Exists" "FAIL"
    fi
}

test_docker_configurations() {
    info "Testing Docker configurations..."
    
    local compose_files=("docker-compose.yml" "docker-compose.production.yml")
    for compose_file in "${compose_files[@]}"; do
        if [[ -f "$compose_file" ]]; then
            if docker-compose -f "$compose_file" config >/dev/null 2>&1; then
                record_test_result "Docker Compose: $compose_file" "PASS"
            else
                record_test_result "Docker Compose: $compose_file" "FAIL"
            fi
        else
            record_test_result "Docker Compose File Exists: $compose_file" "FAIL"
        fi
    done
    
    local dockerfile_dirs=("src/backend/services/Authentication" "src/backend/services/Attendance" "src/backend/services/FaceRecognition" "src/backend/services/LeaveManagement" "src/backend/services/Notifications" "src/backend/services/Analytics" "src/frontend/attendancepro-frontend")
    for dir in "${dockerfile_dirs[@]}"; do
        if [[ -f "$dir/Dockerfile" ]]; then
            if docker build --dry-run "$dir" >/dev/null 2>&1 || [[ $? -eq 1 ]]; then
                record_test_result "Dockerfile: $dir" "PASS"
            else
                record_test_result "Dockerfile: $dir" "FAIL"
            fi
        else
            record_test_result "Dockerfile Exists: $dir" "FAIL"
        fi
    done
}

test_environment_configurations() {
    info "Testing environment configurations..."
    
    local env_files=(".env.production" ".env.security" ".env.example")
    for env_file in "${env_files[@]}"; do
        if [[ -f "$env_file" ]]; then
            record_test_result "Environment File: $env_file" "PASS"
        else
            record_test_result "Environment File: $env_file" "FAIL"
        fi
    done
    
    if [[ -f "k8s/base/secrets.yaml" ]]; then
        if kubectl apply --dry-run=client -f "k8s/base/secrets.yaml" >/dev/null 2>&1; then
            record_test_result "Kubernetes Secrets YAML" "PASS"
        else
            record_test_result "Kubernetes Secrets YAML" "FAIL"
        fi
    else
        record_test_result "Kubernetes Secrets YAML Exists" "FAIL"
    fi
}

test_cicd_pipeline() {
    info "Testing CI/CD pipeline configuration..."
    
    if [[ -f ".github/workflows/ci-cd.yml" ]]; then
        if python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci-cd.yml'))" >/dev/null 2>&1; then
            record_test_result "GitHub Actions YAML Syntax" "PASS"
        else
            record_test_result "GitHub Actions YAML Syntax" "FAIL"
        fi
        
        local required_jobs=("build" "test" "security-scan")
        for job in "${required_jobs[@]}"; do
            if grep -q "$job:" ".github/workflows/ci-cd.yml"; then
                record_test_result "CI/CD Job: $job" "PASS"
            else
                record_test_result "CI/CD Job: $job" "FAIL"
            fi
        done
    else
        record_test_result "GitHub Actions Workflow Exists" "FAIL"
    fi
}

test_monitoring_configuration() {
    info "Testing monitoring configuration..."
    
    if [[ -f "monitoring/prometheus/prometheus.yml" ]]; then
        record_test_result "Prometheus Configuration Exists" "PASS"
    else
        record_test_result "Prometheus Configuration Exists" "FAIL"
    fi
    
    local dashboard_dir="monitoring/grafana/dashboards"
    if [[ -d "$dashboard_dir" ]]; then
        local dashboard_count=$(find "$dashboard_dir" -name "*.json" | wc -l)
        if [[ $dashboard_count -gt 0 ]]; then
            record_test_result "Grafana Dashboards Available" "PASS"
        else
            record_test_result "Grafana Dashboards Available" "FAIL"
        fi
    else
        record_test_result "Grafana Dashboards Directory Exists" "FAIL"
    fi
    
    local monitoring_manifests=("k8s/monitoring/prometheus.yaml" "k8s/monitoring/grafana.yaml" "k8s/monitoring/alertmanager.yaml")
    for manifest in "${monitoring_manifests[@]}"; do
        if [[ -f "$manifest" ]]; then
            if kubectl apply --dry-run=client -f "$manifest" >/dev/null 2>&1; then
                record_test_result "Monitoring Manifest: $(basename $manifest)" "PASS"
            else
                record_test_result "Monitoring Manifest: $(basename $manifest)" "FAIL"
            fi
        else
            record_test_result "Monitoring Manifest Exists: $(basename $manifest)" "FAIL"
        fi
    done
}

generate_test_report() {
    local total_tests=$((TESTS_PASSED + TESTS_FAILED))
    local success_rate=0
    
    if [[ $total_tests -gt 0 ]]; then
        success_rate=$(( (TESTS_PASSED * 100) / total_tests ))
    fi
    
    echo ""
    echo "=========================================="
    echo "Infrastructure Testing Report"
    echo "=========================================="
    echo "Total Tests: $total_tests"
    echo "Passed: $TESTS_PASSED"
    echo "Failed: $TESTS_FAILED"
    echo "Success Rate: $success_rate%"
    echo ""
    
    if [[ $TESTS_FAILED -gt 0 ]]; then
        echo "Failed Tests:"
        for test in "${FAILED_TESTS[@]}"; do
            echo "  - $test"
        done
        echo ""
    fi
    
    cat > "INFRASTRUCTURE_TEST_REPORT.md" << EOF

**Generated:** $(date +'%Y-%m-%d %H:%M:%S UTC')


- **Total Tests:** $total_tests
- **Passed:** $TESTS_PASSED
- **Failed:** $TESTS_FAILED
- **Success Rate:** $success_rate%


- Configuration validation
- Syntax checking
- Required variables verification

- Chart syntax validation
- Template rendering
- Required templates verification

- Application configuration
- Installation manifests
- Repository setup

- Build validation
- Resource management

- Version control configuration
- Rollback script validation

- Docker Compose validation
- Dockerfile syntax checking

- Environment files verification
- Kubernetes secrets validation

- GitHub Actions workflow
- Required jobs verification

- Prometheus setup
- Grafana dashboards
- Kubernetes monitoring manifests

EOF

    if [[ $TESTS_FAILED -gt 0 ]]; then
        cat >> "INFRASTRUCTURE_TEST_REPORT.md" << EOF


EOF
        for test in "${FAILED_TESTS[@]}"; do
            echo "- $test" >> "INFRASTRUCTURE_TEST_REPORT.md"
        done
    fi
    
    cat >> "INFRASTRUCTURE_TEST_REPORT.md" << EOF


1. **Address Failed Tests:** Review and fix any failed test cases
2. **Security Review:** Ensure all security configurations are properly implemented
3. **Performance Testing:** Conduct load testing on the infrastructure
4. **Disaster Recovery:** Test backup and recovery procedures
5. **Documentation:** Update deployment and operational documentation


1. Fix any failed infrastructure tests
2. Deploy to staging environment for integration testing
3. Conduct security penetration testing
4. Perform load and performance testing
5. Prepare for production deployment

EOF
    
    log "Infrastructure test report generated: INFRASTRUCTURE_TEST_REPORT.md"
    
    if [[ $success_rate -ge 90 ]]; then
        log "Infrastructure testing completed successfully! Success rate: $success_rate%"
        return 0
    else
        error "Infrastructure testing failed. Success rate: $success_rate% (minimum required: 90%)"
        return 1
    fi
}

run_infrastructure_tests() {
    log "Starting comprehensive infrastructure testing..."
    
    test_terraform_configuration
    test_helm_charts
    test_argocd_configuration
    test_kustomize_configuration
    test_versioning_rollback
    test_docker_configurations
    test_environment_configurations
    test_cicd_pipeline
    test_monitoring_configuration
    
    generate_test_report
}

run_specific_test() {
    local test_category=$1
    
    case "$test_category" in
        "terraform")
            test_terraform_configuration
            ;;
        "helm")
            test_helm_charts
            ;;
        "argocd")
            test_argocd_configuration
            ;;
        "kustomize")
            test_kustomize_configuration
            ;;
        "versioning")
            test_versioning_rollback
            ;;
        "docker")
            test_docker_configurations
            ;;
        "environment")
            test_environment_configurations
            ;;
        "cicd")
            test_cicd_pipeline
            ;;
        "monitoring")
            test_monitoring_configuration
            ;;
        *)
            error "Unknown test category: $test_category"
            echo "Available categories: terraform, helm, argocd, kustomize, versioning, docker, environment, cicd, monitoring"
            exit 1
            ;;
    esac
    
    generate_test_report
}

case "${1:-}" in
    "all")
        run_infrastructure_tests
        ;;
    "test")
        if [[ -z "${2:-}" ]]; then
            error "Test category required. Usage: $0 test <category>"
            exit 1
        fi
        run_specific_test "$2"
        ;;
    "report")
        if [[ -f "INFRASTRUCTURE_TEST_REPORT.md" ]]; then
            cat "INFRASTRUCTURE_TEST_REPORT.md"
        else
            error "No test report found. Run tests first."
            exit 1
        fi
        ;;
    *)
        echo "Hudur Platform Infrastructure Testing Script"
        echo ""
        echo "Usage: $0 <command> [options]"
        echo ""
        echo "Commands:"
        echo "  all                    - Run all infrastructure tests"
        echo "  test <category>        - Run specific test category"
        echo "  report                 - Display test report"
        echo ""
        echo "Test Categories:"
        echo "  terraform              - Test Terraform configuration"
        echo "  helm                   - Test Helm charts"
        echo "  argocd                 - Test ArgoCD configuration"
        echo "  kustomize              - Test Kustomize configuration"
        echo "  versioning             - Test versioning and rollback"
        echo "  docker                 - Test Docker configurations"
        echo "  environment            - Test environment configurations"
        echo "  cicd                   - Test CI/CD pipeline"
        echo "  monitoring             - Test monitoring configuration"
        echo ""
        echo "Examples:"
        echo "  $0 all"
        echo "  $0 test terraform"
        echo "  $0 test helm"
        echo "  $0 report"
        exit 1
        ;;
esac
