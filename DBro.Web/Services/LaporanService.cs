using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.JSInterop;

namespace DBro.Web.Services;

public interface ILaporanService
{
    Task<bool> GetMasterAsync(string laporan);

    Task<bool> GetTransaksiPesananAsync(string dari, string sampai);

    Task<bool> GetDetailPesananAsync(string idPesanan);

    Task<bool> GetGrafikAsync(string tahun);
}

public class LaporanService : ILaporanService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public LaporanService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IJSRuntime js)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext!.Request.Cookies["api_token"]);
        _js = js;
    }

    public async Task<bool> GetMasterAsync(string laporan)
    {
        var response = await _httpClient.GetAsync($"api/laporan/{laporan}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            var fileStream = new MemoryStream(content);
            var fileName = response.Content.Headers.ContentDisposition!.FileName;
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GetTransaksiPesananAsync(string dari, string sampai)
    {
        var response = await _httpClient.GetAsync($"api/laporan/transaksi-pesanan?dari={dari}&sampai={sampai}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            var fileStream = new MemoryStream(content);
            var fileName = response.Content.Headers.ContentDisposition!.FileName;
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GetDetailPesananAsync(string idPesanan)
    {
        var response = await _httpClient.GetAsync($"api/laporan/detail-pesanan?idPesanan={idPesanan}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            var fileStream = new MemoryStream(content);
            var fileName = response.Content.Headers.ContentDisposition!.FileName;
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GetGrafikAsync(string tahun)
    {
        var response = await _httpClient.GetAsync($"api/laporan/grafik?tahun={tahun}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            var fileStream = new MemoryStream(content);
            var fileName = response.Content.Headers.ContentDisposition!.FileName;
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        return response.IsSuccessStatusCode;
    }
}