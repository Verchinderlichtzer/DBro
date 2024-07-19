using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class MenuController(IMenuRepository menuRepository) : ControllerBase
{
    readonly JsonSerializerOptions _options = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

    #region Menu

    [HttpGet]
    public async Task<IActionResult> GetMenu([FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await menuRepository.GetAsync(includes);
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
    public async Task<IActionResult> FindMenu(string id, [FromQuery] string entities = null!)
    {
        try
        {
            List<string> includes = entities?.Mid(entities.IndexOf('=') + 1).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList()!;
            var result = await menuRepository.FindAsync(id, includes);
            if (result.Item2 == true)
                return Ok(JsonSerializer.Serialize(result.Item1, _options));
            else if (result.Item2 == false)
                return NotFound("Menu tidak ditemukan");
            return BadRequest("Ada kesalahan saat mengakses data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostMenu([FromBody] string jsonString)
    {
        try
        {
            Menu menu = JsonSerializer.Deserialize<Menu>(jsonString)!;
            var result = await menuRepository.AddAsync(menu);
            menu = result.Item1;
            return menu != null ? CreatedAtAction(nameof(FindMenu), new { id = menu.Id }, JsonSerializer.Serialize(menu, _options)) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutMenu([FromBody] string jsonString)
    {
        try
        {
            Menu menu = JsonSerializer.Deserialize<Menu>(jsonString)!;
            var result = await menuRepository.UpdateAsync(menu);
            return result.Item1 == true ? NoContent() : result.Item1 == false ? NotFound(result.Item2) : BadRequest(result.Item2);
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenu(string id)
    {
        try
        {
            var result = await menuRepository.DeleteAsync(id);

            if (result == 0)
                return NoContent();
            else if (result == 1)
                return NotFound("Menu tidak ditemukan");
            else if (result == 2)
                return Conflict("Menu pernah dibeli");
            return BadRequest("Ada kesalahan saat menghapus data");
        }
        catch (Exception)
        {
            return StatusCode(500, "Terjadi kesalahan pada server");
        }
    }

    #endregion Menu

    //#region Varian Menu

    //[HttpPut("varian")]
    //public async Task<IActionResult> PutVarian([FromBody] string jsonString)
    //{
    //    try
    //    {
    //        List<VarianMenu> varianMenu = JsonSerializer.Deserialize<List<VarianMenu>>(jsonString)!;
    //        var result = await menuRepository.UpdatesVarianAsync(varianMenu);
    //        return result == true ? NoContent() : result == false ? NotFound("Varian menu tidak ditemukan") : BadRequest("Ada kesalahan saat mengubah data");
    //    }
    //    catch (Exception)
    //    {
    //        return StatusCode(500, "Terjadi kesalahan pada server");
    //    }
    //}

    //#endregion Varian Menu
}
