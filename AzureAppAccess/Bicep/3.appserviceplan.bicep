resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01'={
  name:'azBicepServicePlan'
  location:resourceGroup().location
  sku:{
    name:'s1'
    capacity:1
  }
}
