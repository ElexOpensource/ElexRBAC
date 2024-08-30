using RbacDashboard;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage(Justification = "Main program cannot be included in test cases.")]
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ServicesBuilderExtensions._enableSwagger = true;
        builder.Services.AddAuthorization(options => { options.AddRbacPolicy(); });
        builder.Services.AddRequiredService(builder.Configuration, builder.Environment);
        var app = builder.Build();
        app.UseRbac();
        app.Run();
    }
}