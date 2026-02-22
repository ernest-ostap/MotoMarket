using MotoMarket.Application;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Infrastructure;
using MotoMarket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

//  Service registration 
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR (chat)
builder.Services.AddSignalR();

//  CORS 
builder.Services.AddCors(options =>
{
    // DevelopmentCors: Web, Mobile, SignalR (local dev)
    options.AddPolicy("DevelopmentCors", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "https://localhost:7029",  // Web (HTTPS)
                "http://localhost:7029",   // Web (HTTP)
                "http://10.0.2.2:5180",    // Android Emulator
                "http://localhost:5180",   // API Localhost
                "http://127.0.0.1:5180",   // API IP
                "http://0.0.0.0:5180"      // API nasłuchujące na wszystkim
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    // Optional: ProductionCors policy
    /*options.AddPolicy("ProductionCors", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("https://twoja-domena.pl")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });*/
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, MotoMarket.Api.Services.CurrentUserService>();

var app = builder.Build();

// Data seeder 
// Seeder zostawiam dołączony do projektu w celu ułatwienia sprawdzania, w produkcji można go wyłączyć
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbContextSeeder>();
    await seeder.SeedAsync();
}

//  Pipeline (middleware) 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Skip HTTPS redirect in development (for Android emulator)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

//  CORS 
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}
else
{
    app.UseCors("ProductionCors");
}

app.UseAuthentication();
app.UseAuthorization();

// Endpoint mapping
app.MapHub<MotoMarket.Api.Hubs.ChatHub>("/chatHub"); // SignalR Hub
app.MapControllers();

app.Run();