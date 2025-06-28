using BlazeQuartz;
using BlazeQuartz.Core.Services;
using BlazeQuartz.Extensions;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
 

using Quartz;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Get Job DLL Paths from Configuration
var jobAssemblies = builder.Configuration.GetSection("Quartz:JobAssemblies").Get<string[]>() ?? new string[0];

#region Configure Quartz3
// base configuration from appsettings.json
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));

// if you are using persistent job store, you might want to alter some options
builder.Services.Configure<QuartzOptions>(options =>
{
    var jobStoreType = options["quartz.jobStore.type"];
    if ((jobStoreType ?? string.Empty) == "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz")
    {
        options.Scheduling.IgnoreDuplicates = true; // default: false
        options.Scheduling.OverWriteExistingData = true; // default: true
    }

    var dataSource = options["quartz.jobStore.dataSource"];
    if (!string.IsNullOrEmpty(dataSource))
    {
        var connectionStringName = options[$"quartz.dataSource.{dataSource}.connectionStringName"];
        if (!string.IsNullOrEmpty(connectionStringName))
        {
            var connStr = builder.Configuration.GetConnectionString(connectionStringName);
            options[$"quartz.dataSource.{dataSource}.connectionString"] = connStr;
        }
    }
});
 

// Add the required Quartz.NET services 
builder.Services.AddQuartz();
// Add the Quartz.NET hosted service
builder.Services.AddQuartzHostedService(
    q => q.WaitForJobsToComplete = true);
#endregion Configure Quartz3

// Add services to the container.
builder.Services.AddSingleton<LogService>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazingQuartzUI(builder.Configuration.GetSection("BlazingQuartz"),
    connectionString: builder.Configuration.GetConnectionString("BlazingQuartzDb"));

// Cấu hình Authentication
builder.Services.AddAuthenticationCore();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ProtectedSessionStorage>();

//builder.Services.AddControllersWithViews()
//    .AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UsePathBase("/BlazingQuartzUI");
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseBlazingQuartzUI();
app.MapBlazorHub();
app.MapFallbackToPage("/BlazingQuartzUI/_Host");

app.Run();

