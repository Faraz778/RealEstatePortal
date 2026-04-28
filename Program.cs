using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Data;

var builder = WebApplication.CreateBuilder(args);

//
// ✅ 1. MVC Services
//
builder.Services.AddControllersWithViews();

//
// ✅ 2. Database Context (EF Core + SQL Server)
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//
// ✅ 3. Session Support (for login / role management)
//
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//
// ✅ 4. Exception Handling
//
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//
// ✅ 5. Middleware Pipeline
//
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//
// ✅ IMPORTANT: Session must be before Authorization
//
app.UseSession();

app.UseAuthorization();

//
// ✅ 6. Default Route
//
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();