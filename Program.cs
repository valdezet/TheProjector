using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using TheProjector.Constants;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Auth/Forbidden";
        options.LoginPath = "/Auth/Login";
    });
builder.Services.AddSingleton<IAuthorizationHandler, TheProjector.Policies.Handlers.HasOneOfRolesHandler>();

builder.Services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy.Requirements.Add(
    new TheProjector.Policies.Requirements.HasOneOfRolesRequirement(new string[] { RoleConstants.Admin, RoleConstants.SuperAdmin })
)));

builder
    .Services
    .AddDbContext<TheProjector.Data.Persistence.TheProjectorDbContext>(
        options => options
            .UseSqlServer(
                builder
                .Configuration
                .GetConnectionString("TheProjectorSqlServer")
            )
    );

builder.Services.AddScoped<TheProjector.Services.ProjectService>();
builder.Services.AddScoped<TheProjector.Services.PersonService>();
builder.Services.AddScoped<TheProjector.Services.ProjectAssignmentService>();
builder.Services.AddScoped<TheProjector.Services.AuthService>();
builder.Services.AddScoped<TheProjector.Services.PasswordService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "SingleEntity",
    pattern: "{controller=Home}/{id:long}/{action=View}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");



app.Run();
