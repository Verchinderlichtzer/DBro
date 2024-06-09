using System.Text.Json.Serialization;
using System.Text.Json;

namespace DBro.Web.Services;

public interface IAktivitasService
{
    Task<(List<Aktivitas>, string)> GetAsync(List<string> includes = null!);

    Task<(bool, string)> DeletesAsync();
}

public class AktivitasService : IAktivitasService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public AktivitasService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext!.Request.Cookies["api_token"]);
    }

    public async Task<(List<Aktivitas>, string)> GetAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/aktivitas?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<List<Aktivitas>>(result)!, null!);
        return (null!, result);
    }

    public async Task<(bool, string)> DeletesAsync()
    {
        var response = await _httpClient.DeleteAsync("api/aktivitas");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }
}