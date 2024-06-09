using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [HttpGet]
    public async Task<IActionResult> GetUser([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await userRepository.GetAsync(includes);
            if (result != null)
                return Ok(JsonSerializer.Serialize(result, _options));
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> FindUser(string email, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await userRepository.FindAsync(email, includes);
            if (result.Item2 == true)
                return Ok(JsonSerializer.Serialize(result.Item1, _options));
            else if (result.Item2 == false)
                return NotFound("User tidak ditemukan");
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostUser([FromBody] string jsonString)
    {
        try
        {
            User user = JsonSerializer.Deserialize<User>(jsonString)!;
            var result = await userRepository.AddAsync(Request.Headers["Id-Editor"]!, user);
            user = result.Item1;
            return user != null ? CreatedAtAction(nameof(FindUser), new { email = user.Email }, JsonSerializer.Serialize(user, _options)) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutUser([FromBody] string jsonString)
    {
        try
        {
            User user = JsonSerializer.Deserialize<User>(jsonString)!;
            var result = await userRepository.UpdateAsync(Request.Headers["Id-Editor"]!, user);
            return result.Item1 == true ? NoContent() : result.Item1 == false ? NotFound(result.Item2) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        try
        {
            var result = await userRepository.DeleteAsync(Request.Headers["Id-Editor"]!, email);

            if (result == 0)
                return NoContent();
            else if (result == 1)
                return NotFound("User tidak ditemukan");
            else if (result == 2)
                return Conflict("User pernah bertransaksi");
            else if (result == 3)
                return Conflict("User pernah bertugas");
            return BadRequest("Ada kesalahan saat menghapus data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }
}