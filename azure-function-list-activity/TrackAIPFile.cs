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

namespace TrackAIPFiles
{
    public static class TrackAIPFile
    {
        // Get WorkspaceID, ClientID and Client Secret from Azure KeyVault
        private static string LAWorkSpaceIDAKV = System.Environment.GetEnvironmentVariable("WorkSpaceIDFromAKV");
        private static string clientIDAKV = System.Environment.GetEnvironmentVariable("ClientIDFromAKV");
        private static string clientSecretAKV = System.Environment.GetEnvironmentVariable("ClientSecretFromAKV");

        [FunctionName("TrackAIPFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            log.LogInformation("AIP Tracking Request for File: {FileName} ", name);

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

            string query = @"let doclookup = InformationProtectionLogs_CL
                | where TimeGenerated >= ago(31d) 
                | where ObjectId_s contains 'AIPFILETOTRACK' and ContentId_g != ''
                | distinct ContentId_g, ObjectId_s;
                let accesslookup = InformationProtectionLogs_CL
                | where Operation_s == 'AcquireLicense' or Activity_s != '';
                   accesslookup
                | join kind = inner(
                   doclookup
                ) on $left.ContentId_g == $right.ContentId_g
                | extend FileName = extract('((([a-zA-Z0-9\\s_:]*\\.[a-z]{1,4}$))|([a-zA-Z0-9\\s_:]*$))', 1, ObjectId_s1)
                | project ContentId_g, FileName, AccessedBy = UserId_s, Activity_s, ProtectionOwner_s, TimeGenerated, ProtectionTime_t, ApplicationName_s, IPv4_s, MachineName_s";

            // update the query with caller user's document to track

            string query1 = query.Replace("AIPFILETOTRACK", name);

            var outputTable = client.Query(query1.Trim()).Tables[0];

            return name != null
                ? (ActionResult)new OkObjectResult(outputTable)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
