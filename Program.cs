using Microsoft.EntityFrameworkCore;
using EcoPlantas.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// EF Core con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger — disponible en todos los entornos para demostracion academica
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EcoPlantas API",
        Version = "v1",
        Description = "API REST para el sistema EcoPlantas - Cieneguilla"
    });
});

// Session cache: Redis en produccion, memoria en desarrollo
var redisConn = builder.Configuration["Redis__ConnectionString"]
             ?? builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrWhiteSpace(redisConn))
{
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swagger UI — accesible en /swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoPlantas API v1");
    c.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    DataSeeder.Seed(db);
}

app.Run();
