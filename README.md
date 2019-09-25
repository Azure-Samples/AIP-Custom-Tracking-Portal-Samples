---
page_type: sample
languages:
- csharp
products:
- dotnet
description: "This sample solution helps AIP users track access to protected files shared internally and externally."
urlFragment: "aip-custom-tracking-portal-samples"
---

# AIP Custom Tracking Portal Sample

<!-- 
Guidelines on README format: https://review.docs.microsoft.com/help/onboard/admin/samples/concepts/readme-template?branch=master

Guidance on onboarding samples to docs.microsoft.com/samples: https://review.docs.microsoft.com/help/onboard/admin/samples/process/onboarding?branch=master

Taxonomies for products and languages: https://review.docs.microsoft.com/new-hope/information-architecture/metadata/taxonomies?branch=master
-->

Azure Information Protection Custom Tracking Portal. This sample solution helps AIP users track access to protected files shared internally and externally.

## Summary
This repo contains sample code demonstrating how you can build a custom AIP Tracking portal that allows end users to see who has accessed or tried to access protected files they've shared internally or externally. The sample solution has an ASP.NET web application to display a list of recently protected documents for the current user. The web application queries AIP Log Analytics via two Azure Functions using REST API and populates a GridView with the results.

Why should you build something like this? For starters, you have more flexibility and can display the right data to end user. Additionally, you could combine the data provided by AIP with other datasets within your organization to enrich the context of the information displayed to your users.

Please read our blog [for more details on this solution](https://dev.loganalytics.io/oms/documentation/1-Tutorials/1-Direct-API
"Custom AIP Portal Blog")

## Prerequisites

While the solution is quite simple, some assembly is required.

•	Visual Studio 2017 or higher  
•	An Azure subscription with a Log Analytics Workspace created  
•	Azure Information Protection (AIP) with Log Analytics integration configured   
•	Either Classic or Unified Labeling client installed on a supported version of Windows (7 or above as of today)  
•	Optional: Azure Service App to host the Web application  
•	Two Azure Functions  
•	An Azure AD application (Service Principal)  
•	Optional: Azure Key Vault  


## Setup

## Clone the Repository
1. Open a command prompt  
2. Create a new folder mkdir c:\samples  
3. Navigate to the new folder using cd c:\samples  
4. Clone the repository by running git clone https://github.com/Azure-Samples/AIP-Custom-Tracking-Portal-Samples  
5. In explorer, navigate to c:\samples\AIP-Custom-Tracking-Portal-Samples and open the AIP-Custom-Tracking-Portal-Samples.sln in Visual Studio 2017 or later.  

## Add the NuGet Package
In Visual Studio, right click the **AIP-Custom-Tracking-Portal-Samples** solution.  
Click **Restore NuGet Packages**

## Authentication
This sample solution uses a single application (service principal) that you must register in Azure AD. Note that this service pricipal requires **Data.Reader** rights in your Log Analytics Workspace as explained on the blog above.  
[Follow these instructions to register an application in Azure Active Directory](https://dev.loganalytics.io/oms/documentation/1-Tutorials/1-Direct-API
"Register Azure AD app")

## Azure Functions keys
To view your keys, create new ones, navigate to one of your HTTP-triggered functions in the [Azure portal]( https://portal.azure.com "Azure Portal") and select **Manage**.  
Functions lets you use keys to make it harder to access your HTTP function endpoints during development. A standard HTTP trigger may require such an API key be present in the request. Most HTTP trigger templates require an API key in the request. So, your HTTP request implementation looks like the following URL: https://<APP_NAME>.azurewebsites.net/api/<FUNCTION_NAME>?name=<USER_EMAIL>&code=<API_KEY>
 
 ## Important
While keys may help obfuscate your HTTP endpoints during development, they are not intended to secure an HTTP trigger in production. To learn more, see [Secure an HTTP endpoint in production](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook#secure-an-http-endpoint-in-production "Secure HTTP endpoint in production").

## Setup/Configure Azure Key Vault
Although Azure Key Vault is an optional component, we highly recommend it. As a bonus, it’s already wired up on both Azure Functions.    
**TODO:** Wire up Azure Key Vault on the ASP.NET Web app in this solution.  
**NOTE** Make sure you follow the [managed identities]( https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity#creating-an-app-with-an-identity "Tenant ID") instructions as well if you decide to use Key Vault.

Please follow Jeff’s excellent walkthrough on how to setup Key Vault: [Configure Azure Key Vault]( https://medium.com/statuscode/getting-key-vault-secrets-in-azure-functions-37620fd20a0b "Tenant ID")

## Update Web.Config
The web.config file must be updated to store several identity and application-specific settings. Several of these settings should already be populated if you created the project from scratch and configured authentication in the wizard.

## Update appSettings
In the AIP-Custom-Tracking-Portal-Samples project, open the **Web.config** file and find the appSettings section.
Update the values in bold below with the values from previous steps.  
Use the table below to find the value for each setting and update the Web.config.

| Key       | Value or Location                                |
|-------------------|--------------------------------------------|
| `ida:ClientId`             | Azure AD App Registration Portal |
| `ida:Domain`      | Domain of AAD Tenant - e.g. Contoso.Onmicrosoft.com      |
| `ida:TenantId`    | From the [AAD Properties Blade]( https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Properties "Tenant ID")            |
| `ida:FirstAzFunctionKey`      | Your first Azure Function Key - Obtained from the **azure-function-list-documents** Function in the Azure portal      |
| `ida:SecondAzFunctionKey`    | Your second Azure Function Key - Obtained from the **azure-function-list-activity** Function in Azure portal          |

## Update Default.aspx.cs file in the web app UI
The presentation layer of this solution makes an HTTP request to the Azure Functions above and needs to be updated. 
Update the <APP_NAME> and <FUNCTION_NAME> inside the **Default.aspx.cs** file with your own values. The <APP_NAME> refers to the name of the Function app in Azure Apps Services.

Finally, we want to hear from you. Please contribute and let us know what other use cases you come up with.

## Sources/Attribution/License/3rd Party Code
Unless otherwise noted, all content is licensed under MIT license.  
JSON de/serialization provided by [Json.NET](https://www.nuget.org/packages/Newtonsoft.Json/)  

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
