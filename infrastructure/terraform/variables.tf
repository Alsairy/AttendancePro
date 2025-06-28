
variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "prod"
  
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod."
  }
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "West Europe"
}

variable "kubernetes_version" {
  description = "Kubernetes version for AKS cluster"
  type        = string
  default     = "1.28.3"
}

variable "system_node_count" {
  description = "Number of nodes in the system node pool"
  type        = number
  default     = 3
  
  validation {
    condition     = var.system_node_count >= 1 && var.system_node_count <= 10
    error_message = "System node count must be between 1 and 10."
  }
}

variable "system_node_size" {
  description = "VM size for system nodes"
  type        = string
  default     = "Standard_D4s_v3"
}

variable "app_node_count" {
  description = "Number of nodes in the application node pool"
  type        = number
  default     = 3
  
  validation {
    condition     = var.app_node_count >= 2 && var.app_node_count <= 20
    error_message = "Application node count must be between 2 and 20."
  }
}

variable "app_node_size" {
  description = "VM size for application nodes"
  type        = string
  default     = "Standard_D8s_v3"
}

variable "postgres_admin_username" {
  description = "Administrator username for PostgreSQL"
  type        = string
  default     = "hudur_admin"
  sensitive   = true
}

variable "postgres_admin_password" {
  description = "Administrator password for PostgreSQL"
  type        = string
  sensitive   = true
  
  validation {
    condition     = length(var.postgres_admin_password) >= 12
    error_message = "PostgreSQL password must be at least 12 characters long."
  }
}

variable "postgres_sku_name" {
  description = "SKU name for PostgreSQL Flexible Server"
  type        = string
  default     = "GP_Standard_D4s_v3"
}

variable "enable_monitoring" {
  description = "Enable monitoring and observability stack"
  type        = bool
  default     = true
}

variable "enable_backup" {
  description = "Enable automated backup solutions"
  type        = bool
  default     = true
}

variable "backup_retention_days" {
  description = "Number of days to retain backups"
  type        = number
  default     = 30
  
  validation {
    condition     = var.backup_retention_days >= 7 && var.backup_retention_days <= 365
    error_message = "Backup retention must be between 7 and 365 days."
  }
}

variable "ssl_certificate_email" {
  description = "Email address for SSL certificate registration"
  type        = string
  default     = "admin@hudur.sa"
}

variable "domain_name" {
  description = "Primary domain name for the application"
  type        = string
  default     = "hudur.sa"
}

variable "staging_domain_name" {
  description = "Staging domain name for the application"
  type        = string
  default     = "staging.hudur.sa"
}

variable "enable_disaster_recovery" {
  description = "Enable disaster recovery configuration"
  type        = bool
  default     = true
}

variable "dr_location" {
  description = "Disaster recovery region"
  type        = string
  default     = "North Europe"
}

variable "enable_auto_scaling" {
  description = "Enable auto-scaling for AKS cluster"
  type        = bool
  default     = true
}

variable "max_node_count" {
  description = "Maximum number of nodes for auto-scaling"
  type        = number
  default     = 20
  
  validation {
    condition     = var.max_node_count >= 5 && var.max_node_count <= 100
    error_message = "Maximum node count must be between 5 and 100."
  }
}

variable "enable_network_policy" {
  description = "Enable network policy for AKS"
  type        = bool
  default     = true
}

variable "enable_pod_security_policy" {
  description = "Enable pod security policy"
  type        = bool
  default     = true
}

variable "log_retention_days" {
  description = "Log retention period in days"
  type        = number
  default     = 90
  
  validation {
    condition     = var.log_retention_days >= 30 && var.log_retention_days <= 365
    error_message = "Log retention must be between 30 and 365 days."
  }
}

variable "enable_private_cluster" {
  description = "Enable private AKS cluster"
  type        = bool
  default     = true
}

variable "authorized_ip_ranges" {
  description = "Authorized IP ranges for AKS API server access"
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Additional tags to apply to resources"
  type        = map(string)
  default     = {}
}

variable "enable_defender" {
  description = "Enable Microsoft Defender for containers"
  type        = bool
  default     = true
}

variable "enable_azure_policy" {
  description = "Enable Azure Policy for AKS"
  type        = bool
  default     = true
}

variable "enable_secret_store_csi" {
  description = "Enable Secret Store CSI driver"
  type        = bool
  default     = true
}

variable "postgres_version" {
  description = "PostgreSQL version"
  type        = string
  default     = "14"
  
  validation {
    condition     = contains(["13", "14", "15"], var.postgres_version)
    error_message = "PostgreSQL version must be 13, 14, or 15."
  }
}

variable "postgres_storage_mb" {
  description = "PostgreSQL storage size in MB"
  type        = number
  default     = 32768
  
  validation {
    condition     = var.postgres_storage_mb >= 32768
    error_message = "PostgreSQL storage must be at least 32GB (32768 MB)."
  }
}

variable "postgres_backup_retention_days" {
  description = "PostgreSQL backup retention in days"
  type        = number
  default     = 7
  
  validation {
    condition     = var.postgres_backup_retention_days >= 7 && var.postgres_backup_retention_days <= 35
    error_message = "PostgreSQL backup retention must be between 7 and 35 days."
  }
}

variable "redis_capacity" {
  description = "Redis cache capacity"
  type        = number
  default     = 2
  
  validation {
    condition     = contains([1, 2, 3, 4, 5, 6], var.redis_capacity)
    error_message = "Redis capacity must be between 1 and 6."
  }
}

variable "redis_sku_name" {
  description = "Redis SKU name"
  type        = string
  default     = "Standard"
  
  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.redis_sku_name)
    error_message = "Redis SKU must be Basic, Standard, or Premium."
  }
}

variable "storage_account_tier" {
  description = "Storage account tier"
  type        = string
  default     = "Standard"
  
  validation {
    condition     = contains(["Standard", "Premium"], var.storage_account_tier)
    error_message = "Storage account tier must be Standard or Premium."
  }
}

variable "storage_replication_type" {
  description = "Storage account replication type"
  type        = string
  default     = "GRS"
  
  validation {
    condition     = contains(["LRS", "GRS", "RAGRS", "ZRS", "GZRS", "RAGZRS"], var.storage_replication_type)
    error_message = "Invalid storage replication type."
  }
}
