variable "environment" {
  type        = string
  description = "Deployment environment. Drives sizing and retention."

  validation {
    condition     = contains(["dev", "test", "prod"], var.environment)
    error_message = "environment must be one of dev, test or prod."
  }
}

variable "location" {
  type        = string
  description = "Azure region for every resource in this stack."
  default     = "westeurope"
}

variable "sql_admin_object_id" {
  type        = string
  description = "Object ID of the Entra group that owns the SQL server."
}

variable "allowed_origins" {
  type        = list(string)
  description = "CORS origins allowed to call the API."
  default     = []
}

variable "tags" {
  type        = map(string)
  description = "Tags applied to every resource."
  default     = {}
}
