using FluentValidation;
using MudBlazor;
using Color = MudBlazor.Color;

namespace DBro.Web.Components.Pages._Sales;

public class SalesFormBase : ComponentBase
{
    [Parameter] public List<string> DiskonIds { get; set; } = null!;

    [Parameter] public List<string> PromoIds { get; set; } = null!;

    [Parameter] public int JumlahDiskon { get; set; }

    [Parameter] public int JumlahPromo { get; set; }

    [Parameter] public string IdEditor { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] protected ISalesService SalesService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected MudForm? _form = new();

    protected SalesDTO _sales = new() { Diskon = [], Promo = [] };
    protected List<Menu> _menu = [];

    protected bool _isNew;
    protected bool _isValidationRuleShow;

    protected override async Task OnInitializedAsync()
    {
        _isNew = DiskonIds.Count == 0 && PromoIds.Count == 0;
        SalesService.IdEditor = IdEditor;

        var response = await SalesService.SalesFormAsync(DiskonIds, PromoIds, [nameof(Menu)]);
        _menu = response.Item1.Menu;
        if (response.Item1.Sales != null)
        {
            _sales = response.Item1.Sales;
            _sales.Diskon.ForEach(x => x.Nilai *= 100);
        }
        else if (response.Item1 == null)
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            MudDialog.Cancel();
            return;
        }

        if (_isNew)
        {
            for (int i = 0; i < JumlahDiskon; i++) AddDiskon();
            for (int i = 0; i < JumlahPromo; i++) AddPromo();
        }
    }

    protected async Task<IEnumerable<Menu>> SearchMenuAsync(string value)
    {
        value ??= string.Empty;
        return await Task.FromResult(_menu.Where(x => $"{x.Id} {x.JenisMenu} {x.Nama} {x.Harga}".Search(value)).OrderBy(x => x.Nama));
    }

    protected async Task<IEnumerable<Diskon>> SearchDiskonAsync(string value)
    {
        value ??= string.Empty;
        return await Task.FromResult(_sales.Diskon.Where(x => x.IdMenu.Search(value) || x.Menu.Nama.Search(value) || x.Nilai.ToString("P0").Search(value) || x.TanggalMulai!.Value.ToString("dd/MM/yyyy MMMM").Search(value) || x.TanggalAkhir!.Value.ToString("dd/MM/yyyy MMMM").Search(value)).OrderBy(x => x.Menu.Nama));
    }

    protected async Task<IEnumerable<Promo>> SearchPromoAsync(string value)
    {
        value ??= string.Empty;
        return await Task.FromResult(_sales.Promo.Where(x => x.IdMenuDibeli.Search(value) || x.IdMenuDidapat.Search(value) || x.MenuDibeli.Nama.Search(value) || x.MenuDidapat.Nama.Search(value) || x.JumlahDibeli.ToString().Search(value) || x.JumlahDidapat.ToString().Search(value) || x.TanggalMulai!.Value.ToString("dd/MM/yyyy MMMM").Search(value) || x.TanggalAkhir!.Value.ToString("dd/MM/yyyy MMMM").Search(value)).OrderBy(x => x.MenuDibeli.Nama));
    }

    protected void AddDiskon()
    {
        _sales.Diskon.Add(new());
    }

    protected void AddPromo()
    {
        _sales.Promo.Add(new());
    }

    protected async Task SaveAsync()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            _sales.Diskon.ForEach(x => x.Nilai /= 100);
            if (_isNew)
            {
                var response = await SalesService.AddsSalesAsync(_sales);
                if (response.Item1 != null)
                {
                    _sales = response.Item1;
                    MudDialog.Close(DialogResult.Ok(_sales));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
            else
            {
                var response = await SalesService.UpdatesSalesAsync(_sales);
                if (response.Item1)
                {
                    MudDialog.Close(DialogResult.Ok(_sales));
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