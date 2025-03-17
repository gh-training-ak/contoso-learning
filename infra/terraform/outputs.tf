output "api_hostname" {
  description = "Public hostname of the API app service."
  value       = azurerm_linux_web_app.api.default_hostname
}

output "api_principal_id" {
  description = "Managed identity of the API, used for data plane role assignments."
  value       = azurerm_linux_web_app.api.identity[0].principal_id
}

output "sql_fqdn" {
  description = "Private FQDN of the SQL server."
  value       = azurerm_mssql_server.this.fully_qualified_domain_name
}

output "key_vault_uri" {
  description = "Vault URI consumed by the API configuration provider."
  value       = azurerm_key_vault.this.vault_uri
}
