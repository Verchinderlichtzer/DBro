using Microsoft.AspNetCore.Mvc;
using DBro.API.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class PesananController(IPesananRepository pesananRepository, IMenuRepository menuRepository, ISalesRepository salesRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
    #region Pesanan

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

    [HttpGet("form/{id?}")]
    public async Task<IActionResult> GetPesananForm(string? id, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            PesananFormDTO dto = new()
            {
                Menu = (await menuRepository.GetAsync()).ConvertAll(x => new Menu() { Id = x.Id, Nama = x.Nama, Harga = x.Harga, Kategori = x.Kategori }),
                //VarianMenu = await menuRepository.GetVarianAsync(),
                Diskon = await salesRepository.GetDiskonAsync(),
                Promo = await salesRepository.GetPromoAsync()
            };
            var result = await pesananRepository.FindAsync(id!, includes);
            dto.Pesanan = result.Item1;
            return Ok(JsonSerializer.Serialize(dto, _options));
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
                return NotFound("Pesanan belum dibuat");
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

    [HttpGet("detail/{idPesanan}/{idMenu}")]
    public async Task<IActionResult> FindDetail(string idPesanan, string idMenu)
    {
        try
        {
            var result = await pesananRepository.FindDetailAsync(idPesanan, idMenu);
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

    #endregion Pesanan
}