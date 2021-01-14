// Resource name
param name string

// Resource location
param location string = resourceGroup().location

// Resource location code
param locationCode string = 'krc'

// Storage Account
param storageAccountSku string = 'Standard_LRS'

// Function App
param functionAppWorkerRuntime string = 'dotnet'
param functionAppEnvironment string {
    allowed: [
        'Development'
        'Staging'
        'Production'
    ]
    default: 'Production'
}
param functionAppTimezone string = 'Korea Standard Time'

// Scheduler
param schedulerMaxDuration string = '27.12:00:00'

// Dev.To
param devtoApiKey string {
    secure: true
}
param devtoArticleSelector string = 'css=div#article-body'
param devtoArticlePageFunction string = 'element => element.dataset[\'articleId\']'
param devtoArticleIdPattern string = '<div.+data-article-id="(\\d+)"\\s*id="article-body">'

var metadata = {
    longName: '{0}-${name}-${locationCode}'
    shortName: '{0}${replace(name, '-', '')}${locationCode}'
}

var storage = {
    name: format(metadata.shortName, 'st')
    location: location
    sku: storageAccountSku
}

resource st 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storage.name
    location: storage.location
    kind: 'StorageV2'
    sku: {
        name: storage.sku
    }
    properties: {
        supportsHttpsTrafficOnly: true
    }
}

var workspace = {
    name: format(metadata.longName, 'wrkspc')
    location: location
}

resource wrkspc 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
    name: workspace.name
    location: workspace.location
    properties: {
        sku: {
            name: 'PerGB2018'
        }
        retentionInDays: 30
        workspaceCapping: {
            dailyQuotaGb: -1
        }
        publicNetworkAccessForIngestion: 'Enabled'
        publicNetworkAccessForQuery: 'Enabled'
    }
}

var appInsights = {
    name: format(metadata.longName, 'appins')
    location: location
}

resource appins 'Microsoft.Insights/components@2020-02-02-preview' = {
    name: appInsights.name
    location: appInsights.location
    kind: 'web'
    properties: {
        Flow_Type: 'Bluefield'
        Application_Type: 'web'
        Request_Source: 'rest'
        WorkspaceResourceId: wrkspc.id
    }
}

var servicePlan = {
    name: format(metadata.longName, 'csplan', '')
    location: location
}

resource csplan 'Microsoft.Web/serverfarms@2020-06-01' = {
    name: servicePlan.name
    location: servicePlan.location
    kind: 'functionApp'
    sku: {
        name: 'Y1'
        tier: 'Dynamic'
    }
    // properties: {
    //     reserved: true
    // }
}

var functionApp = {
    name: format(metadata.longName, 'fncapp')
    location: location
    environment: functionAppEnvironment
    runtime: functionAppWorkerRuntime
    timezone: functionAppTimezone
}

var scheduler = {
    maxDuration: schedulerMaxDuration
}

var devto = {
    apiKey: devtoApiKey
    article: {
        selector: devtoArticleSelector
        pageFunction: devtoArticlePageFunction
        idPattern: devtoArticleIdPattern
    }
}

resource fncapp 'Microsoft.Web/sites@2020-06-01' = {
    name: functionApp.name
    location: functionApp.location
    kind: 'functionapp'
    properties: {
        serverFarmId: csplan.id
        httpsOnly: true
        siteConfig: {
            appSettings: [
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: reference(appins.id, '2020-02-02-preview', 'Full').properties.InstrumentationKey
                }
                {
                    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                    value: reference(appins.id, '2020-02-02-preview', 'Full').properties.connectionString
                }
                {
                    name: 'AZURE_FUNCTIONS_ENVIRONMENT'
                    value: functionApp.environment
                }
                {
                    name: 'AzureWebJobsStorage'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'FUNCTIONS_EXTENSION_VERSION'
                    value: '~3'
                }
                {
                    name: 'FUNCTION_APP_EDIT_MODE'
                    value: 'readonly'
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: functionApp.runtime
                }
                {
                    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'WEBSITE_CONTENTSHARE'
                    value: functionApp.name
                }
                {
                    name: 'WEBSITE_NODE_DEFAULT_VERSION'
                    value: '~12'
                }
                {
                    name: 'WEBSITE_TIME_ZONE'
                    value: functionApp.timezone
                }
                // Scheduler specific settings
                {
                    name: 'Scheduler__MaxDuration'
                    value: scheduler.maxDuration
                }
                {
                    name: 'DevTo__ApiKey'
                    value: devto.apiKey
                }
                {
                    name: 'DevTo__Article__Selector'
                    value: devto.article.selector
                }
                {
                    name: 'DevTo__Article__PageFunction'
                    value: devto.article.pageFunction
                }
                {
                    name: 'DevTo__Article__IdPattern'
                    value: devto.article.idPattern
                }
            ]
        }
    }
}
