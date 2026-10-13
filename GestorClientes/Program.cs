using GestorClientes.Application.Mapping;
using GestorClientes.Application.Services;
using GestorClientes.Infrastructure.Data;
using GestorClientes.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Base de datos SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias: repositorios y servicios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteServicio, ClienteServicio>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MapeoClases));

var app = builder.Build();

// Crea la base de datos y las tablas si no existen
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
