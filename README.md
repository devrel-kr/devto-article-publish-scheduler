# DevTo Article Publish Scheduler #

This is a scheduler, in a given date and time, to publish an article to [https://dev.to](https://dev.to).


## Getting Started ##

## Keys/Secrets ##

To deploy this app onto Azure, you need a few things beforehand.

* API key for dev.to: Go to [https://dev.to/settings/account](https://dev.to/settings/account) and generate a new API key.
* Azure credentials: Use your existing Azure account or [create a free account](https://azure.microsoft.com/free/?WT.mc_id=github-0000-juyoo), if you don't have one. Then run the following [Azure CLI](https://docs.microsoft.com/cli/azure/what-is-azure-cli?WT.mc_id=github-0000-juyoo) command to get your Azure credentials:
    ```bash
    az ad sp create-for-rbac \
        --name "<service_principal_name>" \
        --sdk-auth \
        --role contributor
    ```
* Azure resource group: Use your existing Azure resource group or create a new one by running the Azure CLI command:
    ```bash
    az group create \
        -n "<resource_group_name>" \
        -l "<location>"
    ```


## GitHub Secrets for CI/CD ##

If you want to automate your CI/CD pipeline through [GitHub Actions](https://docs.github.com/en/free-pro-team@latest/actions), Create the following [GitHub Secrets](https://docs.github.com/en/free-pro-team@latest/actions/reference/encrypted-secrets) on your forked repository:

* `AZURE_CREDENTIALS`: Store your Azure credentials generated above.
* `DEVTO_API_KEY`: Store your dev.to APK key generated above.
* `RESOURCE_GROUP_NAME`: Store your Azure resource group name generated above.
* `RESOURCE_FUNCTIONAPP_ENVIRONMENT`: Set `Production`
* `RESOURCE_NAME`: Set your resource name&ndash;this will be used for all your Azure resources including [Storage Account](https://docs.microsoft.com/azure/storage/common/storage-account-overview?WT.mc_id=github-0000-juyoo), [Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview?WT.mc_id=github-0000-juyoo), [Azure Monitor](https://docs.microsoft.com/azure/azure-monitor/overview?WT.mc_id=github-0000-juyoo), [Consumption Plan](https://docs.microsoft.com/azure/azure-functions/consumption-plan?WT.mc_id=github-0000-juyoo) and [Azure Functions](https://docs.microsoft.com/azure/azure-functions/functions-overview?WT.mc_id=github-0000-juyoo).


## Manual Deployment ##

Alternatively, you can run manual provision and app deployment using Azure CLI. Run the following command:

```bash
# Login to Azure
az login

# Provision resources on Azure
az deployment group create \
    -n <deployment_name> \
    -g <resource_group_name> \
    --template-file ./resources/azuredeploy.json \
    --parameters name=<resource_name> \
    --parameters devtoApiKey=<devto_api_key>
    --verbose

# Publish Function app
dotnet publish . -c Release
published='src/PublishDevTo.FunctionApp/bin/Release/netcoreapp3.1/publish'
zip -r published.zip $published

# Deploy Azure Functions app
az functionapp deployment source config-zip \
    -g <resource_group_name> \
    -n <function_app_name> \
    --src published.zip
```


## Schedule Your Article for Publish ##

You have your dev.to article temporarily saved and know the preview URL, with this preview URL, send an HTTP API request to your scheduler app deployed above:

```bash
# Get Function app resource ID
resourceId=$(az functionapp show \
    -g <resource_group_name> \
    -n <function_app_name> \
    --query "id" -o tsv)

# Get Function app auth key
authkey=$(az rest \
    -m POST \
    -u "https://management.azure.com$resourceId/host/default/listkeys?api-version=2020-06-01" \
    --query "functionKeys.default" -o tsv)

# date_time_in_iso8601_format: yyyy-MM-ddTHH:mm:sszzz
curl -X POST 'https://<function_app_name>.azurewebsites.net/api/orchestrators/schedules' \
    -H "x-functions-key: $authkey" \
    -H "Content-Type: application/json" \
    -d '{ "previewUri": "<devto_article_preview_url>", "schedule": "<date_time_in_iso8601_format>" }'
```


## Check Your Scheduling Status ##

Once you schedule your article, you can check its status with the following command:

```bash
curl -X GET 'https://<function_app_name>.azurewebsites.net/api/orchestrators/schedules' \
    -H "x-functions-key: $authkey"
```


## TO-DO List ##

* ✅ Scheduler API
* 🔲 UI Page


## Contribution ##

Your contributions are always welcome! All your work should be done in your forked repository. Once you finish your work with corresponding tests, please send us a pull request onto our `dev` branch for review.


## License ##

**DevTo Article Publish Scheduler** is released under [MIT License](http://opensource.org/licenses/MIT)

> The MIT License (MIT)
>
> Copyright (c) 2020 DevRel Korea (한국 DevRel 커뮤니티)
> 
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
