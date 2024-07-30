using DBro.Web.Components.Layout;
using MudBlazor;
using System.Globalization;

namespace DBro.Web.Components.Pages._Index;

[Authorize]
public class DashboardBase : ComponentBase
{
    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IMenuService MenuService { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected ISalesService SalesService { get; set; } = null!;

    [Inject] protected IUserService UserService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected MudSelect<int>? _inputInterval;
    protected List<ChartSeries> _series = [];
    protected List<Pesanan> _pesanan = null!;
    protected List<LineChartDTO> _seriesPendapatan = null!;
    protected string[] _xAxisLabels = null!;
    protected string _yAxisFormat = string.Empty;

    protected DateTime? _periodeDari = DateTime.Today;
    protected DateTime? _periodeSampai = DateTime.Today;

    protected bool _loaded;

    protected int _jumlahMenu;
    protected int _jumlahDiskon;
    protected int _jumlahPromo;
    protected int _jumlahPesanan;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("Dashboard", "/dashboard")
        ];
        Layout.Refresh();

        await LoadDataAsync();

        TerapkanGrafik();

        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        //_listPeminjaman = await UserService.GetAsync();
        _jumlahMenu = (await MenuService.GetAsync()).Item1.Count;
        _jumlahDiskon = (await SalesService.GetDiskonAsync()).Item1.Count(x => x.TanggalMulai <= DateTime.Today && x.TanggalAkhir >= DateTime.Today);
        _jumlahPromo = (await SalesService.GetPromoAsync()).Item1.Count(x => x.TanggalMulai <= DateTime.Today && x.TanggalAkhir >= DateTime.Today);
        _jumlahPesanan = (await PesananService.GetAsync()).Item1.Count;
        _pesanan = (await PesananService.GetAsync()).Item1;
    }

    protected void TerapkanGrafik()
    {
        _yAxisFormat = "#,##0,,M";
        _seriesPendapatan = _pesanan.ConvertAll(x => new LineChartDTO() { Tanggal = x.Tanggal!.Value, Nominal = x.Total });

        List<string> axis = [];
        List<LineChartDTO> offsetTanggal = [];

        for (int i = 11; i >= 0; i--)
        {
            axis.Add(DateTime.Now.AddMonths(-i).ToString("MMM yy"));
            offsetTanggal.Add(new() { Tanggal = DateTime.Now.AddMonths(-i).Date });
        }
        _seriesPendapatan.AddRange(offsetTanggal);
        _seriesPendapatan = _seriesPendapatan.Where(x => x.Tanggal.Date >= DateTime.Now.AddMonths(-11).Date && x.Tanggal <= DateTime.Now).GroupBy(x => x.Tanggal.Month).Select(x => new LineChartDTO { No = x.Key, Tanggal = x.Max(y => y.Tanggal), Nominal = x.Sum(y => y.Nominal) }).ToList();

        //if (_grafik == 0) // Harian
        //{
        //    for (int i = 9; i >= 0; i--)
        //    {
        //        axis.Add(DateTime.Now.AddDays(-i).ToString("dd/MM"));
        //        offsetTanggal.Add(new() { Tanggal = DateTime.Now.AddDays(-i).Date });
        //    }
        //    _seriesPendapatan.AddRange(offsetTanggal);
        //    _seriesPengeluaran.AddRange(offsetTanggal);
        //    _seriesPendapatan = _seriesPendapatan.Where(x => x.Tanggal.Date >= DateTime.Now.AddDays(-9).Date && x.Tanggal <= DateTime.Now).GroupBy(x => x.Tanggal.Date).Select(x => new LineChartDTO { Tanggal = x.Key, Nominal = x.Sum(y => y.Nominal) }).ToList();
        //    _seriesPengeluaran = _seriesPengeluaran.Where(x => x.Tanggal.Date >= DateTime.Now.AddDays(-9).Date && x.Tanggal <= DateTime.Now).GroupBy(x => x.Tanggal.Date).Select(x => new LineChartDTO { Tanggal = x.Key, Nominal = x.Sum(y => y.Nominal) }).ToList();
        //}
        //else if (_grafik == 2) // Bulanan
        //{
        //    for (int i = 1; i <= 12; i++)
        //    {
        //        axis.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i));
        //        offsetTanggal.Add(new() { Tanggal = new DateTime(DateTime.Now.Year, i, 1) });
        //    }
        //    _seriesPendapatan.AddRange(offsetTanggal);
        //    _seriesPengeluaran.AddRange(offsetTanggal);
        //    _seriesPendapatan = _seriesPendapatan.Where(x => x.Tanggal.Year == DateTime.Now.Year).GroupBy(x => x.Tanggal.Month).Select(x => new LineChartDTO { No = x.Key, Tanggal = x.Max(y => y.Tanggal), Nominal = x.Sum(y => y.Nominal) }).ToList();
        //    _seriesPengeluaran = _seriesPengeluaran.Where(x => x.Tanggal.Year == DateTime.Now.Year).GroupBy(x => x.Tanggal.Month).Select(x => new LineChartDTO { No = x.Key, Tanggal = x.Max(y => y.Tanggal), Nominal = x.Sum(y => y.Nominal) }).ToList();
        //}
        //else if (_grafik == 3) // Tahunan
        //{
        //    for (int i = 9; i >= 0; i--)
        //    {
        //        axis.Add((DateTime.Now.Year - i).ToString());
        //        offsetTanggal.Add(new() { Tanggal = new DateTime(DateTime.Now.Year - i, 1, 1) });
        //    }
        //    _seriesPendapatan.AddRange(offsetTanggal);
        //    _seriesPengeluaran.AddRange(offsetTanggal);
        //    _seriesPendapatan = _seriesPendapatan.Where(x => x.Tanggal.Year >= DateTime.Now.AddYears(-9).Year).GroupBy(x => x.Tanggal.Year).Select(x => new LineChartDTO { No = x.Key, Tanggal = x.Max(y => y.Tanggal), Nominal = x.Sum(y => y.Nominal) }).ToList();
        //    _seriesPengeluaran = _seriesPengeluaran.Where(x => x.Tanggal.Year >= DateTime.Now.AddYears(-9).Year).GroupBy(x => x.Tanggal.Year).Select(x => new LineChartDTO { No = x.Key, Tanggal = x.Max(y => y.Tanggal), Nominal = x.Sum(y => y.Nominal) }).ToList();
        //}
        _series.Clear();
        _series.Add(new() { Name = "Pendapatan", Data = _seriesPendapatan.OrderBy(x => x.Tanggal).Select(x => (double)x.Nominal).ToArray() });
        _xAxisLabels = [.. axis];
    }
}