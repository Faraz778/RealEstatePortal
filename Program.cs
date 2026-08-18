using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Data;
//using RealEstatePortal.Services;

var builder = WebApplication.CreateBuilder(args);

//
//  MVC Services
//
builder.Services.AddControllersWithViews();

//
//  Database Context (EF Core + SQL Server)
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//
//  Session Support (for login / role management)
//
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Expiry Service
//builder.Services.AddScoped<ListingExpiryService>();
var app = builder.Build();

//
//  Exception Handling
//
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//
//  Middleware Pipeline
//
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//
//  IMPORTANT: Session must be before Authorization
//
app.UseSession();

app.UseAuthorization();

//
//  Default Route
//
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();