using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor runtime compilation
builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Token handler
builder.Services.AddTransient<TokenHandler>();

// LoginController için named HttpClient
builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Api:BaseUrl"]!);
})
.AddHttpMessageHandler<TokenHandler>(); // her isteğe Bearer token ekle

// Typed ApiClient (tüm diğer çağrılar için)
builder.Services.AddHttpClient<ApiClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Api:BaseUrl"]!);
})
.AddHttpMessageHandler<TokenHandler>(); // her isteğe Bearer token ekle

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session mutlaka burada
app.UseSession();

// GİRİŞ KONTROLÜ – login harici tüm sayfalar için zorunlu
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

    // Login sayfası ve statik dosyalar serbest
    if (path.StartsWith("/login") ||
        path.StartsWith("/css") ||
        path.StartsWith("/js") ||
        path.StartsWith("/lib") ||
        path.StartsWith("/images"))
    {
        await next();
        return;
    }

    // Session'da token yoksa Login'e yönlendir
    var token = context.Session.GetString("AccessToken");
    if (string.IsNullOrEmpty(token))
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});

// Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
