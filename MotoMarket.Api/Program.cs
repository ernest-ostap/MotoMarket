using MotoMarket.Application;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Infrastructure;
using MotoMarket.Infrastructure.Persistence;
// Usingi do JWT nie s� tu ju� potrzebne, bo s� w Infrastructure

var builder = WebApplication.CreateBuilder(args);

// Rejestracja warstw (Tutaj w �rodku dzieje si� AddJwtBearer z fixem)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(); // SignalR zostaje

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", builder =>
    {
        builder
            .WithOrigins("https://localhost:7029")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    
    // CORS dla aplikacji MAUI (Android emulator i inne)
    options.AddPolicy("AllowMobileClient", builder =>
    {
        builder
            .WithOrigins("http://10.0.2.2:5180", "http://localhost:5180", "http://127.0.0.1:5180")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    
    // W development - pozwól na wszystko (dla łatwiejszego debugowania)
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, MotoMarket.Api.Services.CurrentUserService>();

var app = builder.Build();

// Seeder
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbContextSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// W development nie wymuszaj HTTPS (żeby MAUI mogło łączyć się przez HTTP)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

// Użyj odpowiedniej polityki CORS
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll"); // W development pozwól na wszystko
}
else
{
    app.UseCors("AllowWebClient"); // W produkcji tylko Web
}

app.UseAuthentication(); // Auth
app.UseAuthorization();  // Authz

app.MapHub<MotoMarket.Api.Hubs.ChatHub>("/chatHub"); // Mapowanie Huba
app.MapControllers();

app.Run();