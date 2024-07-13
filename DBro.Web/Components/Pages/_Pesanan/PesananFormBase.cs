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
    //protected List<VarianMenu> _varianMenu = [];
    protected List<Diskon> _diskon = [];
    protected List<Promo> _promo = [];

    protected List<MenuDidapatDTO> _menuDidapat = [];

    protected bool _hasLoaded;
    protected bool _isNew;
    protected bool _isValidationRuleShow;

    protected override async Task OnInitializedAsync()
    {
        _isNew = string.IsNullOrEmpty(Id);
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
            _pesanan = response.Item1.Pesanan ?? new() { DetailPesanan = [] };
            _menu = response.Item1.Menu;
            //_varianMenu = response.Item1.VarianMenu;
            _diskon = response.Item1.Diskon;
            _promo = response.Item1.Promo;
        }
        _hasLoaded = true;
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

    protected async Task PilihMenu(Menu e)
    {
        if (e is null) return;
        _pesanan.DetailPesanan.Add(new() { IdPesanan = _pesanan.Id, IdMenu = e.Id, Menu = e, Harga = e.Harga, Jumlah = 1, Diskon = _diskon.Find(x => x.IdMenu == e.Id)?.Nilai ?? 0 });
        _menu.Remove(e);
        Calculate();
        await autocompleteMenu.ResetAsync();
    }

    protected void DeleteDetail(DetailPesanan detailPesanan)
    {
        _menu.Add(detailPesanan.Menu);
        _pesanan.DetailPesanan!.Remove(detailPesanan);
        Calculate();
    }

    protected void Calculate()
    {
        _menuDidapat.Clear();
        var hasil = _promo.Where(x => x.TanggalMulai <= DateTime.Today && x.TanggalAkhir >= DateTime.Today).ToList();
        foreach (var item in _pesanan.DetailPesanan)
        {
            var menuDibeli = hasil.FirstOrDefault(x => x.IdMenuDibeli == item.IdMenu);
            if (menuDibeli != null)
            {
                _menuDidapat.Add(new() { Menu = menuDibeli.MenuDidapat, Jumlah = item.Jumlah / menuDibeli.JumlahDibeli * menuDibeli.JumlahDidapat });
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
            if (_isNew)
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