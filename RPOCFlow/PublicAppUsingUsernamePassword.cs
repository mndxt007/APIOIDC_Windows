﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPOCFlow
{
    public class PublicAppUsingUsernamePassword
    {
        /// <summary>
        /// Constructor of a public application leveraging username passwords to acquire a token
        /// </summary>
        /// <param name="app">MSAL.NET Public client application</param>
        /// <remarks>
        /// For more information see https://aka.ms/msal-net-up
        /// </remarks>
        public PublicAppUsingUsernamePassword(IPublicClientApplication app)
        {
            App = app;
        }

        /// <summary>
        /// IPublicClientApplication
        /// </summary>
        protected IPublicClientApplication App { get; private set; }

        /// <summary>
        /// Acquires a token from the token cache, or Username/password
        /// </summary>
        /// <returns>An AuthenticationResult if the user successfully signed-in, or otherwise <c>null</c></returns>
        public async Task<AuthenticationResult> AcquireATokenFromCacheOrUsernamePasswordAsync(IEnumerable<String> scopes, User user)
        {
            AuthenticationResult result = null;
            var accounts = await App.GetAccountsAsync().ConfigureAwait(false);

            if (accounts.Any())
            {
                try
                {
                    // Attempt to get a token from the cache (or refresh it silently if needed)
                    result = await (App as PublicClientApplication).AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync().ConfigureAwait(false);
                }
                catch (MsalUiRequiredException)
                {
                    // No token for the account. Will proceed below
                }
            }

            // Cache empty or no token for account in the cache, attempt by username/password
            if (result == null)
            {
                result = await GetTokenForWebApiUsingUsernamePasswordAsync(scopes, user.UserName, user.Password).ConfigureAwait(false);
            }

            return result;
        }

        internal async Task CallWebApiAndProcessResultASync(string? apiEndpoint, string accessToken)
        {
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                throw new ArgumentNullException(nameof(apiEndpoint), "API endpoint cannot be null or empty.");
            }

            using var httpClient = new HttpClient();
            var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }

            defaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine($"\nMaking HTTP Call to {apiEndpoint}");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint).ConfigureAwait(false);

                Console.WriteLine($"Response Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Console.WriteLine($"Response Body: {responseBody}");
                }
                else
                {
                    string errorBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Console.WriteLine($"Error Body: {errorBody}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Request timed out: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }

        }

        /// <summary>
        /// Gets an access token so that the application accesses the web api in the name of the user
        /// who is signed-in Windows (for a domain joined or AAD joined machine)
        /// </summary>
        /// <returns>An authentication result, or null if the user canceled sign-in</returns>
        private async Task<AuthenticationResult> GetTokenForWebApiUsingUsernamePasswordAsync(IEnumerable<string> scopes, string username, string password)
        {
            AuthenticationResult result = null;
            try
            {
                result = await App.AcquireTokenByUsernamePassword(scopes, username, password)
                    .ExecuteAsync().ConfigureAwait(false);
            }
            catch (MsalUiRequiredException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                // Here are the kind of error messages you could have, and possible mitigations

                // ------------------------------------------------------------------------
                // MsalUiRequiredException: 'AADSTS50055: Password is expired.
                // error:invalid_grant
                // suberror:user_password_expired
                // Mitigation: you need to have the user change their password first. This
                // requires an interaction with Azure AD, which is not possible with the username/password flow)
                // if you are not using .NET Core (which does not have any Web UI) by calling (once only) AcquireTokenAsync interactive. 
                // remember that Username/password is for public client applications that is desktop/mobile applications.
                // If you are using .NET core or don't want to call AcquireTokenAsync, you might want to:
                // - use device code flow (See https://aka.ms/msal-net-device-code-flow)
                // - or suggest the user to navigate to a URL to consent: https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={clientId}&response_type=code&scope=user.read
                //   where the user will be prompted to change their password
                // ------------------------------------------------------------------------

                // ------------------------------------------------------------------------
                // MsalUiRequiredException: AADSTS65001: The user or administrator has not consented to use the application 
                // error:invalid_grant
                // suberror:consent_required
                // with ID '{appId}' named '{appName}'. Send an interactive authorization request for this user and resource.
                // Mitigation: you need to get user consent first. This can be done either statically (through the portal), or dynamically (but this
                // requires an interaction with Azure AD, which is not possible with the username/password flow)
                // Statically: in the portal by doing the following in the "API permissions" tab of the application registration: 
                // 1. Click "Add a permission" and add all the delegated permissions corresponding to the scopes you want (for instance
                // User.Read and User.ReadBasic.All)
                // 2. Click "Grant/revoke admin consent for <tenant>") and click "yes".
                // Dynamically, if you are not using .NET Core (which does not have any Web UI) by calling (once only) AcquireTokenAsync interactive. 
                // remember that Username/password is for public client applications that is desktop/mobile applications.
                // If you are using .NET core or don't want to call AcquireTokenAsync, you might want to:
                // - use device code flow (See https://aka.ms/msal-net-device-code-flow)
                // - or suggest the user to navigate to a URL to consent: https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={clientId}&response_type=code&scope=user.read
                // ------------------------------------------------------------------------

                // ------------------------------------------------------------------------
                // ErrorCode: invalid_grant
                // SubError: basic_action
                // MsalUiRequiredException: AADSTS50079: The user is required to use multi-factor authentication.
                // The tenant admin for your organization has chosen to oblige users to perform multi-factor authentication. 
                // Mitigation: none for this flow
                // Your application cannot use the Username/Password grant. 
                // Like in the previous case, you might want to use an interactive flow (AcquireTokenAsync()), or Device Code Flow instead.
                // Note this is one of the reason why using username/password is not recommended;
                // ------------------------------------------------------------------------

                // ------------------------------------------------------------------------
                // ex.ErrorCode: invalid_grant
                // subError: null
                // Message = "AADSTS70002: Error validating credentials. AADSTS50126: Invalid username or password
                // In the case of a managed user (user from an Azure AD tenant opposed to a federated user, which would be owned
                // in another IdP through ADFS), the user has entered the wrong password
                // Mitigation: ask the user to re-enter the password
                // ------------------------------------------------------------------------

                // ------------------------------------------------------------------------
                // ex.ErrorCode: invalid_grant
                // subError: null
                // MsalServiceException: ADSTS50034: To sign into this application the account must be added to the {domainName} directory.
                // or The user account does not exist in the {domainName} directory. To sign into this application, the account must be added to the directory.
                // The user was not found in the directory
                // Explanation: wrong username
                // Mitigation: ask the user to re-enter the username. 
                // ------------------------------------------------------------------------
            }
            catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_request")
            {
                // ------------------------------------------------------------------------
                // AADSTS90010: The grant type is not supported over the /common or /consumers endpoints. Please use the /organizations or tenant-specific endpoint.
                // you used common.
                // Mitigation: as explained in the message from Azure AD, the authority you use in the application needs to be tenanted or otherwise "organizations". change the 
                // "Tenant": property in the appsettings.json to be a GUID (tenant Id), or domain name (contoso.com) if such a domain is registered with your tenant
                // or "organizations", if you want this application to sign-in users in any Work and School accounts.
                // ------------------------------------------------------------------------

            }
            catch (MsalServiceException ex) when (ex.ErrorCode == "unauthorized_client")
            {
                // ------------------------------------------------------------------------
                // AADSTS700016: Application with identifier '{clientId}' was not found in the directory '{domain}'.
                // This can happen if the application has not been installed by the administrator of the tenant or consented to by any user in the tenant. 
                // You may have sent your authentication request to the wrong tenant
                // Cause: The clientId in the appsettings.json might be wrong
                // Mitigation: check the clientId and the app registration
                // ------------------------------------------------------------------------
            }
            catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_client")
            {
                // ------------------------------------------------------------------------
                // AADSTS70002: The request body must contain the following parameter: 'client_secret or client_assertion'.
                // Explanation: this can happen if your application was not registered as a public client application in Azure AD 
                // Mitigation: in the Azure portal, edit the manifest for your application and set the `allowPublicClient` to `true` 
                // ------------------------------------------------------------------------
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "unknown_user_type")
            {
                // Message = "Unsupported User Type 'Unknown'. Please see https://aka.ms/msal-net-up"
                // The user is not recognized as a managed user, or a federated user. Azure AD was not
                // able to identify the IdP that needs to process the user
                throw new ArgumentException("U/P: Wrong username", ex);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "user_realm_discovery_failed")
            {
                // The user is not recognized as a managed user, or a federated user. Azure AD was not
                // able to identify the IdP that needs to process the user. That's for instance the case
                // if you use a phone number
                throw new ArgumentException("U/P: Wrong username", ex);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "unknown_user")
            {
                // the username was probably empty
                // ex.Message = "Could not identify the user logged into the OS. See http://aka.ms/msal-net-iwa for details."
                throw new ArgumentException("U/P: Wrong username", ex);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "parsing_wstrust_response_failed")
            {
                // ------------------------------------------------------------------------
                // In the case of a Federated user (that is owned by a federated IdP, as opposed to a managed user owned in an Azure AD tenant) 
                // ID3242: The security token could not be authenticated or authorized.
                // The user does not exist or has entered the wrong password
                // ------------------------------------------------------------------------
            }
            return result;
        }
    }
}
