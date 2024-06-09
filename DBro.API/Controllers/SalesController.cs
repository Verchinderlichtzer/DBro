using DBro.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.API.Controllers;

// Sales merupakan gabungan antara Diskon dan Promo

[ApiController, Route("api/[controller]")]
public class SalesController(ISalesRepository salesRepository, IMenuRepository menuRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [HttpGet("form")]
    public async Task<IActionResult> SalesForm([FromQuery] string diskonIds = null!, [FromQuery] string promoIds = null!, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> markedDiskon = diskonIds?.Mid(diskonIds.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            List<string> markedPromo = promoIds?.Mid(promoIds.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            SalesFormDTO dto = new()
            {
                Menu = (await menuRepository.GetAsync()).ConvertAll(x => new Menu() { Id = x.Id, Nama = x.Nama, Harga = x.Harga })
            };
            var result = await salesRepository.FindsSalesAsync(markedDiskon, markedPromo, includes);
            if (result.Item2 == true)
                dto.Sales = result.Item1;
            else if (result.Item2 == null)
                return BadRequest("Ada kesalahan saat mengakses data");
            return Ok(JsonSerializer.Serialize(dto, _options));
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    #region Sales

    [HttpGet]
    public async Task<IActionResult> GetSales([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.GetSalesAsync(includes);
            if (result != null)
                return Ok(JsonSerializer.Serialize(result, _options));
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindSales([FromQuery] string diskonIds = null!, [FromQuery] string promoIds = null!, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> markedDiskon = diskonIds?.Mid(diskonIds.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            List<string> markedPromo = promoIds?.Mid(promoIds.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.FindsSalesAsync(markedDiskon, markedPromo, includes);
            if (result.Item2 == true)
                return Ok(JsonSerializer.Serialize(result.Item1, _options));
            else if (result.Item2 == false)
                return NotFound("Data tidak ditemukan");
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostSales([FromBody] string jsonString)
    {
        try
        {
            SalesDTO sales = JsonSerializer.Deserialize<SalesDTO>(jsonString)!;
            var result = await salesRepository.AddsSalesAsync(Request.Headers["Id-Editor"]!, sales);
            sales = result.Item1;
            return sales != null ? CreatedAtAction(nameof(FindSales), new { diskonIds = string.Join(',', sales.Diskon.Select(x => x.Id)), promoIds = string.Join(',', sales.Promo.Select(x => x.Id)) }, JsonSerializer.Serialize(sales, _options)) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutSales([FromBody] string jsonString)
    {
        try
        {
            SalesDTO sales = JsonSerializer.Deserialize<SalesDTO>(jsonString)!;
            var result = await salesRepository.UpdatesSalesAsync(Request.Headers["Id-Editor"]!, sales.Diskon, sales.Promo);
            return result.Item1 == true ? NoContent() : result.Item1 == false ? NotFound(result.Item2) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeletesSales([FromQuery] string diskonIds = null!, [FromQuery] string promoIds = null!)
    {
        try
        {
            List<string> markedDiskon = diskonIds?.Mid(diskonIds.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            List<string> markedPromo = promoIds?.Mid(promoIds.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.DeletesSalesAsync(Request.Headers["Id-Editor"]!, markedDiskon, markedPromo);

            if (result == true)
                return NoContent();
            else if (result == false)
                return NotFound("Terdapat diskon / promo yang tidak ditemukan");
            return BadRequest("Ada kesalahan saat menghapus data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    #endregion Sales

    #region Diskon

    [HttpGet("diskon")]
    public async Task<IActionResult> GetDiskon([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.GetDiskonAsync(includes);
            if (result != null)
                return Ok(JsonSerializer.Serialize(result, _options));
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpGet("diskon/{id}")]
    public async Task<IActionResult> FindDiskon(string id, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.FindDiskonAsync(id, includes);
            if (result.Item2 == true)
                return Ok(JsonSerializer.Serialize(result.Item1, _options));
            else if (result.Item2 == false)
                return NotFound("Diskon tidak ditemukan");
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    #endregion Diskon

    #region Promo

    [HttpGet("promo")]
    public async Task<IActionResult> GetPromo([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.GetPromoAsync(includes);
            if (result != null)
                return Ok(JsonSerializer.Serialize(result, _options));
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpGet("promo/{id}")]
    public async Task<IActionResult> FindPromo(string id, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await salesRepository.FindPromoAsync(id, includes);
            if (result.Item2 == true)
                return Ok(JsonSerializer.Serialize(result.Item1, _options));
            else if (result.Item2 == false)
                return NotFound("Promo tidak ditemukan");
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    #endregion Promo
}