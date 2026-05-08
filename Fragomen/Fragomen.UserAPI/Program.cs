using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault configuration provider
// This will load secrets from Key Vault and override configuration values
// DefaultAzureCredential supports multiple authentication methods:
// - Managed Identity (when deployed to Azure)
// - Azure CLI (for local development)
// - Visual Studio (for local development)
var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential(),
        new AzureKeyVaultConfigurationOptions());
}

// Connection string will be loaded from:
// - Azure Key Vault in production (when KeyVaultName is configured)
// - appsettings.Development.json for local development
var userDbConnectionString = builder.Configuration.GetConnectionString("UserAPIConnectionString");
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(userDbConnectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add CORS policy for local development (allow the client origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:53514") // adjust port if your client runs on a different port
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User.API v1");
    c.RoutePrefix = "swagger";
});

// Enable CORS using the named policy
app.UseCors("DevCorsPolicy");

app.UseAuthorization();
app.MapControllers();

app.Run();

