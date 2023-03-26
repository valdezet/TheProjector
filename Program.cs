using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "SingleEntity",
    pattern: "{controller=Home}/{id:long}/{action=View}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");



app.Run();
