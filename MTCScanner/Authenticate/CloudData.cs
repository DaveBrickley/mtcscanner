using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using MTCScanner.Authenticate;
using MTCScanner.Models;

namespace MTCScanner.Authenticate
{
    public class CloudData
    {
        public static PublicClientApplication ADB2CClient { get; private set; }



        public static string SignUpAndInPolicy = "B2C_1_MMSignUpSignIn";

        public static string EditProfilePolicy = "B2C_1_B2CEditProfile";

        public static readonly string Tenant = "mobilemicrolise.onmicrosoft.com"; // Domain/resource name from AD B2C

        public static readonly string ClientID = "152f50c7-f8e8-4537-939a-3c9b599952a3"; // Application ID from AD B2C


        public static readonly string PolicySignUpSignIn = "B2C_1_MMSignUpSignIn"; // Policy name from AD B2C
        public static string PolicyResetPassword = "B2C_1_B2CPasswordReset";
        public static readonly string[] Scopes = { "https://mobilemicrolise.onmicrosoft.com/backends/user_impersonation" }; // Leave blank unless additional scopes have been added to AD B2C


        //public static string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Tenant}/"; // Doesn't require editing
        public static string AuthorityBase = $"https://mobilemicrolise.b2clogin.com/tfp/{Tenant}/"; // Doesn't require editing


        public static string Authority = $"{AuthorityBase}{SignUpAndInPolicy}"; // Doesn't require editing
        public static string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}"; // Doesn't require editing
        public static string AuthorityEditProfile = $"{AuthorityBase}{EditProfilePolicy}"; // Doesn't require editing

        //private static string BaseAuthority = "https://login.microsoftonline.com/tfp/{tenant}/{policy}/oauth2/v2.0/authorize";
        public static readonly string URLScheme = "msal152f50c7-f8e8-4537-939a-3c9b599952a3"; // Custom Redirect URI from AD B2C (without ://auth/)

        public static readonly string RedirectUri = $"{URLScheme}://auth"; // Doesn't require editing

        public string BaseURI = "https://mobilemicrolise.azurewebsites.net/api/";



        public CloudData()
        {



        }

        HttpClient client = new HttpClient();


        async public Task<string> PostBarcode(string barcode, string notes)

        {

            string SessionToken = App.Current.Properties["UserAccessToken"] as string;
            string uniqueID = App.Current.Properties["uniqueUserID"] as string;
            string StandID = App.Current.Properties["StandID"] as string;
            string Company = App.Current.Properties["Company"] as string;


            string append = "Barcode/New";

            JArray postToAzure = new JArray();

            JObject postContents = new JObject();

            string UTC = DateTime.Now.ToUniversalTime()
                         .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");

            postContents.Add("EMAIL", App.Current.Properties["mail"].ToString());
            postContents.Add("DISPLAY_NAME", App.Current.Properties["displayName"].ToString());
            postContents.Add("BARCODE", barcode);
            postContents.Add("TIMESTAMP", UTC);
            postContents.Add("NOTES", notes);
            postContents.Add("STANDID", StandID);
            postContents.Add("COMPANY", Company);



            postToAzure.Add(postContents);

            var content = new StringContent(postToAzure.ToString(), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionToken);

            var result = client.PostAsync(BaseURI + append, content).Result;

            var contents = await result.Content.ReadAsStringAsync();

            return result.ReasonPhrase;
        }



        async public Task<GraphUser> GetDisplayName()

        {

            string SessionToken = App.Current.Properties["UserAccessToken"] as string;
            string uniqueID = App.Current.Properties["uniqueUserID"] as string;

            System.Diagnostics.Debug.WriteLine("Debug: Called GetDisplayName");


            string append = "GraphAPI/GetUserName";

            JObject postContents = new JObject();
            postContents.Add("GET_USER_ID", uniqueID);
            var content = new StringContent(postContents.ToString(), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionToken);

            var result = client.PostAsync(BaseURI + append, content).Result;

            var contents = await result.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine("Debug: got contents");



            GraphUser userInfo = JsonConvert.DeserializeObject<GraphUser>(contents);

            System.Diagnostics.Debug.WriteLine("Debug: GetDisplayName serialised userInfo " + uniqueID);
            System.Diagnostics.Debug.WriteLine("Debug: GetDisplayName serialised displayName " + userInfo.displayName);
            System.Diagnostics.Debug.WriteLine("Debug: GetDisplayName serialised stand ID " + userInfo.StandID);
            System.Diagnostics.Debug.WriteLine("Debug: GetDisplayName principle name " + userInfo.userPrincipalName);
            System.Diagnostics.Debug.WriteLine("Debug: GetDisplayName company name " + userInfo.Company);



            App.Current.Properties["displayName"] = userInfo.displayName;
            App.Current.Properties["mail"] = userInfo.mail;
            App.Current.Properties["StandID"] = userInfo.StandID;
            App.Current.Properties["Company"] = userInfo.Company;



            System.Diagnostics.Debug.WriteLine("Debug: GetDisplayName saving");


            await App.Current.SavePropertiesAsync();

            return userInfo;

        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.Contains(policy.ToLower())) return account;
            }
            return null;
        }

        private IAccount GetUserByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string accountIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (accountIdentifier.Contains(policy.ToLower())) return account;
            }

            return null;
        }

        async public Task<AuthenticationResult> GetAccessToken(string type)

        {


            System.Diagnostics.Debug.WriteLine("Debug: called GetAccessToken");

            ADB2CClient = new PublicClientApplication(ClientID, Authority);


            System.Diagnostics.Debug.WriteLine("Debug: got to IAccount");

            ADB2CClient.RedirectUri = RedirectUri;

            AuthenticationResult authenticationResult;

            IEnumerable<IAccount> accounts = await ADB2CClient.GetAccountsAsync();
            IAccount firstAccount = accounts.FirstOrDefault();



            try
            {
                authenticationResult = await ADB2CClient.AcquireTokenSilentAsync(Scopes, firstAccount);

                accounts = await ADB2CClient.GetAccountsAsync();
                firstAccount = accounts.FirstOrDefault();
                IAccount me = await ADB2CClient.GetAccountAsync(firstAccount.HomeAccountId.Identifier);

                if (type == "edit")

                {

                    try
                    {


                        //authenticationResult = await ADB2CClient.AcquireTokenAsync(Scopes, firstAccount, UIBehavior.SelectAccount, string.Empty, null, AuthorityEditProfile, App.UiParent);

                        AuthenticationResult authResult = await ADB2CClient.AcquireTokenAsync(Scopes, GetUserByPolicy(accounts, EditProfilePolicy), UIBehavior.NoPrompt, string.Empty, null, AuthorityEditProfile, App.UiParent);




                    }

                    catch (Exception ez)

                    {


                        if (ez.Message == "AADB2C90091")

                        {


                            System.Diagnostics.Debug.WriteLine("Debug: User canceled profile edit");

                            authenticationResult = null;

                        }


                    }



                }

                else
                {

                    App.Current.Properties["UserAccessToken"] = authenticationResult.AccessToken;

                    System.Diagnostics.Debug.WriteLine("Debug: Saving access token " + authenticationResult.AccessToken);

                    App.Current.Properties["uniqueUserID"] = authenticationResult.UniqueId;

                    System.Diagnostics.Debug.WriteLine("Debug: Saving unique user ID " + authenticationResult.UniqueId);

                }

                await App.Current.SavePropertiesAsync();

                return authenticationResult;

            }
            catch (MsalUiRequiredException ex)
            {

                System.Diagnostics.Debug.WriteLine("Debug: no token found in cache, prompting user to log in " + ex.Message);

                try
                {

                    authenticationResult = await ADB2CClient.AcquireTokenAsync(Scopes, firstAccount, App.UiParent);

                    accounts = await ADB2CClient.GetAccountsAsync();
                    firstAccount = accounts.FirstOrDefault();
                    IAccount me = await ADB2CClient.GetAccountAsync(firstAccount.HomeAccountId.Identifier);


                    App.Current.Properties["UserAccessToken"] = authenticationResult.AccessToken;

                    System.Diagnostics.Debug.WriteLine("Debug: Saving access token " + authenticationResult.AccessToken);

                    App.Current.Properties["uniqueUserID"] = authenticationResult.UniqueId;

                    System.Diagnostics.Debug.WriteLine("Debug: Saving unique user ID " + authenticationResult.UniqueId);

                    await App.Current.SavePropertiesAsync();

                    return authenticationResult;
                }

                catch (Exception ey)

                {
                    System.Diagnostics.Debug.WriteLine("Debug: exception type is " + ey.Message);

                    if (ey.Message.Contains("User canceled authentication"))
                    {

                        System.Diagnostics.Debug.WriteLine("Debug: User canceled authentication");

                        authenticationResult = null;

                    }

                    if (ey.Message.Contains("AADB2C90118"))
                    {

                        try

                        {
                            System.Diagnostics.Debug.WriteLine("Debug: User has forgotten password ");

                            authenticationResult = await ADB2CClient.AcquireTokenAsync(Scopes, firstAccount, UIBehavior.SelectAccount, string.Empty, null, AuthorityResetPassword, App.UiParent);

                            accounts = await ADB2CClient.GetAccountsAsync();
                            firstAccount = accounts.FirstOrDefault();
                            IAccount me = await ADB2CClient.GetAccountAsync(firstAccount.HomeAccountId.Identifier);


                            App.Current.Properties["UserAccessToken"] = authenticationResult.AccessToken;

                            System.Diagnostics.Debug.WriteLine("Debug: Saving access token " + authenticationResult.AccessToken);

                            App.Current.Properties["uniqueUserID"] = authenticationResult.UniqueId;

                            System.Diagnostics.Debug.WriteLine("Debug: Saving unique user ID " + authenticationResult.UniqueId);

                            await App.Current.SavePropertiesAsync();

                            return authenticationResult;
                        }

                        catch (Exception ez)

                        {
                            if (ey.Message.Contains("User canceled authentication"))
                            {

                                System.Diagnostics.Debug.WriteLine("Debug: User canceled password reset");

                                authenticationResult = null;

                            }

                            else

                            {
                                System.Diagnostics.Debug.WriteLine("Debug: User canceled password reset " + ez.Message);

                                authenticationResult = null;

                            }


                        }

                    }


                    else

                    {
                        authenticationResult = null;

                    }


                }

            }




            return authenticationResult;



        }


        async public void Logout()

        {
            try
            {

                IEnumerable<IAccount> accounts = await ADB2CClient.GetAccountsAsync();
                IAccount firstAccount = accounts.FirstOrDefault();
                await ADB2CClient.RemoveAsync(firstAccount);

            }

            catch (Exception ex)

            {

                System.Diagnostics.Debug.WriteLine("Error: could not log out " + ex.Message);

            }

        }


    }
}