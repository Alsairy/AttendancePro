
terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "~> 2.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "hudur-terraform-state"
    storage_account_name = "hudurterraformstate"
    container_name       = "tfstate"
    key                  = "attendancepro.terraform.tfstate"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

provider "kubernetes" {
  host                   = azurerm_kubernetes_cluster.hudur_aks.kube_config.0.host
  client_certificate     = base64decode(azurerm_kubernetes_cluster.hudur_aks.kube_config.0.client_certificate)
  client_key             = base64decode(azurerm_kubernetes_cluster.hudur_aks.kube_config.0.client_key)
  cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.hudur_aks.kube_config.0.cluster_ca_certificate)
}

provider "helm" {
  kubernetes {
    host                   = azurerm_kubernetes_cluster.hudur_aks.kube_config.0.host
    client_certificate     = base64decode(azurerm_kubernetes_cluster.hudur_aks.kube_config.0.client_certificate)
    client_key             = base64decode(azurerm_kubernetes_cluster.hudur_aks.kube_config.0.client_key)
    cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.hudur_aks.kube_config.0.cluster_ca_certificate)
  }
}

locals {
  environment = var.environment
  location    = var.location
  
  common_tags = {
    Environment   = var.environment
    Project       = "Hudur-AttendancePro"
    ManagedBy     = "Terraform"
    Owner         = "Hudur-DevOps"
    CostCenter    = "Engineering"
    Application   = "AttendancePro"
  }

  resource_prefix = "hudur-${var.environment}"
}

data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "hudur_rg" {
  name     = "${local.resource_prefix}-rg"
  location = local.location
  tags     = local.common_tags
}

resource "azurerm_virtual_network" "hudur_vnet" {
  name                = "${local.resource_prefix}-vnet"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  tags                = local.common_tags
}

resource "azurerm_subnet" "aks_subnet" {
  name                 = "${local.resource_prefix}-aks-subnet"
  resource_group_name  = azurerm_resource_group.hudur_rg.name
  virtual_network_name = azurerm_virtual_network.hudur_vnet.name
  address_prefixes     = ["10.0.1.0/24"]
}

resource "azurerm_subnet" "database_subnet" {
  name                 = "${local.resource_prefix}-db-subnet"
  resource_group_name  = azurerm_resource_group.hudur_rg.name
  virtual_network_name = azurerm_virtual_network.hudur_vnet.name
  address_prefixes     = ["10.0.2.0/24"]
  
  delegation {
    name = "database-delegation"
    service_delegation {
      name    = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action"]
    }
  }
}

resource "azurerm_subnet" "redis_subnet" {
  name                 = "${local.resource_prefix}-redis-subnet"
  resource_group_name  = azurerm_resource_group.hudur_rg.name
  virtual_network_name = azurerm_virtual_network.hudur_vnet.name
  address_prefixes     = ["10.0.3.0/24"]
}

resource "azurerm_network_security_group" "aks_nsg" {
  name                = "${local.resource_prefix}-aks-nsg"
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  tags                = local.common_tags

  security_rule {
    name                       = "AllowHTTPS"
    priority                   = 1001
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "AllowHTTP"
    priority                   = 1002
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "80"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}

resource "azurerm_subnet_network_security_group_association" "aks_nsg_association" {
  subnet_id                 = azurerm_subnet.aks_subnet.id
  network_security_group_id = azurerm_network_security_group.aks_nsg.id
}

resource "azurerm_key_vault" "hudur_kv" {
  name                = "${local.resource_prefix}-kv-${random_string.kv_suffix.result}"
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  tags                = local.common_tags

  enabled_for_disk_encryption     = true
  enabled_for_deployment          = true
  enabled_for_template_deployment = true
  purge_protection_enabled        = false
  soft_delete_retention_days      = 7

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Recover", "Backup", "Restore"
    ]

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Recover", "Backup", "Restore"
    ]

    certificate_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Recover", "Backup", "Restore"
    ]
  }
}

resource "random_string" "kv_suffix" {
  length  = 4
  special = false
  upper   = false
}

resource "azurerm_log_analytics_workspace" "hudur_logs" {
  name                = "${local.resource_prefix}-logs"
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.common_tags
}

resource "azurerm_application_insights" "hudur_insights" {
  name                = "${local.resource_prefix}-insights"
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  workspace_id        = azurerm_log_analytics_workspace.hudur_logs.id
  application_type    = "web"
  tags                = local.common_tags
}

resource "azurerm_container_registry" "hudur_acr" {
  name                = "${replace(local.resource_prefix, "-", "")}acr"
  resource_group_name = azurerm_resource_group.hudur_rg.name
  location            = azurerm_resource_group.hudur_rg.location
  sku                 = "Premium"
  admin_enabled       = true
  tags                = local.common_tags

  georeplications {
    location                = "East US"
    zone_redundancy_enabled = true
    tags                    = local.common_tags
  }
}

resource "azurerm_kubernetes_cluster" "hudur_aks" {
  name                = "${local.resource_prefix}-aks"
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  dns_prefix          = "${local.resource_prefix}-aks"
  kubernetes_version  = var.kubernetes_version
  tags                = local.common_tags

  default_node_pool {
    name                = "system"
    node_count          = var.system_node_count
    vm_size             = var.system_node_size
    vnet_subnet_id      = azurerm_subnet.aks_subnet.id
    type                = "VirtualMachineScaleSets"
    enable_auto_scaling = true
    min_count           = 1
    max_count           = 5
    max_pods            = 110
    os_disk_size_gb     = 128
    os_disk_type        = "Managed"

    upgrade_settings {
      max_surge = "10%"
    }

    tags = local.common_tags
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin    = "azure"
    network_policy    = "azure"
    dns_service_ip    = "10.1.0.10"
    docker_bridge_cidr = "172.17.0.1/16"
    service_cidr      = "10.1.0.0/16"
  }

  oms_agent {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.hudur_logs.id
  }

  azure_policy_enabled = true

  microsoft_defender {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.hudur_logs.id
  }

  key_vault_secrets_provider {
    secret_rotation_enabled  = true
    secret_rotation_interval = "2m"
  }
}

resource "azurerm_kubernetes_cluster_node_pool" "app_nodes" {
  name                  = "apps"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.hudur_aks.id
  vm_size               = var.app_node_size
  node_count            = var.app_node_count
  vnet_subnet_id        = azurerm_subnet.aks_subnet.id
  enable_auto_scaling   = true
  min_count             = 2
  max_count             = 10
  max_pods              = 110
  os_disk_size_gb       = 128
  os_disk_type          = "Managed"

  node_labels = {
    "workload-type" = "applications"
  }

  node_taints = [
    "workload-type=applications:NoSchedule"
  ]

  upgrade_settings {
    max_surge = "33%"
  }

  tags = local.common_tags
}

resource "azurerm_role_assignment" "aks_acr_pull" {
  scope                = azurerm_container_registry.hudur_acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_kubernetes_cluster.hudur_aks.kubelet_identity[0].object_id
}

resource "azurerm_postgresql_flexible_server" "hudur_postgres" {
  name                   = "${local.resource_prefix}-postgres"
  resource_group_name    = azurerm_resource_group.hudur_rg.name
  location               = azurerm_resource_group.hudur_rg.location
  version                = "14"
  delegated_subnet_id    = azurerm_subnet.database_subnet.id
  private_dns_zone_id    = azurerm_private_dns_zone.postgres_dns.id
  administrator_login    = var.postgres_admin_username
  administrator_password = var.postgres_admin_password
  zone                   = "1"
  storage_mb             = 32768
  sku_name               = var.postgres_sku_name
  backup_retention_days  = 7
  tags                   = local.common_tags

  depends_on = [azurerm_private_dns_zone_virtual_network_link.postgres_dns_link]
}

resource "azurerm_private_dns_zone" "postgres_dns" {
  name                = "${local.resource_prefix}-postgres.private.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.hudur_rg.name
  tags                = local.common_tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "postgres_dns_link" {
  name                  = "${local.resource_prefix}-postgres-dns-link"
  private_dns_zone_name = azurerm_private_dns_zone.postgres_dns.name
  virtual_network_id    = azurerm_virtual_network.hudur_vnet.id
  resource_group_name   = azurerm_resource_group.hudur_rg.name
  tags                  = local.common_tags
}

resource "azurerm_redis_cache" "hudur_redis" {
  name                = "${local.resource_prefix}-redis"
  location            = azurerm_resource_group.hudur_rg.location
  resource_group_name = azurerm_resource_group.hudur_rg.name
  capacity            = 2
  family              = "C"
  sku_name            = "Standard"
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
  subnet_id           = azurerm_subnet.redis_subnet.id
  tags                = local.common_tags

  redis_configuration {
    enable_authentication = true
  }
}

resource "azurerm_storage_account" "hudur_storage" {
  name                     = "${replace(local.resource_prefix, "-", "")}storage"
  resource_group_name      = azurerm_resource_group.hudur_rg.name
  location                 = azurerm_resource_group.hudur_rg.location
  account_tier             = "Standard"
  account_replication_type = "GRS"
  min_tls_version          = "TLS1_2"
  tags                     = local.common_tags

  blob_properties {
    versioning_enabled = true
    delete_retention_policy {
      days = 7
    }
  }
}

resource "azurerm_storage_container" "face_templates" {
  name                  = "face-templates"
  storage_account_name  = azurerm_storage_account.hudur_storage.name
  container_access_type = "private"
}

resource "azurerm_storage_container" "documents" {
  name                  = "documents"
  storage_account_name  = azurerm_storage_account.hudur_storage.name
  container_access_type = "private"
}

resource "azurerm_storage_container" "backups" {
  name                  = "backups"
  storage_account_name  = azurerm_storage_account.hudur_storage.name
  container_access_type = "private"
}
