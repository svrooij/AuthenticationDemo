using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SvR.AuthenticationDemo.Authentication.ApiKey;
using SvR.AuthenticationDemo.Authentication.BasicAuth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("FAKE")
    // This is a demo implementation of basic auth, it is not secure and should not be used in production!
    .AddBasicAuth("Hello", "World")
    // This is a demo implementation of API key auth, it is not secure and should not be used in production!
    .AddApiKey(new string[] { "SuperSecretKey", "Application1", "ESPC23" })
    ;

builder.Services.AddAuthorization(options =>
{
    // Add the schemes you want to use to the default authorization policy. This is needed if you have more then one authentication scheme added.
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        BasicAuthDefaults.DefaultScheme,
        ApiKeyDefaults.DefaultScheme
    );

    // Override the default authentication scheme to use all configured authentication schemes
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new() { Title = "Authentication demo", Version = "v1" });

    // Add basic auth to swagger documentation
    swagger.AddSecurityDefinition("Basic", new() { Type = SecuritySchemeType.Http, Scheme = "Basic" });

    // Add basic auth to each endpoint
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
            },
            new string[] { }
        }
    });

    // Add API key auth to swagger documentation
    swagger.AddSecurityDefinition(ApiKeyDefaults.DefaultScheme, new () { Type = SecuritySchemeType.ApiKey , In = ParameterLocation.Header, Name = "X-API-Key" });

    // Add API key auth to each endpoint
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeyDefaults.DefaultScheme }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
