# Dynamic Service Registration Library .NET Core
This library provides a modular and flexible approach for dynamically registering services in a .NET Core application. By breaking down the registration logic into specialized classes, the library enhances code maintainability and reusability. It supports configuration for services, databases, identity, repositories, and JWT authentication in a clear, manageable way.

## Features
- **Modular Service Registration**: Register services, repositories, identity, and database contexts in separate classes for better maintainability.
- **Flexible Database Configuration**: Supports database context registration with DbContext.
- **Identity and Authentication**: Easily set up Identity Core and JWT authentication.
- **Scoped and Transient Services**: Supports registration of scoped and transient services.


## Installation
Install the package via NuGet:
```bash
 NuGet\Install-Package Gurung.ServicesRegister
```

## Usage

#### Program.cs
Here is an example of how to use the library in your `Program.cs`:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// Register services using the dynamic service registration library
builder.Services.AddServiceRegister(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.Run();
```
# How It Works
The library supports two main ways to organize and modularize service registration:
##  Decentralized (Modular) Service Registration:

The library allows you to split the service registration logic into separate classes based on specific concerns, such as:

- **Database Registration**
- **Identity and Authentication**
- **Repository Registration**
- **Service-Specific Registration**
- **Service-Specific Registration with Configuration**
#### Example of Modular Registrations:
#### 1. DatabaseRegistration:
This class registers essential services like `HttpContextAccessor`and JWT settings
```csharp
public class ServiceRegistration : IServicesRegistrationWithConfig
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter(c => c.WithValidatorLifetime(ServiceLifetime.Scoped));
        services.AddHttpContextAccessor();
        services.Configure<JwtTokenSetting>(configuration.GetSection("JwtSettings"));
        services.AddScoped<TenantMigrationService>();
        RepositoryServiceRegistration.ConfigureServices(services);
    }
}
```
#### 2. DatabaseRegistration:
This class is responsible for registering the database contexts.
```csharp
public class DatabaseRegistration : IDbServiceRegistration
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MainDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName)));
        services.AddDbContextFactory<TenantDbContext>();
    }
}
```
#### 3. IdentityRegistration:
This class configures Identity Core and authentication mechanisms, including JWT and cookie authentication.
Here is an example of how to use the library in your `Program.cs`:
```csharp
public class IdentityRegistration : IIdentityServicesRegistration
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
    }
}
```

#### 4. RepositoryRegistration:
This class registers repositories needed for your application.
```csharp
public class RepositoryRegistration : IRepositoriesRegistration
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<ITenantsRepository, TenantsRepository>();
    }
}
```
#### 5. ServicesRegistration:
This class registers other application services.
```csharp
public class ServicesRegistration : IServicesRegistration
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<ITenantUserManagementService, TenantUserManagementService>();
        services.AddTransient<ITokenService, TokenService>();
    }
}
```
#  Centralized Service Registration in a Single Class

#### 1. ServicesRegistration:
All the services, such as database contexts, identity, repositories, and authentication, can be registered centrally in a single class.
#### Example:
```csharp
public class ServiceRegistration : IServicesRegistrationWithConfig
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter(c => c.WithValidatorLifetime(ServiceLifetime.Scoped));
        services.AddHttpContextAccessor();  

        ConfigureDbContext(services, configuration);
        services.ConfigureIdentity(configuration);

        services.Configure<JwtTokenSetting>(configuration.GetSection("JwtSettings"));

        services.AddScoped<TenantMigrationService>();
        RepositoryServiceRegistration.ConfigureServices(services);

        AddServices(services);
        AddRepositories(services);
    }

    internal IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<ITenantUserManagementService, TenantUserManagementService>();
        services.AddScoped<ITenancyManagerService, TenancyManagerService>();
        services.AddTransient<ITokenService, TokenService>();
        return services;
    }

    internal void AddRepositories(IServiceCollection services)
    {
        services.AddTransient<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<ITenantsRepository, TenantsRepository>();
    }

    internal IServiceCollection ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantContextProvider, TenantContextProvider>();
        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName));
        });
        services.AddDbContextFactory<TenantDbContext>();
        services.AddScoped<ITenantDbContext>(provider => provider.GetRequiredService<TenantDbContext>());
        return services;
    }
}

```
#### 2. Identity Configuration:
The identity configuration is extracted into an extension method to simplify the service registration process in `ServiceRegistration`.
```csharp
public static class IdentityConfigureServiceExtension
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();

        services.AddIdentityCore<TenantUser>()
                .AddSignInManager<SignInManager<TenantUser>>()
                .AddRoles<TenantRole>()
                .AddEntityFrameworkStores<TenantDbContext>()
                .AddDefaultTokenProviders();

        services.AddAuthorization();
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(IdentityConstants.ApplicationScheme, o =>
        {
            o.LoginPath = new PathString("/Account/Login");
            o.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            };
        })
        .AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        return services;
    }
}

```


## Contributing
Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

## License

This package is free to use for any purpose.
