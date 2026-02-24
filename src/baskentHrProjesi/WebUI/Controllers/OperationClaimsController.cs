using Application.Features.OperationClaims.Commands.Create;
using Application.Features.OperationClaims.Commands.Update;
using Application.Features.OperationClaims.Commands.Delete;
using Application.Features.OperationClaims.Queries.GetById;
using Application.Features.OperationClaims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class OperationClaimsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OperationClaimsController(IHttpClientFactory httpClientFactory)
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
            await client.GetFromJsonAsync<GetListResponse<GetListOperationClaimListItemDto>>("/api/OperationClaims");

        var items = apiResponse?.Items ?? new List<GetListOperationClaimListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var op = await client
            .GetFromJsonAsync<GetByIdOperationClaimResponse>($"/api/OperationClaims/{id}");

        if (op == null) return NotFound();

        return Json(op);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOperationClaimCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Json(new { success = false, message = "Rol adı zorunlu." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/OperationClaims", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Rol eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] UpdateOperationClaimCommand command)
    {
        if (command.Id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        if (string.IsNullOrWhiteSpace(command.Name))
            return Json(new { success = false, message = "Rol adı zorunlu." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/OperationClaims", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Rol güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var deleteCommand = new DeleteOperationClaimCommand { Id = id };

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/OperationClaims")
        {
            Content = JsonContent.Create(deleteCommand)
        };

        var resp = await client.SendAsync(request);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Rol silinemedi." });

        return Json(new { success = true });
    }
}
