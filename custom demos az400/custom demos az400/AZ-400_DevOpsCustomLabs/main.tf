terraform {
  required_providers {
    azurerm={
        source="hashicorp/azurerm"
        version="3.17.0"
    }
  }
}

provider "azurerm" {
  subscription_id = "c5f6b777-a115-4e13-86f4-e1af2e819314"
  tenant_id = "cf55562b-6b91-4e5d-ae52-57763f374edf"
  client_id = "5f845813-cfda-474b-b413-0bccac507c7d"
  client_secret = "t.w8Q~w4--yGImnfXj8ExUQmzjuwjqtzhraDdbAZ"
  features {    
  }
}

resource "azurerm_service_plan" "demowebnew456" {
  name                = "demowebnew456"
  resource_group_name = "template-grp"
  location            = "North Europe"
  os_type             = "Windows"
  sku_name            = "S1"
}

resource "azurerm_windows_web_app" "latestwebapp765" {
  name                = "latestwebapp765"
  resource_group_name = "template-grp"
  location            = "North Europe"
  service_plan_id     = azurerm_service_plan.demowebnew456.id

  site_config {
    always_on = false
    application_stack{
        current_stack="dotnet"
        dotnet_version="v6.0"
    }
  }

  depends_on = [
    azurerm_service_plan.demowebnew456
  ]
}
