using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Text;

namespace DBro.API.Controllers;

[ApiController, Route("api/[controller]")]
public class LaporanController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public LaporanController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
    {
        _appDbContext = appDbContext;
        _webHostEnvironment = webHostEnvironment;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<FileContentResult> Lapor<T>(string judul, IEnumerable<T> data, string keterangan = "")
    {
        await using Stream reportDefinition = new FileStream($"{_webHostEnvironment.WebRootPath}\\Laporan{judul}.rdlc", FileMode.Open);
        LocalReport report = new();
        report.LoadReportDefinition(reportDefinition);

        report.SetParameters(new ReportParameter("Keterangan", keterangan));

        if (judul == "DetailPesanan")
        {
            var pesanan = (List<Pesanan>)data;
            report.DataSources.Add(new ReportDataSource("DataSetTransaksiPesanan", pesanan.ConvertAll(x => new { x.Id, x.Tanggal, x.Subtotal, x.Potongan, x.Total, x.Bayar })));
            report.DataSources.Add(new ReportDataSource("DataSetDetailPesanan", pesanan[0].DetailPesanan.ConvertAll(x => new { Menu = x.Menu.Nama, x.Harga, x.Jumlah, x.Subtotal, x.Diskon, x.Total })));
            report.DataSources.Add(new ReportDataSource("DataSetMenuPromoPesanan", pesanan[0].MenuPromoPesanan.ConvertAll(x => new { Menu = x.Menu.Nama, x.Jumlah })));
        }
        else
        {
            report.DataSources.Add(new ReportDataSource($"DataSet{judul}", data));
        }

        byte[] pdf = report.Render("PDF");
        return File(pdf, "application/pdf", $"Laporan{judul}{DateTime.Now.Date:ddMMyy}.pdf");
    }

    [HttpGet("master-menu")]
    public async Task<ActionResult> LaporanMasterMenu()
    {
        var data = await _appDbContext.Menu.Select(x => new { x.Id, x.Kategori, x.Nama, x.Harga }).ToListAsync();
        return await Lapor("MasterMenu", data);
    }

    [HttpGet("master-diskon")]
    public async Task<ActionResult> LaporanMasterDiskon()
    {
        var data = await _appDbContext.Diskon.Include(x => x.Menu).Select(x => new { NamaMenu = x.Menu.Nama, Diskon = x.Nilai, Mulai = x.TanggalMulai, Akhir = x.TanggalAkhir }).ToListAsync();
        return await Lapor("MasterDiskon", data);
    }

    [HttpGet("master-promo")]
    public async Task<ActionResult> LaporanMasterPromo()
    {
        var data = await _appDbContext.Promo.Include(x => x.MenuDibeli).Include(x => x.MenuDidapat).Select(x => new { MenuDibeli = x.MenuDibeli.Nama, MenuDidapat = x.MenuDidapat.Nama, x.JumlahDibeli, x.JumlahDidapat, Mulai = x.TanggalMulai, Akhir = x.TanggalAkhir }).ToListAsync();
        return await Lapor("MasterPromo", data);
    }

    [HttpGet("transaksi-pesanan")]
    public async Task<ActionResult> LaporanTransaksiPesanan([FromQuery] string dari, [FromQuery] string sampai)
    {
        DateTime tglDari = ToDateTime(dari);
        DateTime tglSampai = ToDateTime(sampai);
        var data = (await _appDbContext.Pesanan.ToListAsync()).Where(x => x.Tanggal!.Value.Date >= tglDari.Date && x.Tanggal!.Value.Date <= tglSampai.Date).Select(x => new { x.Id, x.Tanggal, x.Subtotal, x.Potongan, x.Total, x.Bayar }).ToList();
        return await Lapor("TransaksiPesanan", data, $"Pesanan dari tanggal {tglDari.Date:dd/MM/yyyy} sampai {tglSampai.Date:dd/MM/yyyy}");
    }

    [HttpGet("detail-pesanan")]
    public async Task<ActionResult> LaporanDetailPesanan([FromQuery] string idPesanan)
    {
        var data = await _appDbContext.Pesanan
            .Include(x => x.DetailPesanan).ThenInclude(x => x.Menu)
            .Include(x => x.MenuPromoPesanan).ThenInclude(x => x.Menu)
            .Where(x => x.Id == idPesanan).ToListAsync();
        return await Lapor("DetailPesanan", data, $"Detail pesanan pada ID {idPesanan}");
    }

    [HttpGet("grafik")]
    public async Task<ActionResult> LaporanGrafik([FromQuery] string tahun)
    {
        var data = await _appDbContext.Pesanan.Where(x => x.Tanggal!.Value.Year == int.Parse(tahun)).Select(x => new { x.Id, x.Tanggal, Nominal = x.Total }).ToListAsync();
        return await Lapor("Grafik", data, $"GRAFIK TAHUN {tahun}");
    }
}
