using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class ApiClient
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient client,
                     IConfiguration configuration,
                     IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;

        var baseUrl = configuration["Api:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
            _client.BaseAddress = new Uri(baseUrl);
    }

    private void AddAuthHeader()
    {
        var ctx = _httpContextAccessor.HttpContext;
        var token = ctx?.Session.GetString("AccessToken");   // ⬅️ Login’de yazdığımız

        if (!string.IsNullOrEmpty(token))
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        else
            _client.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<TResponse?> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string url,
        TRequest? body = default,
        CancellationToken cancellationToken = default)
    {
        AddAuthHeader(); // HER İSTEK ÖNCESİ

        using var request = new HttpRequestMessage(method, url);

        if (body is not null && method != HttpMethod.Get && method != HttpMethod.Delete)
            request.Content = JsonContent.Create(body);

        using var response = await _client.SendAsync(request, cancellationToken);

        var raw = response.Content == null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"API Error {(int)response.StatusCode} ({response.ReasonPhrase}): {raw}");

        if (string.IsNullOrWhiteSpace(raw))
            return default;

        return JsonSerializer.Deserialize<TResponse>(raw, JsonOptions);
    }

    public Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken ct = default)
        => SendAsync<object?, TResponse>(HttpMethod.Get, url, null, ct);

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
        => SendAsync<TRequest, TResponse>(HttpMethod.Post, url, body, ct);

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
        => SendAsync<TRequest, TResponse>(HttpMethod.Put, url, body, ct);

    public Task<TResponse?> DeleteAsync<TResponse>(string url, CancellationToken ct = default)
        => SendAsync<object?, TResponse>(HttpMethod.Delete, url, null, ct);
}
