using DBro.Shared.Models;
using MudBlazor;

namespace DBro.Web.Components.Pages._Keranjang;

[Authorize]
public class KeranjangFormBase : ComponentBase
{
    [CascadingParameter] public CustomerLayout Layout { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected ISalesService SalesService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected Pesanan _pesanan = null!;
    protected SalesDTO _sales = null!;

    protected bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        Layout.Refresh();
        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await PesananService.CekKeranjangAsync(Layout.CurrentUser.Email);
        var responseSales = await SalesService.GetSalesAsync([nameof(Menu)]);

        if (response.Item1 != null)
        {
            _pesanan = response.Item1;
            _pesanan.MenuPromoPesanan = [];
            _sales = responseSales.Item1;
            Calculate();
        }
        else
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }

    protected void Calculate()
    {
        _pesanan.MenuPromoPesanan.Clear();
        var hasil = _sales.Promo.Where(x => x.TanggalMulai <= DateTime.Today && x.TanggalAkhir >= DateTime.Today).ToList();
        foreach (var item in _pesanan.DetailPesanan)
        {
            var menuDibeli = hasil.FindLast(x => x.IdMenuDibeli == item.IdMenu);
            if (menuDibeli != null)
            {
                int jumlah = item.Jumlah / menuDibeli.JumlahDibeli * menuDibeli.JumlahDidapat;
                if (jumlah > 0) _pesanan.MenuPromoPesanan.Add(new() { Menu = menuDibeli.MenuDidapat, IdMenu = menuDibeli.IdMenuDidapat, Jumlah = jumlah });
            }
            item.Diskon = _sales.Diskon.Find(x => x.IdMenu == item.IdMenu)?.Nilai ?? 0;
        }
        _pesanan.Subtotal = _pesanan.DetailPesanan.Sum(x => x.Harga * x.Jumlah);
        _pesanan.Potongan = _pesanan.DetailPesanan.Sum(x => x.Potongan);
    }

    protected async Task DeleteAsync(DetailPesanan detail)
    {
        var response = await PesananService.DeleteDetailAsync(detail.IdPesanan, detail.IdMenu);
        if (response.Item1)
        {
            Snackbar.Add("Berhasil disimpan", Severity.Success);
            await LoadDataAsync();
        }
        else
        {
            if (!string.IsNullOrEmpty(response.Item2))
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            Snackbar.Add("Gagal disimpan", Severity.Error);
        }
    }

    protected async Task ModifyAsync(DetailPesanan detail)
    {
        (bool, string) response = await PesananService.UpdateDetailAsync(detail);

        if (response.Item1)
        {
            //Snackbar.Add("Berhasil disimpan", Severity.Success);
            await LoadDataAsync();
        }
        else
        {
            if (!string.IsNullOrEmpty(response.Item2))
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            //Snackbar.Add("Gagal disimpan", Severity.Error);
        }
    }

    protected async Task SaveAsync()
    {
        _pesanan.Status = StatusPesanan.MenungguPersetujuan;
        var response = await PesananService.UpdateAsync(_pesanan);
        if (response.Item1)
        {
            Snackbar.Add("Berhasil checkout", Severity.Success);
            NavManager.NavigateTo("/daftar-keranjang");
        }
        else
        {
            if (!string.IsNullOrEmpty(response.Item2))
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            Snackbar.Add("Gagal checkout", Severity.Error);
        }
    }
}