using MudBlazor;

namespace DBro.Web.Components.Pages._Pesanan;

[Authorize]
public class PesananDetailBase : ComponentBase
{
    [Parameter] public string Id { get; set; } = string.Empty;

    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected Pesanan _pesanan = null!;

    protected bool _loaded;
    protected double _jumlah;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("Pesanan", "/pesanan"),
            new("Detail", $"/pesanan/detail/{Id}")
        ];
        Layout.Refresh();

        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await PesananService.FindAsync(Id, [nameof(Menu)]);
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