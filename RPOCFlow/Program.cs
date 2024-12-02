using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using RPOCFlow;
using System.Reflection;

var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
var configuration = builder.Build();
// Bind the configuration section to PublicClientApplicationOptions
var appConfig = new PublicClientApplicationOptions();
appConfig.LogLevel = LogLevel.Info;
appConfig.EnablePiiLogging = true;
configuration.GetSection("Authentication").Bind(appConfig);


var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig)
    .WithLogging((level, message, containsPii) => Console.WriteLine($"MSAL: {level} {message}"))
    .Build();
var user = Helper.UserNamePassWord();

var rpocApp = new PublicAppUsingUsernamePassword(app);
var scopes = configuration.GetSection("WebAPI:Scopes").Get<string[]>();
var token = await rpocApp.AcquireATokenFromCacheOrUsernamePasswordAsync(scopes, user);
Console.WriteLine(token.AccessToken);

//make HTTP call to WebAPI:APIEndpoint
var apiEndpoint = configuration["WebAPI:APIEndpoint"];
await rpocApp.CallWebApiAndProcessResultASync(apiEndpoint, token.AccessToken);