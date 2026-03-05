resource "azurerm_mssql_server" "this" {
  name                          = "sql-${local.prefix}-${random_string.suffix.result}"
  location                      = azurerm_resource_group.this.location
  resource_group_name           = azurerm_resource_group.this.name
  version                       = "12.0"
  minimum_tls_version           = "1.2"
  public_network_access_enabled = false

  azuread_administrator {
    login_username              = "sql-admins-${var.environment}"
    object_id                   = var.sql_admin_object_id
    azuread_authentication_only = true
  }

  tags = local.base_tags
}

resource "azurerm_mssql_database" "this" {
  name           = "sqldb-contoso"
  server_id      = azurerm_mssql_server.this.id
  sku_name       = local.current.sql_sku
  zone_redundant = local.current.zone_balance
  collation      = "SQL_Latin1_General_CP1_CI_AS"

  short_term_retention_policy {
    retention_days = var.environment == "prod" ? 35 : 7
  }

  tags = local.base_tags
}

resource "azurerm_redis_cache" "this" {
  name                          = "redis-${local.prefix}-${random_string.suffix.result}"
  location                      = azurerm_resource_group.this.location
  resource_group_name           = azurerm_resource_group.this.name
  capacity                      = local.current.redis_cap
  family                        = local.current.redis_family
  sku_name                      = local.current.redis_sku
  non_ssl_port_enabled          = false
  minimum_tls_version           = "1.2"
  public_network_access_enabled = false
  tags                          = local.base_tags
}

resource "azurerm_key_vault" "this" {
  name                       = "kv-${local.prefix}-${random_string.suffix.result}"
  location                   = azurerm_resource_group.this.location
  resource_group_name        = azurerm_resource_group.this.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  enable_rbac_authorization  = true
  purge_protection_enabled   = var.environment == "prod"
  soft_delete_retention_days = 7
  tags                       = local.base_tags
}

data "azurerm_client_config" "current" {}

resource "azurerm_role_assignment" "api_reads_secrets" {
  scope                = azurerm_key_vault.this.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_linux_web_app.api.identity[0].principal_id
}
