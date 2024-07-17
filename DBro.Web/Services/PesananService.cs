using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.Web.Services;

public interface IPesananService
{
    public string IdEditor { get; set; } // Id Pesanan yang memanipulasi data

    #region Pesanan

    Task<(List<Pesanan>, string)> GetAsync(List<string> includes = null!);

    Task<(PesananFormDTO, string)> GetFormAsync(string id, List<string> includes = null!);

    Task<(Pesanan, string)> FindAsync(string id, List<string> includes = null!);

    Task<(Pesanan, string)> AddAsync(Pesanan pesanan);

    Task<(bool, string)> UpdateAsync(Pesanan pesanan);

    Task<(bool, string)> DeleteAsync(string id);

    #endregion Pesanan

    #region Keranjang

    /// <summary> Ambil pesanan terakhir Include Detail then Menu </summary>
    Task<(Pesanan, string)> CekKeranjangAsync(string email);

    Task<(DetailPesanan, string)> TambahKeKeranjangAsync(DetailPesanan detailPesanan);

    Task<(bool, string)> UpdateDetailAsync(DetailPesanan detailPesanan);

    Task<(bool, string)> DeleteDetailAsync(string idPesanan, string idMenu);

    #endregion Keranjang
}

public class PesananService : IPesananService
{
    public string IdEditor { get; set; } = string.Empty;

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
        _httpClient.DefaultRequestHeaders.Add("Id-Editor", IdEditor);
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
        _httpClient.DefaultRequestHeaders.Add("Id-Editor", IdEditor);
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
        _httpClient.DefaultRequestHeaders.Add("Id-Editor", IdEditor);
        var response = await _httpClient.DeleteAsync($"api/pesanan/{id}");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }

    #endregion Pesanan

    #region Keranjang

    public async Task<(Pesanan, string)> CekKeranjangAsync(string email)
    {
        var response = await _httpClient.GetAsync($"api/pesanan/keranjang/{email}");
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            return (JsonSerializer.Deserialize<Pesanan>(result)!, null!);
        return (null!, result);
    }

    public async Task<(DetailPesanan, string)> TambahKeKeranjangAsync(DetailPesanan detailPesanan)
    {
        var response = await _httpClient.PostAsJsonAsync("api/pesanan/keranjang", JsonSerializer.Serialize(detailPesanan), _options);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return (JsonSerializer.Deserialize<DetailPesanan>(result, _options)!, null!);
        }
        else
        {
            return (null!, result);
        }
    }

    public async Task<(bool, string)> UpdateDetailAsync(DetailPesanan detailPesanan)
    {
        var response = await _httpClient.PutAsJsonAsync("api/pesanan/keranjang", JsonSerializer.Serialize(detailPesanan), _options);
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

    public async Task<(bool, string)> DeleteDetailAsync(string idPesanan, string idMenu)
    {
        var response = await _httpClient.DeleteAsync($"api/pesanan/keranjang/{idPesanan}/{idMenu}");
        string result = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, result);
    }

    #endregion Keranjang
}