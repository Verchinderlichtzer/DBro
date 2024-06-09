using Microsoft.AspNetCore.Mvc;
using DBro.API.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class PesananController(IPesananRepository pesananRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [HttpGet]
    public async Task<IActionResult> GetPesanan([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await pesananRepository.GetAsync(includes);
            if (result != null)
                return Ok(JsonSerializer.Serialize(result, _options));
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindPesanan(string id, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await pesananRepository.FindAsync(id, includes);
            if (result.Item2 == true)
                return Ok(JsonSerializer.Serialize(result.Item1, _options));
            else if (result.Item2 == false)
                return NotFound("Pesanan tidak ditemukan");
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostPesanan([FromBody] string jsonString)
    {
        try
        {
            Pesanan pesanan = JsonSerializer.Deserialize<Pesanan>(jsonString)!;
            var result = await pesananRepository.AddAsync(pesanan);
            pesanan = result.Item1;
            return pesanan != null ? CreatedAtAction(nameof(FindPesanan), new { id = pesanan.Id }, JsonSerializer.Serialize(pesanan, _options)) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutPesanan([FromBody] string jsonString)
    {
        try
        {
            Pesanan pesanan = JsonSerializer.Deserialize<Pesanan>(jsonString)!;
            var result = await pesananRepository.UpdateAsync(pesanan);
            return result.Item1 == true ? NoContent() : result.Item1 == false ? NotFound(result.Item2) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePesanan(string id)
    {
        try
        {
            var result = await pesananRepository.DeleteAsync(id);

            if (result == 0)
                return NoContent();
            else if (result == 1)
                return NotFound("Pesanan tidak ditemukan");
            return BadRequest("Ada kesalahan saat menghapus data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }
}