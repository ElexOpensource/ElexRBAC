using Microsoft.OpenApi.Models;
using RbacDashboard;

var builder = WebApplication.CreateBuilder(args);

#region Rbac
builder.Services.AddAuthorization(options => { options.AddRbacPolicy(); });
builder.Services.AddRbacService(builder.Configuration, builder.Environment);
#endregion

builder.Services.AddControllersWithViews();

#region Swagger
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
#endregion

var app = builder.Build();

#region Rbac
var masterData = ServicesBuilderExtensions.LoadMasterDataJson("MasterData");
app.UseRbac("/Rbac", masterData); 
#endregion

#region Swagger
app.UseSwagger(c =>
{
c.RouteTemplate = "api-docs/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api-docs/default/swagger.json", "Default API");
    c.SwaggerEndpoint("/api-docs/rbac/swagger.json", $"RBAC API {_rbacSwaggerVersion}");
});
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/index.html")
    {
        context.Response.Redirect("/home");
        return;
    }
    await next();
});  

app.Run();
