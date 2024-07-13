using MudBlazor;

namespace DBro.Web.Components.Pages._Menu;

[Authorize]
public class MenuListBase : ComponentBase
{
    [CascadingParameter] public MainLayout Layout { get; set; } = null!;

    [Inject] protected IMenuService MenuService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    protected MudMessageBox? _deleteConfirmation = new();

    protected List<Menu> _menuList = null!;
    protected List<Menu> _filteredList = null!;
    protected List<Menu> _displayedList = null!;

    protected bool _hasLoaded;
    protected string _searchTerms = string.Empty;
    protected string _deleteMessage = string.Empty;
    protected int _dataPerPage = 20;
    protected int _currentPage = 1;
    protected int _totalPage;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("Menu", "/menu")
        ];
        Layout.Refresh();
        MenuService.IdEditor = Layout.CurrentUser.Email;

        await LoadDataAsync();
        _hasLoaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await MenuService.GetAsync();
        if (response.Item1 != null)
        {
            _menuList = response.Item1;
            _filteredList = _menuList;
            ShowData();
        }
        else
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }

    protected void SearchData()
    {
        _filteredList = _menuList.Where(x => $"{x.Id} {x.Nama} {x.Kategori.GetDescription()}".Search(_searchTerms)).ToList();
    }

    protected void ShowData()
    {
        _displayedList = _filteredList.Skip((_currentPage - 1) * _dataPerPage).Take(_dataPerPage).ToList();
    }

    protected async Task OpenFormAsync(Menu menu = null!)
    {
        bool isNew = menu == null;
        var parameters = new DialogParameters { ["Id"] = menu?.Id, ["IdEditor"] = MenuService.IdEditor };
        var form = await DialogService.Show<MenuForm>(isNew ? "Tambah Menu" : $"Edit \"{menu!.Nama}\"", parameters).Result;

        if (form!.Data is Menu model)
            Snackbar.Add(isNew ? "Menu berhasil ditambah" : "Menu berhasil diubah", Severity.Success);
        await LoadDataAsync();
    }

    //protected async Task OpenVarianAsync(Menu menu)
    //{
    //    var parameters = new DialogParameters { ["IdMenu"] = menu.Id, ["IdEditor"] = MenuService.IdEditor };
    //    var form = await DialogService.Show<VarianForm>($"Varian pada \"{menu.Nama}\"", parameters).Result;

    //    Menu model = (Menu)form.Data;

    //    if (model != null)
    //        Snackbar.Add("Varian berhasil diubah", Severity.Success);
    //    await LoadDataAsync();
    //}

    protected async Task DeleteAsync(Menu menu)
    {
        _deleteMessage = $"Hapus \"{menu.Nama}\"?";
        bool? result = await _deleteConfirmation!.ShowAsync();
        if (result == false)
        {
            var response = await MenuService.DeleteAsync(menu.Id);
            if (response.Item1)
            {
                Snackbar.Add("Menu berhasil dihapus", Severity.Success);
                await LoadDataAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Item2))
                    await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                Snackbar.Add("Menu gagal dihapus", Severity.Error);
            }
        }
    }
}