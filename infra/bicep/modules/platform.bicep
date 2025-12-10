param prefix string
param location string
param appSku string
param logRetention int
param zoneRedundant bool
param sqlAdminObjectId string

var suffix = uniqueString(resourceGroup().id)

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-${prefix}'
  location: location
  properties: {
    sku: { name: 'PerGB2018' }
    retentionInDays: logRetention
  }
}

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-${prefix}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: 'plan-${prefix}'
  location: location
  kind: 'linux'
  sku: { name: appSku }
  properties: {
    reserved: true
    zoneRedundant: zoneRedundant
  }
}

resource api 'Microsoft.Web/sites@2023-12-01' = {
  name: 'app-${prefix}-api-${suffix}'
  location: location
  identity: { type: 'SystemAssigned' }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: appSku != 'B1'
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      healthCheckPath: '/healthz'
      appSettings: [
        { name: 'APPLICATIONINSIGHTS_CONNECTION_STRING', value: insights.properties.ConnectionString }
      ]
    }
  }
}

resource sql 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: 'sql-${prefix}-${suffix}'
  location: location
  properties: {
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Disabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: 'sql-admins'
      sid: sqlAdminObjectId
      azureADOnlyAuthentication: true
    }
  }
}

output apiHostname string = api.properties.defaultHostName
output apiPrincipalId string = api.identity.principalId
