using APIOIDCWindows;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Configuration for Azure AD
var config = builder.Configuration.GetSection("AzureAd").Get<AzureAdConfig>();
if(config is null)
{
    throw new InvalidOperationException("AzureAd configuration is missing");
}

// Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies"; // Default to cookies for persistence
    options.DefaultChallengeScheme = "MicrosoftOidc"; // 
})
.AddNegotiate() // Negotiate (Windows Authentication)
.AddOpenIdConnect("MicrosoftOidc", oidcOptions =>
{
    oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    oidcOptions.CallbackPath = new PathString("/signin-oidc");
    foreach (var scope in config.Scopes)
    {
        oidcOptions.Scope.Add(scope);
    }
    oidcOptions.Authority =  $"{config.Instance}/{config.TenantId}/v2.0/";
    oidcOptions.ClientId = config.ClientId;
    oidcOptions.ClientSecret = config.ClientSecret;

    oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
    oidcOptions.MapInboundClaims = false;
    oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
    oidcOptions.TokenValidationParameters.RoleClaimType = "role";
    oidcOptions.SaveTokens = true;
}).AddCookie("Cookies")
.AddJwtBearer("Bearer", options =>
{
    options.Authority =  $"{config.Instance}/{config.TenantId}";
    options.Audience = $"api://{config.ClientId}";
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("APIScope", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var scopeClaim = context.User.FindFirst(ClaimConstants.Scope);
            //Change the scope name as needed
            return scopeClaim != null && scopeClaim.Value.Split(' ').Contains("Api");

        });
    });
}
);
builder.Services.AddRequiredScopeAuthorization();
// Add Swagger and include security definitions
builder.Services.AddSwaggerGen(setup =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter your **JWT Bearer** token in the textbox below (do not include 'Bearer ').",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    // Add operation filter to conditionally apply the scheme
    setup.OperationFilter<JwtAuthenticationOperationFilter>();
});

var app = builder.Build();

IdentityModelEventSource.ShowPII = true;

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// Example endpoints
app.MapGet("/", async (HttpContext ctx) =>
{
    var token = await ctx.GetTokenAsync("access_token");
    Debug.WriteLine(token);
    return !string.IsNullOrEmpty(token) ? token : "No access token found, go to /login";
});

app.MapGet("/login", () =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/swagger" },
        authenticationSchemes: new List<string> { "MicrosoftOidc" }
    ));


app.Run();
