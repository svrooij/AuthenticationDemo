using Microsoft.OpenApi.Models;
using SvR.AuthenticationDemo.Authentication.BasicAuth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication()
    // This is a demo implementation of basic auth, it is not secure and should not be used in production!
    .AddBasicAuth("Hello", "World")
    ;

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
