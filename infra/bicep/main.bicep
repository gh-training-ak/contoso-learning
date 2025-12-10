targetScope = 'subscription'

@description('Deployment environment. Drives sizing and retention.')
@allowed(['dev', 'test', 'prod'])
param environment string

@description('Azure region for every resource in this stack.')
param location string = 'westeurope'

@description('Object ID of the Entra group that owns the SQL server.')
param sqlAdminObjectId string

var prefix = 'contoso-${environment}'

var sizing = {
  dev:  { appSku: 'B1',   logRetention: 30, zoneRedundant: false }
  test: { appSku: 'P0v3', logRetention: 30, zoneRedundant: false }
  prod: { appSku: 'P1v3', logRetention: 90, zoneRedundant: true }
}

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-${prefix}'
  location: location
  tags: {
    application: 'contoso-learning'
    environment: environment
    managedBy: 'bicep'
  }
}

module platform 'modules/platform.bicep' = {
  name: 'platform-${environment}'
  scope: rg
  params: {
    prefix: prefix
    location: location
    appSku: sizing[environment].appSku
    logRetention: sizing[environment].logRetention
    zoneRedundant: sizing[environment].zoneRedundant
    sqlAdminObjectId: sqlAdminObjectId
  }
}

output apiHostname string = platform.outputs.apiHostname
output apiPrincipalId string = platform.outputs.apiPrincipalId
