using Microsoft.EntityFrameworkCore;
using azureTest.Data;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
ConfigureServices(builder.Services, builder.Configuration);

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    var dbPassword = "";

    dbPassword = configuration["DATABASE_PASSWORD"];

    dbPassword = configuration["DATABASE_PASSWORD"];
    connectionString += $"Password={dbPassword};";

    // Add services to the container.
    services.AddControllersWithViews();

    services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(configuration.GetConnectionString("GcpConnection"),
    ServerVersion.AutoDetect(configuration.GetConnectionString("GcpConnection"))));
}

var app = builder.Build();

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

app.Run();

