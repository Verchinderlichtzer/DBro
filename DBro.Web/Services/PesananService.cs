using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.Web.Services;

public interface IPesananService
{
    #region Pesanan

    Task<(List<Pesanan>, string)> GetAsync(List<string> includes = null!);

    Task<(PesananFormDTO, string)> GetFormAsync(string id, List<string> includes = null!);

    Task<(Pesanan, string)> FindAsync(string id, List<string> includes = null!);

    Task<(Pesanan, string)> AddAsync(Pesanan pesanan);

    Task<(bool, string)> UpdateAsync(Pesanan pesanan);

    Task<(bool, string)> DeleteAsync(string id);

    #endregion Pesanan
}

public class PesananService : IPesananService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public PesananService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext!.Request.Cookies["api_token"]);
    }

    #region Pesanan

    public async Task<(List<Pesanan>, string)> GetAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/pesanan?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<List<Pesanan>>(result)!, null!);
        return (null!, result);
    }

    public async Task<(PesananFormDTO, string)> GetFormAsync(string id, List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/pesanan/form/{id}?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<PesananFormDTO>(result)!, null!);
        return (null!, result);
    }

    public async Task<(Pesanan, string)> FindAsync(string id, List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/pesanan/{id}?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<Pesanan>(result)!, null!);
        return (null!, result);
    }

    public async Task<(Pesanan, string)> AddAsync(Pesanan pesanan)
    {
        
        var response = await _httpClient.PostAsJsonAsync("api/pesanan", JsonSerializer.Serialize(pesanan), _options);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return (JsonSerializer.Deserialize<Pesanan>(result, _options)!, null!);
        }
        else
        {
            return (null!, result);
        }
    }

    public async Task<(bool, string)> UpdateAsync(Pesanan pesanan)
    {
        
        var response = await _httpClient.PutAsJsonAsync("api/pesanan", JsonSerializer.Serialize(pesanan), _options);
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
        
        var response = await _httpClient.DeleteAsync($"api/pesanan/{id}");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }

    #endregion Pesanan
}