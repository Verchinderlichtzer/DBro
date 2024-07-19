using DBro.Shared.Models;
using MudBlazor;

namespace DBro.Web.Components.Pages._Laporan;

[Authorize]
public class LaporanBase : ComponentBase
{
    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected ILaporanService LaporanService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected Func<Pesanan, bool> FilterSearch => x => $"{x.Id} {x.Tanggal:dd/MM/yyyy}".Search(_searchTerms);

    protected string _searchTerms = string.Empty;
    protected bool _loaded;
    protected int _tahunan = DateTime.Today.Year;
    protected JenisLaporan _jenisLaporan;
    protected DateRange _rentangWaktu = null!;
    protected List<Pesanan> _pesanan = null!;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("Laporan", "/laporan")
        ];
        Layout.Refresh();

        int year = DateTime.Today.Year, month = DateTime.Today.Month;
        int last = DateTime.DaysInMonth(year, month);
        _rentangWaktu = new(new DateTime(year, month, 1), new DateTime(year, month, last));

        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await PesananService.GetAsync();

        if (response.Item1 == null)
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            NavManager.NavigateTo("/pesanan");
            return;
        }
        else
        {
            _pesanan = response.Item1;
        }
    }

    protected async Task ShowLaporanMasterAsync()
    {
        if (_jenisLaporan == JenisLaporan.MasterMenu)
        {
            await LaporanService.GetMasterAsync("master-menu");
        }
        else if (_jenisLaporan == JenisLaporan.MasterDiskon)
        {
            await LaporanService.GetMasterAsync("master-diskon");
        }
        else if (_jenisLaporan == JenisLaporan.MasterPromo)
        {
            await LaporanService.GetMasterAsync("master-promo");
        }
    }

    protected async Task ShowLaporanTransaksiPesananAsync()
    {
        await LaporanService.GetTransaksiPesananAsync(_rentangWaktu.Start!.Value.ToString("dd/MM/yyyy"), _rentangWaktu.End!.Value.ToString("dd/MM/yyyy"));
    }

    protected async Task ShowLaporanGrafikAsync()
    {
        await LaporanService.GetGrafikAsync(_tahunan.ToString());
    }

    protected async Task ShowLaporanDetailPesananAsync(string idPesanan)
    {
        await LaporanService.GetDetailPesananAsync(idPesanan);
    }

    public enum JenisLaporan
    {
        None,
        MasterMenu,
        MasterDiskon,
        MasterPromo,
        TransaksiPesanan,
        DetailPesanan,
        Grafik
    }
}