using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.Web.Services;

public interface IAuthService
{
    Task<TokenClaimsDTO> LoginAsync(LoginDTO loginDTO);
}

public class AuthService(HttpClient httpClient) : IAuthService
{
    private readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public async Task<TokenClaimsDTO> LoginAsync(LoginDTO loginDTO)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth", loginDTO);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<TokenClaimsDTO>(result, _options)!;
        }
        else
        {
            return null!;
        }
    }

}