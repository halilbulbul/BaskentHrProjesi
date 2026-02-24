using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Dtos;
using System.Net.Http.Json;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View(new UserForLoginDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserForLoginDto model)
    {
        if (!ModelState.IsValid)
        {
            var firstError = ModelState.Values
                .SelectMany(v => v.Errors)
                .FirstOrDefault()?.ErrorMessage;

            return Json(new
            {
                success = false,
                message = firstError ?? "Lütfen e-posta ve şifre alanlarını kontrol edin."
            });
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var body = new
        {
            email = model.Email,
            password = model.Password,
            authenticatorCode = model.AuthenticatorCode
        };

        var response = await client.PostAsJsonAsync("/api/Auth/Login", body);

        if (!response.IsSuccessStatusCode)
        {
            return Json(new
            {
                success = false,
                message = "E-posta veya şifre hatalı."
            });
        }

        var result = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();

        if (result == null || result.AccessToken == null || string.IsNullOrEmpty(result.AccessToken.Token))
        {
            return Json(new
            {
                success = false,
                message = "Token alınamadı."
            });
        }

        HttpContext.Session.SetString("AccessToken", result.AccessToken.Token);

        return Json(new
        {
            success = true,
            redirectUrl = Url.Action("Index", "Home")
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Login");
    }
}
