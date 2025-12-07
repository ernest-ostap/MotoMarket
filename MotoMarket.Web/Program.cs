using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor(); // Rejestrujemy dostêp do HttpContext
builder.Services.AddScoped<MotoMarket.Web.Services.IAuthService, MotoMarket.Web.Services.AuthService>(); // Rejestrujemy serwis uwierzytelniania
//konfiguracja uwierzytelniania za pomoc¹ ciasteczek
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Gdzie przekierowaæ niezalogowanego?
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddHttpClient(); // Rejestrujemy fabrykê klienta HTTP
builder.Services.AddScoped<MotoMarket.Web.Services.IVehicleService, MotoMarket.Web.Services.VehicleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Dodajemy middleware uwierzytelniania, musi byc przed UseAuthorization 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
