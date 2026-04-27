using Microsoft.EntityFrameworkCore;
using EcoPlantas.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF Core with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session cache: Redis en producción, memoria en desarrollo
var redisConn = builder.Configuration["Redis__ConnectionString"]
             ?? builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrWhiteSpace(redisConn))
{
    // Normalizar URL estilo redis://host:port que entrega Render
    if (redisConn.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(redisConn);
        redisConn = $"{uri.Host}:{uri.Port}";
    }

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConn;
        options.InstanceName = "ecoplantas:";
    });
}
else if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    throw new InvalidOperationException(
        "Redis__ConnectionString es requerido en producción. " +
        "Configura la variable de entorno Redis__ConnectionString en Render.");
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    DataSeeder.Seed(db);
}

app.Run();
