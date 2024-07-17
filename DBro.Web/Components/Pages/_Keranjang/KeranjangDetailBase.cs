using MudBlazor;

namespace DBro.Web.Components.Pages._Keranjang;

[Authorize]
public class KeranjangDetailBase : ComponentBase
{
    [Parameter] public string IdPesanan { get; set; } = string.Empty;

    [CascadingParameter] public CustomerLayout Layout { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected Pesanan _pesanan = null!;

    protected bool _loaded;
    protected double _jumlah;

    protected override async Task OnInitializedAsync()
    {
        Layout.Refresh();
        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await PesananService.FindAsync(IdPesanan, [nameof(Menu)]);
        if (response.Item1 != null)
        {
            _pesanan = response.Item1;
        }
        else
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }
}