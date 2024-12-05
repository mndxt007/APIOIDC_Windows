
This is sample that demonstrates Windows Auth and JWTBearer auth together - [mndxt007/APIOIDC_Windows](https://github.com/mndxt007/APIOIDC_Windows)

-   Details:
    
    -   Register both Negotiate and JwtBearer Auhentication Provider (optionally OIDC for interactive logon).
        
        -   [Configure Windows Authentication in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-9.0&tabs=visual-studio)
            
        -   [JWT Validation and Authorization in ASP.NET Core - .NET Blog](https://devblogs.microsoft.com/dotnet/jwt-validation-and-authorization-in-asp-net-core/)
            
            -   You can also Identity Web Library which abstracts the setup of JWTBearer
                
                -   [Code samples for Microsoft identity platform authentication and authorization - Microsoft identity platform | Microsoft Learn](https://learn.microsoft.com/en-us/entra/identity-platform/sample-v2-code?tabs=apptype)
                    
                
    -   Use different authentication schemes in the controller - [Authorize with a specific scheme in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-9.0)
        
    -   Use enforce Authorization policies and token validation parameters for additional security (such as validating scopes)
        
        -   [Verify scopes and app roles protected web API - Microsoft identity platform | Microsoft Learn](https://learn.microsoft.com/en-us/entra/identity-platform/scenario-protected-web-api-verification-scope-app-roles?tabs=aspnetcore)
            
        -   In the sample - [APIOIDC_Windows/APIOIDCWindows/Controllers/OIDCUserController.cs at master · mndxt007/APIOIDC_Windows](https://github.com/mndxt007/APIOIDC_Windows/blob/master/APIOIDCWindows/Controllers/OIDCUserController.cs) uses an Authorization policy to validate scope.
            

Client flows for acquiring tokens:

-   Authorization Code flow - [Microsoft identity platform and OAuth 2.0 authorization code flow - Microsoft identity platform | Microsoft Learn](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow)
    
-   RPOC Flow (not recommended) - [Microsoft identity platform and OAuth 2.0 Resource Owner Password Credentials - Microsoft identity platform | Microsoft Learn](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth-ropc)
    
    -   I've created client sample for RPOC Flow - [APIOIDC_Windows/RPOCFlow at master · mndxt007/APIOIDC_Windows](https://github.com/mndxt007/APIOIDC_Windows/tree/master/RPOCFlow)
        
        -   This is referred from [Azure-Samples/active-directory-dotnetcore-console-up-v2: A .NET Core console application which gets an access token to call Microsoft Graph using a username and password](https://github.com/azure-samples/active-directory-dotnetcore-console-up-v2)
            
-   Client Credentials flow - [OAuth 2.0 client credentials flow on the Microsoft identity platform - Microsoft identity platform | Microsoft Learn](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-client-creds-grant-flow)
    
    -   I've created client sample for Client Crendential Flow - [APIOIDC_Windows/ClientCredentialsFlow at master · mndxt007/APIOIDC_Windows](https://github.com/mndxt007/APIOIDC_Windows/tree/master/ClientCredentialsFlow)
        
        -   This is referred from - [active-directory-dotnetcore-daemon-v2/2-Call-OwnApi at master · Azure-Samples/active-directory-dotnetcore-daemon-v2](https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/tree/master/2-Call-OwnApi)
            

Public client flows like RPOC will need additional setting in Azure App Registration.

-   [Public and confidential client apps (MSAL) - Microsoft identity platform | Microsoft Learn](https://learn.microsoft.com/en-us/entra/identity-platform/msal-client-applications)
