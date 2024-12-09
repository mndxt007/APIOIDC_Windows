using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;

//referring https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/blob/master/4-WebApp-your-API/4-1-MyOrg/TodoListService/Startup.cs
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Negotiate"; // Default to cookies for persistence
})
.AddNegotiate();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthorization();
app.MapControllers();

app.Run();
