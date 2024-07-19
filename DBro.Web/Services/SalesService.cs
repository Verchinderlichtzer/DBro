using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.Web.Services;

public interface ISalesService
{
    #region Sales

    Task<(SalesDTO, string)> GetSalesAsync(List<string> includes = null!);

    Task<(SalesDTO, string)> GetFormAsync(List<string> diskonIds, List<string> promoIds, List<string> includes = null!);

    Task<(SalesDTO, string)> FindsSalesAsync(List<string> diskonIds, List<string> promoIds, List<string> includes = null!);

    Task<(SalesDTO, string)> AddsSalesAsync(SalesDTO diskon);

    Task<(bool, string)> UpdatesSalesAsync(SalesDTO diskon);

    Task<(bool, string)> DeletesSalesAsync(List<string> diskonIds, List<string> promoIds);

    #endregion Sales

    #region Diskon

    Task<(List<Diskon>, string)> GetDiskonAsync(List<string> includes = null!);

    Task<(Diskon, string)> FindDiskonAsync(string id, List<string> includes = null!);

    #endregion Diskon

    #region Promo

    Task<(List<Promo>, string)> GetPromoAsync(List<string> includes = null!);

    Task<(Promo, string)> FindPromoAsync(string id, List<string> includes = null!);

    #endregion Promo
}

public class SalesService : ISalesService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public SalesService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext!.Request.Cookies["api_token"]);
    }

    #region Sales

    public async Task<(SalesDTO, string)> GetSalesAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<SalesDTO>(result)!, null!);
        return (null!, result);
    }

    public async Task<(SalesDTO, string)> GetFormAsync(List<string> diskonIds, List<string> promoIds, List<string> includes = null!)
    {
        string diskonIdList = diskonIds.Count > 0 ? $"diskonIds={string.Join(',', diskonIds)}" : string.Empty;
        string promoIdList = promoIds.Count > 0 ? $"promoIds={string.Join(',', promoIds)}" : string.Empty;
        string join = includes != null ? $"entities={string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales/form?diskonIds={diskonIdList}&promoIds={promoIdList}&entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<SalesDTO>(result)!, null!);
        return (null!, result);
    }

    public async Task<(SalesDTO, string)> FindsSalesAsync(List<string> diskonIds, List<string> promoIds, List<string> includes = null!)
    {
        string diskonIdList = diskonIds.Count > 0 ? $"diskonIds={string.Join(',', diskonIds)}" : string.Empty;
        string promoIdList = promoIds.Count > 0 ? $"promoIds={string.Join(',', promoIds)}" : string.Empty;
        string join = includes != null ? $"entities={string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales/find?diskonIds={diskonIdList}&promoIds={promoIdList}&entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<SalesDTO>(result)!, null!);
        return (null!, result);
    }

    public async Task<(SalesDTO, string)> AddsSalesAsync(SalesDTO sales)
    {
        
        var response = await _httpClient.PostAsJsonAsync("api/sales", JsonSerializer.Serialize(sales), _options);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return (JsonSerializer.Deserialize<SalesDTO>(result, _options)!, null!);
        }
        else
        {
            return (null!, result);
        }
    }

    public async Task<(bool, string)> UpdatesSalesAsync(SalesDTO sales)
    {
        
        var response = await _httpClient.PutAsJsonAsync("api/sales", JsonSerializer.Serialize(sales), _options);
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

    public async Task<(bool, string)> DeletesSalesAsync(List<string> diskonIds, List<string> promoIds)
    {
        
        string diskonIdList = diskonIds.Count > 0 ? $"diskonIds={string.Join(',', diskonIds)}" : string.Empty;
        string promoIdList = promoIds.Count > 0 ? $"promoIds={string.Join(',', promoIds)}" : string.Empty;
        var response = await _httpClient.DeleteAsync($"api/sales?diskonIds={diskonIdList}&promoIds={promoIdList}");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }

    #endregion Sales

    #region Diskon

    public async Task<(List<Diskon>, string)> GetDiskonAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales/diskon?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<List<Diskon>>(result)!, null!);
        return (null!, result);
    }

    public async Task<(Diskon, string)> FindDiskonAsync(string id, List<string> includes = null!)
    {
        string join = includes != null ? $"entities={string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales/diskon/{id}?{join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<Diskon>(result)!, null!);
        return (null!, result);
    }

    #endregion Diskon

    #region Promo

    public async Task<(List<Promo>, string)> GetPromoAsync(List<string> includes = null!)
    {
        string join = includes != null ? $"{string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales/promo?entities={join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<List<Promo>>(result)!, null!);
        return (null!, result);
    }

    public async Task<(Promo, string)> FindPromoAsync(string id, List<string> includes = null!)
    {
        string join = includes != null ? $"entities={string.Join(',', includes)}" : string.Empty;
        var response = await _httpClient.GetAsync($"api/sales/promo/{id}?{join}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<Promo>(result)!, null!);
        return (null!, result);
    }

    #endregion Promo

}