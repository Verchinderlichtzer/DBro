using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.Web.Services;

public interface IUserService
{
    Task<(List<User>, string)> GetAsync(List<string> includes = null!);

    Task<(User, string)> FindAsync(string email, List<string> includes = null!);

    Task<(User, string)> AddAsync(User user);

    Task<(bool, string)> UpdateAsync(User user);

    Task<(bool, string)> DeleteAsync(string email);
}

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext!.Request.Cookies["api_token"]);
    }

    public async Task<(List<User>, string)> GetAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/user?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<List<User>>(result)!, null!);
        return (null!, result);
    }

    public async Task<(User, string)> FindAsync(string email, List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/user/{email}?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<User>(result)!, null!);
        return (null!, result);
    }

    public async Task<(User, string)> AddAsync(User user)
    {
        
        var response = await _httpClient.PostAsJsonAsync("api/user", JsonSerializer.Serialize(user), _options);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return (JsonSerializer.Deserialize<User>(result, _options)!, null!);
        }
        else
        {
            return (null!, result);
        }
    }

    public async Task<(bool, string)> UpdateAsync(User user)
    {
        
        var response = await _httpClient.PutAsJsonAsync("api/user", JsonSerializer.Serialize(user), _options);
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

    public async Task<(bool, string)> DeleteAsync(string email)
    {
        
        var response = await _httpClient.DeleteAsync($"api/user/{email}");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }
}