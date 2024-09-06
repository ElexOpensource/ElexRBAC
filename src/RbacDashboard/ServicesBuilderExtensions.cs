using Radzen;
using RbacDashboard.DAL;
using RbacDashboard.BAL;
using RbacDashboard.Common;
using Microsoft.OpenApi.Models;
using RbacDashboard.Components;
using Microsoft.AspNetCore.Identity;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;
using System.Diagnostics.CodeAnalysis;
using RbacDashboard.Common.ClientService;
using Microsoft.AspNetCore.Authorization;
using RbacDashboard.Common.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using RbacDashboard.DAL.Base;
using RbacDashboard.Asserts;
using RbacDashboard.DAL.Enum;

namespace RbacDashboard;

/// <summary>
/// Provides extension methods to add and configure services for the RBAC system.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Main program cannot be included in test cases.")]
public static class ServicesBuilderExtensions
{
    private static readonly string _swaggerVersion = $"Build - {File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location):MM.dd.yyyy.HH.mm}";
    private static string _prefix = string.Empty;
    public static bool _enableSwagger = false;

    /// <summary>
    /// Adds required RBAC services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration object.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddRbacService([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration, [NotNull] IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        _enableSwagger = false;
        services.AddRequiredService(configuration, environment);
        return services;
    }

    /// <summary>
    /// Configures the application to use RBAC with the specified prefix path.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="prefixPath">The prefix path for RBAC routes.</param>
    /// <returns>The modified application builder.</returns>
    public static IApplicationBuilder UseRbac([NotNull] this IApplicationBuilder builder, [NotNull] string prefixPath = "/RbacDashboard")
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefixPath);

        _prefix = prefixPath;
        builder.Map(prefixPath, subApp => subApp.UseRbac());

        return builder;
    }

    /// <summary>
    /// Adds the RBAC authorization policy.
    /// </summary>
    /// <param name="authorizationOptions">The authorization options.</param>
    public static void AddRbacPolicy([NotNull] this AuthorizationOptions authorizationOptions)
    {
        ArgumentNullException.ThrowIfNull(authorizationOptions);

        var RbacPolicy = new AuthorizationPolicyBuilder(RbacConstants.AuthenticationSchema)
            .RequireAuthenticatedUser()
            .Build();

        authorizationOptions.AddPolicy(RbacConstants.AuthenticationSchema, RbacPolicy);
    }

    /// <summary>
    /// Adds the required services for RBAC.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration object.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The modified service collection.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the RBAC policy is not found in the <see cref="AuthorizationOptions"/>.
    /// </exception>
    internal static IServiceCollection AddRequiredService([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration, [NotNull] IWebHostEnvironment environment)
    {
        configuration.EnsureConfigurationKeysPresent();
        services.EnsureRbacPolicyExists();

        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

        services.AddControllers();
        services.AddRadzenComponents();

        #region HttpClient
        services.AddHttpClient();
        services.AddHttpClient("Rbac", client => { client.BaseAddress = new Uri(configuration["RbacSettings:BaseUrl"]!); })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false })
            .AddHeaderPropagation(o => o.Headers.Add("Cookie"));
        services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));

        services.AddSingleton(configuration);
        services.AddSingleton(environment);
        services.AddScoped<RbacApiService>();
        #endregion

        #region DB and CQRS
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ServicesBuilderExtensions).Assembly));
        services.AddDBService(new RbacDbServiceObject
        {
            ConnectionString = configuration["RbacSettings:DbConnectionString"]!,
            DbType = configuration.GetDbType("RbacSettings:DbType")
        });
        #endregion

        #region Authentication
        services.AddAuthentication(RbacConstants.AuthenticationSchema)
            .AddCookie(RbacConstants.AuthenticationSchema, options =>
            {
                options.LoginPath = new PathString("/RbacLogin");
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

        services.AddIdentity<RbacApplicationUser, IdentityRole>()
                .AddUserStore<RbacUserStore>()
                .AddRoleStore<RbacRoleStore>()
                .AddDefaultTokenProviders();

        services.AddScoped<RbacSecurityService>();
        services.AddScoped<IUserStore<RbacApplicationUser>, RbacUserStore>();
        services.AddScoped<SignInManager<RbacApplicationUser>, RbacSignInManager>();
        services.AddScoped<AuthenticationStateProvider, RbacAuthenticationStateProvider>();
        #endregion

        #region Repositories and Services
        services.AddTransient<IMediatorService, MediatorService>();
        services.AddTransient<IRbacTokenRepository, TokenRepository>();
        services.AddTransient<IRbacApplicationRepository, ApplicationRepository>();
        services.AddTransient<IRbacRoleRepository, RoleRepository>();
        services.AddTransient<IRbacMasterRepository, MasterRepository>();
        services.AddTransient<IRbacAccessRepository, AccessRepository>();
        services.AddTransient<IRbacRoleAccessRepository, RoleAccessRepository>();
        services.AddTransient<IRbacAccessTokenRepository, AccessTokenRepository>();
        #endregion

        #region Swagger
        if (_enableSwagger && _prefix == string.Empty)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("rbac", new OpenApiInfo
                {
                    Title = "RBAC API",
                    Version = $"RBAC API V1 {_swaggerVersion}"
                });

                c.DocInclusionPredicate((docName, api) =>
                {
                    if (docName == "rbac" && api.GroupName == "Rbac") return true;
                    return false;
                });
            });
        }
        #endregion

        return services;
    }

    /// <summary>
    /// Configures the application to use RBAC services and middlewares.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The modified application builder.</returns>
    internal static IApplicationBuilder UseRbac([NotNull] this IApplicationBuilder builder)
    {
        builder.UseHttpsRedirection();
        builder.UseHeaderPropagation();
        builder.UseStaticFiles();
        builder.UseRouting();
        builder.UseAuthentication();
        builder.UseAuthorization();
        builder.UseAntiforgery();

        builder.UseEndpoints(builder =>
        {
            builder.MapRazorComponents<App>().AddInteractiveServerRenderMode();
            builder.MapControllers();
        });

        builder.UseMiddleware<RbacEmbeddedFileMiddleware>();

        if (_enableSwagger && _prefix == string.Empty)
        {
            builder.UseSwagger();
            builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/rbac/swagger.json", $"RBAC API V1 {_swaggerVersion}");
            });
        }
        builder.ApplyMigrationsAndSeedData();
        return builder;
    }

    /// <summary>
    /// Ensures that all required configuration keys are present.
    /// </summary>
    /// <param name="configuration">The configuration object.</param>
    private static void EnsureConfigurationKeysPresent(this IConfiguration configuration)
    {
        List<string> keys = [
            "RbacSettings:BaseUrl",
            "RbacSettings:DbConnectionString",
            "RbacSettings:DbType",
            "RbacSettings:Jwt:ValidIssuer",
            "RbacSettings:Jwt:ValidAudience",
            "RbacSettings:Jwt:IssuerSigningKey",
        ];

        List<string> missingKeys = [];

        foreach (var key in keys)
        {
            if (configuration[key] is null)
            {
                missingKeys.Add(key);
            }
        }

        if (missingKeys.Count > 0)
        {
            throw new ArgumentNullException($"The following configuration keys are missing: {string.Join(", ", missingKeys)}");
        }
    }

    /// <summary>
    /// Gets the database type from the configuration.
    /// </summary>
    /// <param name="configuration">The configuration object.</param>
    /// <param name="key">The configuration key for the database type.</param>
    /// <returns>The database type.</returns>
    private static RbacDbType GetDbType(this IConfiguration configuration, string key)
    {
        var dbTypeString = configuration[key];
        if (Enum.TryParse(typeof(RbacDbType), dbTypeString, out var dbType))
        {
            return (RbacDbType)dbType;
        }

        throw new Exception($"Invalid DbType in configuration: {dbTypeString}");
    }

    /// <summary>
    /// Ensures that the RBAC policy is registered in the <see cref="IAuthorizationPolicyProvider"/>.
    /// If the policy is not found, throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to check for the RBAC policy.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the RBAC policy is not found in the <see cref="IAuthorizationPolicyProvider"/>.
    /// </exception>
    private static void EnsureRbacPolicyExists(this IServiceCollection services)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();
            var policyProvider = serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>();

            const string policyName = RbacConstants.AuthenticationSchema;
            _ = policyProvider.GetPolicyAsync(policyName).Result
                ??
                throw new InvalidOperationException();
        }
        catch
        {
            throw new InvalidOperationException($"The policy '{RbacConstants.AuthenticationSchema}' was not found in the service collection. Please add the RBAC policy using `AddRbacPolicy` before using `AddRbacService` for registering other services.");
        }
    }
}