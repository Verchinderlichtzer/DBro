using DBro.Shared.Models;
using MudBlazor;

namespace DBro.Web.Components.Pages._Pesanan;

[Authorize]
public class PesananListBase : ComponentBase
{
    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected Func<Pesanan, bool> FilterSearch => x => $"{x.Id} {x.Tanggal:dd/MM/yyyy}".Search(_searchTerms);

    protected MudMessageBox? _deleteConfirmation = new();

    protected List<Pesanan> _pesananList = null!;

    protected bool _loaded;
    protected string _searchTerms = string.Empty;
    protected string _deleteMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("Pesanan", "/pesanan")
        ];
        Layout.Refresh();

        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await PesananService.GetAsync([nameof(User)]);
        if (response.Item1 != null)
        {
            //_pesananList = response.Item1;
            _pesananList = response.Item1;
        }
        else
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }

    protected async Task DeleteAsync(Pesanan pesanan)
    {
        _deleteMessage = $"Hapus \"{pesanan.Id}\"?";
        bool? result = await _deleteConfirmation!.ShowAsync();
        if (result == false)
        {
            var response = await PesananService.DeleteAsync(pesanan.Id);
            if (response.Item1)
            {
                Snackbar.Add("Pesanan berhasil dihapus", Severity.Success);
                await LoadDataAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Item2))
                    await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                Snackbar.Add("Pesanan gagal dihapus", Severity.Error);
            }
        }
    }
}