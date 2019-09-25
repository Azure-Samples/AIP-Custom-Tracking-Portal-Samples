// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE.txt in the project root for license information.
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.OperationalInsights;
using System.Text;
using System.Net.Http;
using System.Net;

namespace MyLogsFunction
{
    public static class AIPUserActivity
    {
        // Get WorkspaceID, ClientID and Client Secret from Azure KeyVault
        private static string LAWorkSpaceIDAKV = System.Environment.GetEnvironmentVariable("WorkSpaceIDFromAKV");
        private static string clientIDAKV = System.Environment.GetEnvironmentVariable("ClientIDFromAKV");
        private static string clientSecretAKV = System.Environment.GetEnvironmentVariable("ClientSecretFromAKV");

        [FunctionName("AIPUserActivity")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            log.LogInformation("Function called by {UserName}", name);

            var workspaceId = LAWorkSpaceIDAKV;
            var clientId = clientIDAKV;
            var clientSecret = clientSecretAKV;

            var domain = "AZURE_AD_DOMAIN_NAME";
            var authEndpoint = "https://login.microsoftonline.com";
            var tokenAudience = "https://api.loganalytics.io/";

            var adSettings = new ActiveDirectoryServiceSettings
            {
                AuthenticationEndpoint = new Uri(authEndpoint),
                TokenAudience = new Uri(tokenAudience),
                ValidateAuthority = true
            };

            var creds = ApplicationTokenProvider.LoginSilentAsync(domain, clientId, clientSecret, adSettings).GetAwaiter().GetResult();
            var client = new OperationalInsightsDataClient(creds)
            {
                WorkspaceId = workspaceId
            };

            // Log Analytics Kusto query - look for data in the past 10 days
            string query = @"
                InformationProtectionLogs_CL
                | where TimeGenerated >= ago(10d)
                | where UserId_s == 'USERNAME@DOMAIN.COM'
                | where ProtectionOwner_s == 'USERNAME@DOMAIN.COM'
                | where Protected_b == 'true'
                | where ObjectId_s != 'document1'
                | where MachineName_s != '' 
                | where ApplicationName_s != 'Outlook'
                | extend FileName = extract('((([a-zA-Z0-9\\s_:]*\\.[a-z]{1,4}$))|([a-zA-Z0-9\\s_:]*$))', 1, ObjectId_s)
                | distinct FileName, Activity_s, LabelName_s, TimeGenerated, Protected_b, MachineName_s
                | sort by TimeGenerated desc nulls last";

            // update the query with caller user's email
           string query1 = query.Replace("USERNAME@DOMAIN.COM", name);

           var outputTable = client.Query(query1.Trim()).Tables[0];

            // Return results to calling agent as a table
            return name != null
                ? (ActionResult)new OkObjectResult(outputTable)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
