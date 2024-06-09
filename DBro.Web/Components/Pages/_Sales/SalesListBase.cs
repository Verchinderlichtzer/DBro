using MudBlazor;

namespace DBro.Web.Components.Pages._Sales;

[Authorize]
public class SalesListBase : ComponentBase
{
    [CascadingParameter] public MainLayout Layout { get; set; } = null!;

    [Inject] protected ISalesService SalesService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    protected Func<Diskon, bool> FilterSearchDiskon => x => $"{x.Id} {x.Menu.Nama} {x.TanggalMulai} {x.TanggalAkhir}".Search(_searchDiskonTerms);

    protected Func<Promo, bool> FilterSearchPromo => x => $"{x.Id} {x.MenuDibeli.Nama} {x.MenuDidapat.Nama} {x.TanggalMulai} {x.TanggalAkhir}".Search(_searchPromoTerms);

    protected MudMessageBox? _deleteConfirmation = new();

    protected SalesDTO _sales = null!;

    protected HashSet<Diskon> _selectedDiskon = [];
    protected HashSet<Promo> _selectedPromo = [];

    protected int _jumlahDiskon;
    protected int _jumlahPromo;

    protected bool isNew = true;
    protected bool _hasLoaded;
    protected bool _isVisibleSales;
    protected string _searchDiskonTerms = string.Empty;
    protected string _searchPromoTerms = string.Empty;
    protected string _deleteMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("Sales", "/sales")
        ];
        Layout.Refresh();
        SalesService.IdEditor = Layout.CurrentUser.Email;

        await LoadDataAsync();
        _hasLoaded = true;
    }

    protected async Task LoadDataAsync()
    {
        _selectedDiskon.Clear();
        _selectedPromo.Clear();
        isNew = true;

        var response = await SalesService.GetSalesAsync([nameof(Menu)]);
        if (response.Item1 != null)
        {
            _sales = response.Item1;
        }
        else
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }

    protected string KeteranganPromo(Promo promo)
    {
        if (promo.MenuDibeli.Id == promo.MenuDidapat.Id)
        {
            return $"""{promo.MenuDibeli.Nama} (<span style="color: var(--mud-palette-success-darken)">Beli {promo.JumlahDibeli} Gratis {promo.JumlahDidapat}</span>)""";
        }
        else
        {
            string beli = promo.JumlahDibeli == 1 ? $"<span style=\"color: rgb(47,177,91)\">Beli</span> \"{promo.MenuDibeli.Nama}\"" : $"<span style=\"color: rgb(47,177,91)\">Beli {promo.JumlahDibeli}</span> \"{promo.MenuDibeli.Nama}\"";
            string gratis = promo.JumlahDidapat == 1 ? $"<span style=\"color: rgb(47,177,91)\">Gratis</span> \"{promo.MenuDidapat.Nama}\"" : $"<span style=\"color: rgb(47,177,91)\">Gratis {promo.JumlahDidapat}</span> \"{promo.MenuDidapat.Nama}\"";
            return string.Join(", ", beli, gratis);
        }
    }

    protected async Task ShowModalAsync()
    {
        if (_jumlahDiskon + _jumlahPromo == 0)
        {
            Snackbar.Add("Tidak ada data", Severity.Error);
            return;
        }
        if (_jumlahDiskon + _jumlahPromo > 100)
        {
            Snackbar.Add("Maksimal 100 data", Severity.Error);
            return;
        }
        _isVisibleSales = false;
        await Task.Delay(50);
        await OpenFormSalesAsync();
    }

    protected async Task OpenFormSalesAsync()
    {
        var parameters = new DialogParameters { ["DiskonIds"] = _selectedDiskon.Select(x => x.Id).ToList(), ["PromoIds"] = _selectedPromo.Select(x => x.Id).ToList(), ["JumlahDiskon"] = _jumlahDiskon, ["JumlahPromo"] = _jumlahPromo, ["IdEditor"] = SalesService.IdEditor };
        var form = await DialogService.Show<SalesForm>(isNew ? "Tambah Diskon & Promo" : "Edit Diskon & Promo", parameters).Result;
        if (form.Canceled)
        {
            _selectedDiskon.Clear();
            _selectedPromo.Clear();
            isNew = true;
        }
        else
        {
            SalesDTO model = (SalesDTO)form.Data;
            if (model != null)
            {
                Snackbar.Add(isNew ? "Data berhasil ditambah" : "Data berhasil diubah", Severity.Success);
            }
        }
        await LoadDataAsync();
    }

    protected async Task DeletesAsync()
    {
        List<string> jumlahTerpilih = [$"{_selectedDiskon.Count} diskon", $"{_selectedPromo.Count} promo"];
        _deleteMessage = $"Hapus {jumlahTerpilih.CombineWords()} terpilih?";
        bool? result = await _deleteConfirmation!.Show();
        if (result == false)
        {
            var response = await SalesService.DeletesSalesAsync(_selectedDiskon.Select(x => x.Id).ToList(), _selectedPromo.Select(x => x.Id).ToList());
            if (response.Item1)
            {
                Snackbar.Add("Sales berhasil dihapus", Severity.Success);
                await LoadDataAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Item2))
                    await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                Snackbar.Add("Sales gagal dihapus", Severity.Error);
            }
        }
    }
}