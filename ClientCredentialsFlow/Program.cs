using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json;

// Get the Token acquirer factory instance. By default it reads an appsettings.json
// file if it exists in the same folder as the app (make sure that the 
// "Copy to Output Directory" property of the appsettings.json file is "Copy if newer").
var tokenAcquirerFactory = TokenAcquirerFactory.GetDefaultInstance();
// Add console logging or other services if you wish
tokenAcquirerFactory.Services.AddLogging(
    (loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Information)
                                      .AddConsole()
                                        );

// Create a downstream API service named 'MyApi' which comes loaded with several
// utility methods to make HTTP calls to the DownstreamApi configurations found
// in the "MyWebApi" section of your appsettings.json file.
tokenAcquirerFactory.Services.AddDownstreamApi("MyApi",
    tokenAcquirerFactory.Configuration.GetSection("MyWebApi"));
var sp = tokenAcquirerFactory.Build();

// Extract the downstream API service from the 'tokenAcquirerFactory' service provider.
var api = sp.GetRequiredService<IDownstreamApi>();

// You can use the API service to make direct HTTP calls to your API. Token
// acquisition is handled automatically based on the configurations in your
// appsettings.json file.
try
{
    var result = await api.GetForAppAsync<IEnumerable<Claim>>("MyApi");
    if (result != null && result.Any())
    {
        foreach (var claim in result)
        {
            Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }
    }
    else
    {
        Console.WriteLine("No claims found in the response.");
    }
}
catch (JsonException ex)
{
    Console.WriteLine($"JSON conversion error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
Console.WriteLine("Press any key to exit");
Console.ReadKey();


public class Claim
{
    public string Type { get; set; }
    public string Value { get; set; }
}