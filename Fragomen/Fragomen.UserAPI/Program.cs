using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// In connection string: Only use Trusted_Connection=True for local development.
// For production, set Trusted_Connection=False and
// provide appropriate credentials in the connection string.
var userDbConnectionString = builder.Configuration.GetConnectionString("UserAPIConnectionString");
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(userDbConnectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

app.UseAuthorization();
app.MapControllers();

app.Run();

