using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROJBP.UI.Data;
using PROJBP.UI.Modules;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

// Add EF Module to DI
builder.Services.IncludeEFModule(builder.Configuration);

//Register Service Modules to DI
builder.Services.IncludeServiceModule(builder.Configuration);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();


//Add Controllers with views;
builder.Services.AddControllersWithViews();


//Implement Quartz Scheduler
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();

    // Register the job, loading the schedule from configuration
    //ReadAndPushBillNoJob perform periodic pushing of Jobs to Server every 1 minute;
    q.AddJobAndTrigger<ReadAndPushBillNoJob>(builder.Configuration);
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
