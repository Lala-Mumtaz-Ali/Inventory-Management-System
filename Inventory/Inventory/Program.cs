using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Inventory.Data;
using Inventory.Models;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Repository;
using Inventory.Repository.Services;
using MySql.Data.MySqlClient;
using AutoMapper;
using Org.BouncyCastle.Crypto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Inventory_Connection"),
        new MySqlServerVersion(new Version(8, 0, 27))));

builder.Services.AddSingleton<MySqlConnection>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("Inventory_Connection"); // Assuming you have a connection string in appsettings.json
    return new MySqlConnection(connectionString);
});

var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<EmployeeDTO, Employees>()
       .ForAllMembers(options =>
           options.Condition((source, destination, sourceMember) => sourceMember != null));

    cfg.CreateMap<ProductUpdateDTO, Product>()
        .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))  // Map Name only if it's not null
        .ForMember(dest => dest.Current_Price, opt => opt.Condition(src => src.Current_Price.HasValue)) // Map Price only if it's not null
        .ForMember(dest => dest.Low_Stock_Threshold, opt => opt.Condition(src => src.Low_Stock_Threshold.HasValue)) // Map LowStockThreshold only if it's not null
        .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))  // Map Description only if it's not null
        .ForMember(dest => dest.Id, opt => opt.Ignore())  // Ignore Id
        .ForMember(dest => dest.Category_id, opt => opt.Ignore());  // Ignore Category_id

});

// Register the mapper
builder.Services.AddSingleton<IMapper>(config.CreateMapper());

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<ITransactionsRepository, WTransactionServices>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

// Add Authentication & JWT Bearer Middleware
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["role"] = ClaimTypes.Role;

// Register AuthService
builder.Services.AddScoped<AuthService>(provider =>
    new AuthService(provider.GetRequiredService<IEmployeeRepository>(),
                    provider.GetRequiredService<AppDbContext>(),
                    jwtKey!,
                    jwtIssuer!));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Configure the HTTP request pipeline.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Add JWT Bearer Security definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your JWT token"
    });

    // Add a security requirement to each endpoint
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


var app = builder.Build();

// Use Swagger in Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

// Use CORS before UseRouting()
app.UseCors("AllowAngularApp");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
