using MotoMarket.Application;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Infrastructure;
using MotoMarket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// --- REJESTRACJA SERWISÓW ---

// Warstwy aplikacji i infrastruktury (tu jest też JWT Configuration)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR (wymagane dla czatu)
builder.Services.AddSignalR();

// --- NAPRAWA CORS ---
builder.Services.AddCors(options =>
{
    // Polityka "DevelopmentCors" - obsługuje wszystko co potrzebujesz lokalnie.
    // Łączy w sobie Web, Mobile i SignalR.
    options.AddPolicy("DevelopmentCors", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "https://localhost:7029",  // Twój Web (HTTPS)
                "http://localhost:7029",   // Twój Web (HTTP - na wszelki wypadek)
                "http://10.0.2.2:5180",    // Android Emulator
                "http://localhost:5180",   // API Localhost
                "http://127.0.0.1:5180",   // API IP
                "http://0.0.0.0:5180"      // API nasłuchujące na wszystkim
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // <--- TO NAPRAWIA BŁĄD W KONSOLI (signalr.min.js)
    });

    // Opcjonalnie: Polityka na Produkcję (gdybyś publikował)
    options.AddPolicy("ProductionCors", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("https://twoja-domena.pl")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, MotoMarket.Api.Services.CurrentUserService>();

var app = builder.Build();

// --- SEEDER DANYCH ---
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbContextSeeder>();
    await seeder.SeedAsync();
}

// --- PIPELINE (MIDDLEWARE) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// W development nie wymuszaj HTTPS (ułatwia życie emulatorowi Androida)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// --- WŁĄCZENIE CORS (Z naszą nową polityką) ---
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors"); // <--- Używamy tej naprawionej polityki
}
else
{
    app.UseCors("ProductionCors");
}

app.UseAuthentication(); // Logowanie (musi być przed Authorization)
app.UseAuthorization();  // Uprawnienia

// Mapowanie Endpointów
app.MapHub<MotoMarket.Api.Hubs.ChatHub>("/chatHub"); // SignalR Hub
app.MapControllers();

app.Run();