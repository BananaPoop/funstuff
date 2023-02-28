using Pulumi;
using Pulumi.Command.Local;
using AzureNative = Pulumi.AzureNative; //We may support multicloud in the future, so dont pollute the namespace with AzureNative
using System.Collections.Generic;
using System.IO;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new AzureNative.Resources.ResourceGroup("resourceGroup", new(){
        Location = "Australia East"
    });

    // Create an Azure resource (Storage Account)
    var storageAccount = new AzureNative.Storage.StorageAccount("sablob", new AzureNative.Storage.StorageAccountArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Sku = new AzureNative.Storage.Inputs.SkuArgs
        {
            Name = AzureNative.Storage.SkuName.Standard_LRS
        },
        Kind = AzureNative.Storage.Kind.StorageV2
    });

    var testContainer = new AzureNative.Storage.BlobContainer("testContainer", new AzureNative.Storage.BlobContainerArgs
    {
        AccountName = storageAccount.Name,
        ResourceGroupName = resourceGroup.Name,
        PublicAccess = AzureNative.Storage.PublicAccess.None,
        ContainerName = "mycontainer"
    });

    var storageAccountKeys = AzureNative.Storage.ListStorageAccountKeys.Invoke(new AzureNative.Storage.ListStorageAccountKeysInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        AccountName = storageAccount.Name
    });

    // var primaryStorageKey = storageAccountKeys.Apply(accountKeys =>
    // {
    //     var firstKey = accountKeys.Keys[0].Value;
    //     return Output.CreateSecret(firstKey);
    // });

    //Stuff for Azure Function Serverless App
    const string appPath = "../functionApp";
    var appContainer = new AzureNative.Storage.BlobContainer("appcontainer", new AzureNative.Storage.BlobContainerArgs
    {
        AccountName = storageAccount.Name,
        ResourceGroupName = resourceGroup.Name,
        PublicAccess = AzureNative.Storage.PublicAccess.None,
    });

    var outputPath = "publish";
    var publishCommand = Run.Invoke(new(){
        Command = $"dotnet publish -c Release --output {outputPath}",
        Dir = appPath,
    });

    var appBlob = new AzureNative.Storage.Blob("appBlob", new ()
    {
        AccountName = storageAccount.Name,
        ResourceGroupName = resourceGroup.Name,
        ContainerName = appContainer.Name,
        Source = publishCommand.Apply(result=> new FileArchive(Path.Combine(appPath, outputPath)) as AssetOrArchive),
        Type = AzureNative.Storage.BlobType.Block
    });

    // Create a shared access signature to give the Function App access to the code.
    var signature = AzureNative.Storage.ListStorageAccountServiceSAS.Invoke(new()
    {
        ResourceGroupName = resourceGroup.Name,
        AccountName = storageAccount.Name,
        Protocols = AzureNative.Storage.HttpProtocol.Https,
        SharedAccessStartTime = "2022-01-01",
        SharedAccessExpiryTime = "2030-01-01",
        Resource = AzureNative.Storage.SignedResource.C,
        Permissions = AzureNative.Storage.Permissions.R,
        ContentType = "application/json",
        CacheControl = "max-age=5",
        ContentDisposition = "inline",
        ContentEncoding = "deflate",
        CanonicalizedResource = Output.Tuple(storageAccount.Name, appContainer.Name).Apply(values => $"/blob/{values.Item1}/{values.Item2}"),
    }).Apply(result => result.ServiceSasToken);

    var plan = new AzureNative.Web.AppServicePlan("appPlan", new (){
        ResourceGroupName = resourceGroup.Name,
        Kind = "FunctionApp",
        Sku = new AzureNative.Web.Inputs.SkuDescriptionArgs
        {
            Name = "Y1",
            Tier = "Dynamic",
        },
        Location = resourceGroup.Location,
    });


    var functionApp = new AzureNative.Web.WebApp("functionApp", new(){
        ResourceGroupName = resourceGroup.Name,
        ServerFarmId = plan.Id,
        Kind = "FunctionApp",
        SiteConfig = new AzureNative.Web.Inputs.SiteConfigArgs {
            NetFrameworkVersion = "v7.0",
            DetailedErrorLoggingEnabled = true,
            HttpLoggingEnabled = true,
            AppSettings = new AzureNative.Web.Inputs.NameValuePairArgs[]{
                new(){
                    Name = "FUNCTIONS_WORKER_RUNTIME",
                    Value = "dotnet-isolated",
                },
                new(){
                    Name = "FUNCTIONS_EXTENSION_VERSION",
                    Value = "~4",
                },
                new (){
                    Name = "WEBSITE_RUN_FROM_PACKAGE",
                    Value = Output.Tuple(storageAccount.Name, appContainer.Name, appBlob.Name, signature).Apply(values =>
                        $"https://{values.Item1}.blob.core.windows.net/{values.Item2}/{values.Item3}?{values.Item4}"),
                },
                new (){
                    Name = "AzureWebJobsStorage",
                    Value = Output.Tuple(storageAccount.Name, storageAccountKeys).Apply(values =>
                        $"DefaultEndpointsProtocol=https;AccountName={values.Item1};AccountKey={values.Item2.Keys[0].Value};EndpointSuffix=core.windows.net"),
                },
            },
            Cors = new AzureNative.Web.Inputs.CorsSettingsArgs(){
                AllowedOrigins = new []{"*"}
            }
        }
    });

    //Provide this to the config for any website that needs to call the api
    // var apiUrl = functionApp.DefaultHostName.Apply(hostname => {
    //         var config = System.Text.Json.JsonSerializer.Serialize(new { api = $"https://{hostname}/api" });
    //         return new Pulumi.StringAsset(config) as AssetOrArchive;
    //     });

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?>
    {
        //["apiUrl"] = apiUrl,
        //["primaryStorageKey"] = primaryStorageKey
    };
});