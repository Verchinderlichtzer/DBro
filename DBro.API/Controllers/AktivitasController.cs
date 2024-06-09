using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class AktivitasController(IAktivitasRepository aktivitasRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [HttpGet]
    public async Task<IActionResult> GetAktivitas([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await aktivitasRepository.GetAsync(includes);
            return result != null ? Ok(JsonSerializer.Serialize(result, _options)) : BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAktivitas()
    {
        try
        {
            var result = await aktivitasRepository.DeleteAsync();

            if (result == true)
                return NoContent();
            else if (result == false)
                return NotFound("Tidak ada aktivitas");
            return BadRequest("Ada kesalahan saat menghapus data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }
}
