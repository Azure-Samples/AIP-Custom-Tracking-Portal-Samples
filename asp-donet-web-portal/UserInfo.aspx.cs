using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using findmydocs.Models;

namespace findmydocs
{
    public partial class UserInfo : System.Web.UI.Page
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientID"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = EnsureTrailingSlash(ConfigurationManager.AppSettings["ida:AADInstance"]);
        private static string graphResourceId = "https://graph.windows.net";

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(GetUserData));
        }

        public Task GetUserData()
        {
            return Task.Run(() =>
            {
                string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                try
                {
                    Uri servicePointUri = new Uri(graphResourceId);
                    Uri serviceRoot = new Uri(servicePointUri, tenantID);
                    ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                          async () => await GetTokenForApplication());

                    // use the token for querying the graph to get the user details
                    IUser user = activeDirectoryClient.Users
                        .Where(u => u.ObjectId.Equals(userObjectID))
                        .ExecuteAsync().Result.CurrentPage.ToList().First();

                    UserData.DataSource = new List<IUser> { user };
                    UserData.DataBind();
                }
                // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
                catch (AdalException)
                {
                    GetToken.Visible = true;
                }
                // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
                catch (Exception)
                {
                    ShowData.Visible = false;
                    GetToken.Visible = true;
                }
            });
        }

        protected void Unnamed_Click(object sender, System.EventArgs e)
        {
            ShowData.Visible = false;
            HttpContext.Current.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/UserInfo" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public async Task<string> GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID, new ADALTokenCache(signedInUserID));
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(graphResourceId, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }

        private static string EnsureTrailingSlash(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith("/", StringComparison.Ordinal))
            {
                return value + "/";
            }

            return value;
        }
    }
}