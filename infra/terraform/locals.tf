locals {
  prefix = "contoso-${var.environment}"

  sizing = {
    dev = {
      app_sku       = "B1"
      sql_sku       = "GP_S_Gen5_1"
      redis_sku     = "Basic"
      redis_family  = "C"
      redis_cap     = 0
      log_retention = 30
      zone_balance  = false
    }
    test = {
      app_sku       = "P0v3"
      sql_sku       = "GP_S_Gen5_2"
      redis_sku     = "Standard"
      redis_family  = "C"
      redis_cap     = 1
      log_retention = 30
      zone_balance  = false
    }
    prod = {
      app_sku       = "P1v3"
      sql_sku       = "GP_Gen5_4"
      redis_sku     = "Premium"
      redis_family  = "P"
      redis_cap     = 1
      log_retention = 90
      zone_balance  = true
    }
  }

  current = local.sizing[var.environment]

  base_tags = merge(var.tags, {
    application = "contoso-learning"
    environment = var.environment
    managed_by  = "terraform"
  })
}
