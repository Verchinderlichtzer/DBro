using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using static DBro.Shared.Extensions.EncryptionHelper;
using System;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class AuthController(IConfiguration config, IUserRepository userRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        User user = await Authenticate(loginDTO);

        if (user != null)
        {
            var token = GenerateToken(user);
            var claims = token.Item2.Select(c => new { c.Type, c.Value });
            return Ok(JsonSerializer.Serialize(new TokenClaimsDTO { Token = token.Item1, Claims = token.Item2.Select(x => new ClaimDTO() { Type = x.Type, Value = x.Value }) }, _options));
        }
        return NotFound("User tidak ditemukan");
    }

    private (string, IEnumerable<Claim>) GenerateToken(User user)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Email),
            new(ClaimTypes.Name, user.Nama),
            new(ClaimTypes.Role, user.JenisUser.ToString()),
            new(ClaimTypes.Gender, user.JenisKelamin.ToString()),
            new(ClaimTypes.Email, user.Email)
        ];

        JwtSecurityToken token = new(config["Jwt:Issuer"], config["Jwt:Audience"], claims, expires: DateTimeOffset.UtcNow.AddHours(3).DateTime, signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), claims);
    }

    private async Task<User> Authenticate(LoginDTO loginDTO)
    {
        User user = (await userRepository.FindAsync(loginDTO.Email)).Item1;
        if (user != null)
        {
            string plainPassword = Decrypt(user.Password);
            if (loginDTO.Password == plainPassword) return user;
        }

        return null!;
    }
}
