using System.Text.Json.Serialization;
using System.Text.Json;

namespace DBro.Web.Services;

public interface IMenuService
{
    Task<(List<Menu>, string)> GetAsync(List<string> includes = null!);

    Task<(Menu, string)> FindAsync(string id, List<string> includes = null!);

    Task<(Menu, string)> AddAsync(Menu menu);

    Task<(bool, string)> UpdateAsync(Menu menu);

    Task<(bool, string)> DeleteAsync(string id);
}

public class MenuService : IMenuService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public MenuService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext!.Request.Cookies["api_token"]);
    }

    public async Task<(List<Menu>, string)> GetAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/menu?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<List<Menu>>(result)!, null!);
        return (null!, result);
    }

    public async Task<(Menu, string)> FindAsync(string id, List<string> includes = null!)
    {
        string join = includes != null ? $"entities={string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/menu/{id}?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<Menu>(result)!, null!);
        return (null!, result);
    }

    public async Task<(Menu, string)> AddAsync(Menu menu)
    {
        
        var response = await _httpClient.PostAsJsonAsync("api/menu", JsonSerializer.Serialize(menu), _options);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return (JsonSerializer.Deserialize<Menu>(result, _options)!, null!);
        }
        else
        {
            return (null!, result);
        }
    }

    public async Task<(bool, string)> UpdateAsync(Menu menu)
    {
        
        var response = await _httpClient.PutAsJsonAsync("api/menu", JsonSerializer.Serialize(menu), _options);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return (true, null!);
        }
        else
        {
            return (false, result);
        }
    }

    public async Task<(bool, string)> DeleteAsync(string id)
    {
        
        var response = await _httpClient.DeleteAsync($"api/menu/{id}");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }
}