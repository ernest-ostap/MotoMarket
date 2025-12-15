using MotoMarket.Application;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Infrastructure;
using MotoMarket.Infrastructure.Persistence;
// Usingi do JWT nie s¹ tu ju¿ potrzebne, bo s¹ w Infrastructure

var builder = WebApplication.CreateBuilder(args);

// Rejestracja warstw (Tutaj w œrodku dzieje siê AddJwtBearer z fixem)
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowWebClient"); // CORS

app.UseAuthentication(); // Auth
app.UseAuthorization();  // Authz

app.MapHub<MotoMarket.Api.Hubs.ChatHub>("/chatHub"); // Mapowanie Huba
app.MapControllers();

app.Run();