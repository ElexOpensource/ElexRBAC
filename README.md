# Rbac Dashboard (Role Based Access Control Dashboard)

## Overview

The **Rbac Dashboard** is a Blazor-based project that provides a user-friendly dashboard UI for managing Role-Based Access Control (RBAC) within your .NET application. With minimal setup, this dashboard allows you to create or modify applications, roles, access, and roleAccess entries. It also integrates seamlessly with an RBAC API and services, enabling you to retrieve permission lists based on provided role IDs.

## Getting Started

To get started with the Rbac Dashboard in your .NET application:

1. **Clone or Download** the project repository.
2. **Follow the integration steps** provided in the "Integration Guide" section.
3. **Start your application**, and navigate to `/Rbac` on the URL to access the dashboard UI.

## Features

- **Comprehensive Dashboard UI**: Manage applications, roles, access controls, and role-access entries effortlessly.
- **Seamless RBAC API Integration**: Seamlessly integrates with your .NET application to handle permissions and role-based access control.
- **Easy Service Setup**: Simplify authentication and permission management with pre-configured services.

## Integration Guide

To integrate the Rbac Dashboard into your application, follow these steps:
1. **Add Dlls**:

    Build the project and include the following DLLs in your application. Ensure they are referenced in your main project:
    - RbacDashboard.dll
    - RbacDashboard.BAL.dll
    - RbacDashboard.DAL.dll
    - RbacDashboard.Common.dll
    - RbacDashboard.Asserts.dll

2. **Configure Authorization Policy**:

    Integrate the RBAC authorization policy into your service collection
    ```csharp
    builder.Services.AddAuthorization(options => { 
        options.AddRbacPolicy(); 
    });
    ```

3. **Register RBAC Services**:

    Register all necessary RBAC services using the following command
    ```csharp
    builder.Services.AddRbacService(builder.Configuration, builder.Environment);
    ```

4. **Add the Dashboard UI**: 

   Incorporate the Rbac Dashboard UI into your application path with
   ```csharp
   app.UseRbac("/Rbac");
    ```

5. **Configure AppSettings**:

    Add the following configuration to your `appsettings.json` or equivalent configuration file
    ```json
    "RbacSettings": {
        "BaseUrl": "Application Base URL",
        "DbConnectionString": "Database connection string (SQL or PostgreSQL)",
        "DbType": "Database type (Sql or PgSql)",
        "Jwt": {
            "ValidIssuer": "Token Valid Issuer",
            "ValidAudience": "Token Valid Audience",
            "IssuerSigningKey": "Token Issuer Signing Key"
        }
    }
    ```
    If you have any middleware to handle response and error handling (`IResultFilter`), you need to exclude the Rbac API by using the base API name `/Rbacapi/`.

6. **Swagger Setup**:

    To include Rbac endpoints in your Swagger API, add the following configuration:
    ```csharp
    string _rbacSwaggerVersion = $"Build - {File.GetLastWriteTime(typeof(RbacDashboard.Asserts.RbacAsserts).Assembly.Location):MM.dd.yyyy.HH.mm}";
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("default", new OpenApiInfo { Title = "Default API", Version = "v1" });
        c.SwaggerDoc("rbac", new OpenApiInfo
        {
            Title = "RBAC API",
            Version = $"RBAC API {_rbacSwaggerVersion}"
        });

        c.DocInclusionPredicate((docName, api) =>
        {
            if (docName == "rbac" && api.GroupName == "Rbac")
                return true;
            if (docName == "default" && (api.GroupName == null || api.GroupName == "default"))
                return true;
            return false;
        });
    });

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api-docs/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api-docs/default/swagger.json", "Default API");
        c.SwaggerEndpoint("/api-docs/rbac/swagger.json", $"RBAC API {_rbacSwaggerVersion}");
    });
    ````

## Technologies Used
- **Blazor**: For building the dashboard UI.
- **ASP.NET Core**: To handle backend services and API integration.
- **Entity Framework Core**: To manage database operations, including data retrieval, storage, and manipulation.
- **Mediator**: To implement the CQRS (Command Query Responsibility Segregation) pattern, ensuring a clean separation between commands (actions that change state) and queries (actions that retrieve data).

## Contributing
If you'd like to contribute to the Rbac Dashboard project, feel free to fork the repository and submit a pull request.