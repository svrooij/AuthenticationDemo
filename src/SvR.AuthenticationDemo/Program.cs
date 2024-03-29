using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SvR.AuthenticationDemo.Authentication.ApiKey;
using SvR.AuthenticationDemo.Authentication.BasicAuth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication()
    // This is a demo implementation of basic auth, it is not secure and should not be used in production!
    .AddBasicAuth("Hello", "World")
    // This is a demo implementation of API key auth, it is not secure and should not be used in production!
    .AddApiKey(new string[] { "SuperSecretKey", "ESPC23", "BitBash" })


    // This is all it takes to add JWT authentication to your application.
    // It's really that easy!
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // Read the JWT configuration from the appsettings.json file
        // Check out the 'JWT' section in the appsettings.json file.
        builder.Configuration.Bind("JWT", options);

        // Just some default settings, you don't want overriden by the appsettings.json file.
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(20);
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.RequireSignedTokens = true;
        options.TokenValidationParameters.RequireExpirationTime = true;
        options.TokenValidationParameters.NameClaimType = "name";
    })
    ;

builder.Services.AddAuthorization(options =>
{
    // Add the schemes you want to use to the default authorization policy. This is needed if you have more then one authentication scheme added.
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        BasicAuthDefaults.DefaultScheme,
        ApiKeyDefaults.DefaultScheme,
        JwtBearerDefaults.AuthenticationScheme
    );

    // Override the default authentication scheme to use all configured authentication schemes
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder
        .RequireAuthenticatedUser()
        .Build();

    var secureAppPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        // It's good to always add this, the other rules implicitly state this requirement.
        .RequireAuthenticatedUser()

        // Require that the application uses certificates for authentication.
        // The azpacr claim with value 2 is added to the token if the application uses certificates (or managed identity) for authentication
        // The azpacr claim with value 1 is added to the token if the application uses a client secret for authentication
        //.RequireClaim("azpacr", "2")

        // Be sure to check the roles claim!
        // ANY application can get a token for any api in your tenant without any consent!
        // but the token without consent does not contain any roles.
        .RequireRole("OwnerDetails.All")
    .Build();

    options.AddPolicy("SecureApp", secureAppPolicy);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // Describe the API version (the description is optional, the version is required)
    swagger.SwaggerDoc("v1", new()
    {
        Title = "BitBash - Authentication demo",
        Version = "v1",
        Description = "A demo API to show how JWT authentication works in .NET7 by Stephan van Rooij",
        Contact = new()
        {
            Url = new Uri("https://github.com/svrooij/AuthenticationDemo"),
            Name = "Authentication demo repository"
        },
        License = new()
        {
            Name = "MIT",
            Url = new Uri("https://github.com/svrooij/AuthenticationDemo/blob/main/LICENSE.txt")
        }
    });

    // Add the XML comments to the swagger documentation (optional)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    swagger.IncludeXmlComments(xmlPath, true);

    #region Swagger Basic Auth
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
    #endregion

    #region Swagger API Key Auth
    // Add API key auth to swagger documentation
    swagger.AddSecurityDefinition(ApiKeyDefaults.DefaultScheme, new() { Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = "X-API-Key" });

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
    #endregion

    #region Swagger JWT Auth
    // Add JWT auth to swagger documentation
    // you would only need this if you want to test the API with the swagger UI.
    // If you don't add this, you can still use the API with a JWT token, but you need to add the Authorization header manually.
    // and by not adding this you would make testing a lot harder.
    swagger.AddSecurityDefinition("JWT", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            AuthorizationCode = new OpenApiOAuthFlow()
            {
                // Copied from the Entra portal under the app registration -> overview -> endpoints
                AuthorizationUrl = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/token"),
                // Specify which app you want to request access to, this is one of the scopes you defined in the app registration for the API (resource application).
                Scopes = new Dictionary<string, string>()
                {
                    { "api://c47ece9b-5c83-49ea-82fd-71df69074581/user_impersonation", "Access the API as a user" }
                }
            }
        }
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "JWT" }
            },
            new string[] { }
        }
    });
    #endregion
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Always enable swagger, just for demo purposes.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(swaggerUi =>
    {
        // Pre-fill the swagger UI with the correct client id.
        // This is completely optional, but it makes testing a lot easier.
        swaggerUi.OAuthClientId("366faa7a-25d3-414e-af67-ecb8dd6b6670");

        // Enable PKCE, this is required for the authorization code flow.
        // Let's say you have a SPA application, you can't keep a secret in a SPA application.
        // that is why they invented PKCE, it's a way to "authenticate" without a secret.
        // More info: https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow
        swaggerUi.OAuthUsePkce();

        // Add the scopes that should be requested when the user tries to login.
        swaggerUi.OAuthScopes("api://c47ece9b-5c83-49ea-82fd-71df69074581/user_impersonation", "openid", "profile");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers()
    //.RequireAuthorization()
    ;

// Redirect the root to the swagger UI, and hide from swagger itself.
app.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();

app.Run();
