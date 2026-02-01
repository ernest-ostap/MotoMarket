using Microsoft.AspNetCore.Authentication.Cookies;
using MotoMarket.Web.Services.Admin;
using MotoMarket.Web.Services.Auth;
using MotoMarket.Web.Services.Chat;
using MotoMarket.Web.Services.Listings;
using MotoMarket.Web.Services.PdfGenerator;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor(); // Rejestrujemy dostêp do HttpContext
builder.Services.AddScoped<IAuthService, AuthService>(); // Rejestrujemy serwis uwierzytelniania
//konfiguracja uwierzytelniania za pomoc¹ ciasteczek
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Gdzie przekierowaæ niezalogowanego?

        options.AccessDeniedPath = "/Auth/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddHttpClient(); // Rejestrujemy fabrykê klienta HTTP
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddHttpClient<IAdminService, AdminService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days, for production scenarios change it as needed.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/NotFound");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Dodajemy middleware uwierzytelniania, musi byc przed UseAuthorization 
app.UseAuthorization();

app.UseMiddleware<MotoMarket.Web.Middleware.UserStatusMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
