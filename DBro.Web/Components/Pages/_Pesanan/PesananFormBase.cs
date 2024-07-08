using FluentValidation;
using MudBlazor;
using Color = MudBlazor.Color;
using DBro.Web.Services;
using MudBlazor.Charts;

namespace DBro.Web.Components.Pages._Pesanan;

public class PesananFormBase : ComponentBase
{
    [Parameter] public string Email { get; set; } = null!;

    [Parameter] public string IdEditor { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected IValidator<Pesanan> Validator { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected MudForm? _form = new();
    protected MudAutocomplete<Menu> autocompleteMenu = new();

    protected Pesanan _pesanan = new();
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
        _isNew = string.IsNullOrEmpty(Email);
        PesananService.IdEditor = Email;

        if (!_isNew)
        {
            var response = await PesananService.GetFormAsync(Email);
            if (response.Item1 != null)
            {
                _pesanan = response.Item1.Pesanan;
                _menu = response.Item1.Menu;
                //_varianMenu = response.Item1.VarianMenu;
                _diskon = response.Item1.Diskon;
                _promo = response.Item1.Promo;
            }
            else
            {
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                MudDialog.Cancel();
                return;
            }
        }
        _hasLoaded = true;
    }

    protected async Task<IEnumerable<Menu>> SearchMenuAsync(string value, CancellationToken token)
    {
        value ??= string.Empty;
        return await Task.FromResult(_menu.Where(x => $"{x.Id} {x.JenisMenu.GetDescription()} {x.Nama}".Search(value)).OrderBy(x => x.Nama));
    }

    //protected async Task<IEnumerable<VarianMenu>> SearchVarianMenuAsync(string value, CancellationToken token)
    //{
    //    value ??= string.Empty;
    //    return await Task.FromResult(_varianMenu.Where(x => x.IdMenu ==  $"{x.Id} {x.JenisMenu.GetDescription()} {x.Nama}".Search(value)).OrderBy(x => x.Nama));
    //}

    protected async Task PilihMenu(Menu e)
    {
        if (e is null) return;
        _pesanan.DetailPesanan.Add(new() { IdPesanan = _pesanan.Id, IdMenu = e.Id, Menu = e, Jumlah = 1, Harga = e.Harga });
        _menu.Remove(e);
        //Hitung();
        await autocompleteMenu.ResetAsync();
    }

    protected void DeleteDetail(DetailPesanan detailPesanan)
    {
        _menu.Add(detailPesanan.Menu);
        _pesanan.DetailPesanan!.Remove(detailPesanan);
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
                    MudDialog.Close(DialogResult.Ok(_pesanan));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
            else
            {
                var response = await PesananService.UpdateAsync(_pesanan);
                if (response.Item1)
                {
                    MudDialog.Close(DialogResult.Ok(_pesanan));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
        }
    }

    public async Task OnKeyPressAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter") await SaveAsync();
    }

    protected void Cancel() => MudDialog.Cancel();
}