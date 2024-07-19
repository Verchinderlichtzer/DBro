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

    [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] protected ISalesService SalesService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected MudForm? _form = new();

    protected SalesFormDTO _sales = new() { DiskonDTO = [], PromoDTO = [] };
    protected List<Menu> _menu = null!;

    protected bool _new;
    protected bool _isValidationRuleShow;

    protected override async Task OnInitializedAsync()
    {
        _new = DiskonIds.Count == 0 && PromoIds.Count == 0;

        var response = await SalesService.GetFormAsync(DiskonIds, PromoIds, [nameof(Menu)]);
        _menu = response.Item1.Menu;
        if (response.Item1.Diskon != null)
        {
            _sales.DiskonDTO = response.Item1.Diskon.ConvertAll(x => new DiskonFormDTO() { Diskon = x, DR = new(x.TanggalMulai, x.TanggalAkhir) });
            _sales.PromoDTO = response.Item1.Promo.ConvertAll(x => new PromoFormDTO() { Promo = x, DR = new(x.TanggalMulai, x.TanggalAkhir) });
            _sales.DiskonDTO.ForEach(x => x.Diskon.Nilai *= 100);
        }
        else if (response.Item1 == null)
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            MudDialog.Cancel();
            return;
        }

        if (_new)
        {
            for (int i = 0; i < JumlahDiskon; i++) AddDiskon();
            for (int i = 0; i < JumlahPromo; i++) AddPromo();
        }
    }

    protected async Task<IEnumerable<Menu>> SearchMenuAsync(string value, CancellationToken token)
    {
        value ??= string.Empty;
        return await Task.FromResult(_menu.Where(x => $"{x.Id} {x.Kategori} {x.Nama} {x.Harga}".Search(value)).OrderBy(x => x.Nama));
    }

    protected void AddDiskon()
    {
        _sales.DiskonDTO.Add(new() { Diskon = new() });
    }

    protected void AddPromo()
    {
        _sales.PromoDTO.Add(new() { Promo = new() });
    }

    protected async Task SaveAsync()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            _sales.DiskonDTO.ForEach(x =>
            {
                x.Diskon.Nilai /= 100;
                x.Diskon.TanggalMulai = x.DR.Start;
                x.Diskon.TanggalAkhir = x.DR.End;
            });
            _sales.PromoDTO.ForEach(x =>
            {
                x.Promo.TanggalMulai = x.DR.Start;
                x.Promo.TanggalAkhir = x.DR.End;
            });
            if (_new)
            {
                var response = await SalesService.AddsSalesAsync(new() { Diskon = _sales.DiskonDTO.ConvertAll(x => x.Diskon), Promo = _sales.PromoDTO.ConvertAll(x => x.Promo) });
                if (response.Item1 != null)
                {
                    var result = response.Item1;
                    MudDialog.Close(DialogResult.Ok(result));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
            else
            {
                var response = await SalesService.UpdatesSalesAsync(new() { Diskon = _sales.DiskonDTO.ConvertAll(x => x.Diskon), Promo = _sales.PromoDTO.ConvertAll(x => x.Promo) });
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

    public class SalesFormDTO
    {
        public List<DiskonFormDTO> DiskonDTO { get; set; } = null!;
        public List<PromoFormDTO> PromoDTO { get; set; } = null!;
    }

    public class DiskonFormDTO
    {
        public Diskon Diskon { get; set; } = null!;
        public DateRange DR { get; set; } = new(DateTime.Today, DateTime.Today.AddDays(7));
    }

    public class PromoFormDTO
    {
        public Promo Promo { get; set; } = null!;
        public DateRange DR { get; set; } = new(DateTime.Today, DateTime.Today.AddDays(7));
    }
}