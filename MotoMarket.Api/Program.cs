using MotoMarket.Application;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Infrastructure;
using MotoMarket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor(); // Rejestrujemy dostêp do HttpContext
builder.Services.AddScoped<ICurrentUserService, MotoMarket.Api.Services.CurrentUserService>(); // Rejestracja serwisu aktualnego u¿ytkownika

var app = builder.Build();

// tymczasowy scope do wykonania seeda
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbContextSeeder>();
    await seeder.SeedAsync(); // To wykona migracje i doda dane
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication(); // 1. SprawdŸ kim jestem (czy mam token)
app.UseAuthorization();  // 2. SprawdŸ czy mam dostêp

app.UseAuthorization();

app.MapControllers();

app.Run();
