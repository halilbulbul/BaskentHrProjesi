using Application.Features.UserOperationClaims.Commands.Create;
using Application.Features.UserOperationClaims.Commands.Update;
using Application.Features.UserOperationClaims.Commands.Delete;
using Application.Features.UserOperationClaims.Queries.GetById;
using Application.Features.UserOperationClaims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class UserOperationClaimsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserOperationClaimsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var apiResponse =
            await client.GetFromJsonAsync<GetListResponse<GetListUserOperationClaimListItemDto>>(
                "/api/UserOperationClaims");

        var items = apiResponse?.Items ?? new List<GetListUserOperationClaimListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var result =
            await client.GetFromJsonAsync<GetByIdUserOperationClaimResponse>(
                $"/api/UserOperationClaims/{id}");

        if (result == null) return NotFound();

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserOperationClaimCommand command)
    {
        if (command == null ||
            command.UserId == Guid.Empty ||
            command.OperationClaimId <= 0)
        {
            return Json(new { success = false, message = "Geçersiz parametreler." });
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/UserOperationClaims", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Kayıt eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateUserOperationClaimCommand command)
    {
        if (command == null || command.UserId == Guid.Empty)
            return Json(new { success = false, message = "Geçersiz parametreler." });

        if (command.Id == Guid.Empty)
            command.Id = Guid.NewGuid();

        if ((command.OperationClaimIds == null || command.OperationClaimIds.Length == 0) && command.OperationClaimId > 0)
            command.OperationClaimIds = new[] { command.OperationClaimId };

        command.OperationClaimIds ??= Array.Empty<int>();

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/UserOperationClaims", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Kayıt güncellenemedi." });

        return Json(new { success = true });
    }


    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/UserOperationClaims/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Kayıt silinemedi." });

        return Json(new { success = true });
    }
}
