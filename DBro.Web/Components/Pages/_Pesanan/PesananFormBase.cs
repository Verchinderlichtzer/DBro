using DBro.Shared.Models;
using FluentValidation;
using MudBlazor;

namespace DBro.Web.Components.Pages._Pesanan;

public class PesananFormBase : ComponentBase
{
    [Parameter] public string Id { get; set; } = string.Empty;

    [Parameter] public string IdEditor { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected IValidator<Pesanan> Validator { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected MudForm? _form = new();
    protected MudAutocomplete<Menu> autocompleteMenu = new();

    protected Pesanan _pesanan = null!;
    protected List<Menu> _menu = [];
    protected Menu? _menuTerpilih;
    //protected List<VarianMenu> _varianMenu = [];
    protected List<Diskon> _diskon = [];
    protected List<Promo> _promo = [];

    protected bool _loaded;
    protected bool _new;
    protected bool _isValidationRuleShow;

    protected override async Task OnInitializedAsync()
    {
        _new = string.IsNullOrEmpty(Id);
        PesananService.IdEditor = IdEditor;

        var response = await PesananService.GetFormAsync(Id);

        if (response.Item1 == null)
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            NavManager.NavigateTo("/pesanan");
            return;
        }
        else
        {
            _pesanan = response.Item1.Pesanan ?? new() { DetailPesanan = [], MenuPromoPesanan = [] };
            _menu = response.Item1.Menu;
            //_varianMenu = response.Item1.VarianMenu;
            _diskon = response.Item1.Diskon;
            _promo = response.Item1.Promo;
        }
        _loaded = true;
    }

    protected async Task<IEnumerable<Menu>> SearchMenuAsync(string value, CancellationToken token)
    {
        value ??= string.Empty;
        return await Task.FromResult(_menu.Where(x => $"{x.Id} {x.Kategori.GetDescription()} {x.Nama}".Search(value)).OrderBy(x => x.Nama));
    }

    //protected async Task<IEnumerable<VarianMenu>> SearchVarianMenuAsync(string value, CancellationToken token)
    //{
    //    value ??= string.Empty;
    //    return await Task.FromResult(_varianMenu.Where(x => x.IdMenu ==  $"{x.Id} {x.JenisMenu.GetDescription()} {x.Nama}".Search(value)).OrderBy(x => x.Nama));
    //}

    protected void PilihMenu(Menu? e)
    {
        if (e is null) return;
        _pesanan.DetailPesanan.Add(new() { IdPesanan = _pesanan.Id, IdMenu = e.Id, Menu = e, Harga = e.Harga, Jumlah = 1, Diskon = _diskon.FindLast(x => x.IdMenu == e.Id && x.TanggalMulai <= DateTime.Today && x.TanggalAkhir >= DateTime.Today)?.Nilai ?? 0 });
        _menu.Remove(e);
        Calculate();
        _menuTerpilih = null;
    }

    protected void DeleteDetail(DetailPesanan detailPesanan)
    {
        _menu.Add(detailPesanan.Menu);
        _pesanan.DetailPesanan!.Remove(detailPesanan);
        Calculate();
    }

    protected void Calculate()
    {
        _pesanan.MenuPromoPesanan.Clear();
        var hasil = _promo.Where(x => x.TanggalMulai <= DateTime.Today && x.TanggalAkhir >= DateTime.Today).ToList();
        foreach (var item in _pesanan.DetailPesanan)
        {
            var menuDibeli = hasil.FindLast(x => x.IdMenuDibeli == item.IdMenu);
            if (menuDibeli != null)
            {
                int jumlah = item.Jumlah / menuDibeli.JumlahDibeli * menuDibeli.JumlahDidapat;
                if(jumlah > 0) _pesanan.MenuPromoPesanan.Add(new() { Menu = menuDibeli.MenuDidapat, IdMenu = menuDibeli.IdMenuDidapat, Jumlah = jumlah });
            }
        }
        _pesanan.Subtotal = _pesanan.DetailPesanan.Sum(x => x.Harga * x.Jumlah);
        _pesanan.Potongan = _pesanan.DetailPesanan.Sum(x => x.Potongan);
    }

    protected async Task SaveAsync()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            if (_new)
            {
                var response = await PesananService.AddAsync(_pesanan);
                if (response.Item1 != null)
                {
                    _pesanan = response.Item1;
                    NavManager.NavigateTo("/pesanan");
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
            else
            {
                var response = await PesananService.UpdateAsync(_pesanan);
                if (response.Item1)
                {
                    NavManager.NavigateTo("/pesanan");
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
        }
    }

    public async Task OnKeyPressAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter") await SaveAsync();
    }
}