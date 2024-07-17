using MudBlazor;

namespace DBro.Web.Components.Pages._Index;

[Authorize]
public class HomeBase : ComponentBase
{
    [CascadingParameter] public CustomerLayout Layout { get; set; } = null!;

    [Inject] protected IMenuService MenuService { get; set; } = null!;

    [Inject] protected IPesananService PesananService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected Pesanan _keranjang = new();
    protected List<DisplayedMenu> _displayedMenu = null!;
    protected List<Menu> _allMenu = null!, _filteredMenu = null!;

    protected string _searchTerms = string.Empty;
    protected int _dataPerPage = 20;
    protected int _currentPage = 1;
    protected int _totalPage;
    protected bool _loaded;

    //protected Menu _menuTerpilih = new();
    protected int _jumlah = 1;
    //protected bool _menuCountVisible;

    protected override async Task OnInitializedAsync()
    {
        Layout.Refresh();
        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await MenuService.GetAsync();
        var result = await PesananService.CekKeranjangAsync(Layout.CurrentUser.Email);
        _keranjang = result.Item1;
        if (response.Item1 != null)
        {
            _allMenu = response.Item1.Where(x => !_keranjang.DetailPesanan.Select(y => y.IdMenu).Contains(x.Id)).ToList();
            _filteredMenu = _allMenu;
            ShowData();
        }
        else
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }

    protected void SearchData()
    {
        _filteredMenu = _allMenu.Where(x => $"{x.Id} {x.Nama} {x.Kategori.GetDescription()}".Search(_searchTerms)).ToList();
        ShowData();
    }

    protected void ShowData()
    {
        _displayedMenu = _filteredMenu.Skip((_currentPage - 1) * _dataPerPage).Take(_dataPerPage).ToList().ConvertAll(x => new DisplayedMenu { Menu = x });
    }
    protected void ClosePopups()
    {
        _displayedMenu.ForEach(x => x.Open = false);
    }

    protected async Task TambahKeKeranjangAsync(Menu menu)
    {
        await PesananService.TambahKeKeranjangAsync(new() { IdPesanan = _keranjang.Id, IdMenu = menu.Id, Jumlah = _jumlah, Harga = menu.Harga });
        await LoadDataAsync();
        Snackbar.Add("Menu ditambah ke keranjang", Severity.Success);
    }

    public class DisplayedMenu
    {
        public Menu Menu { get; set; } = null!;
        public bool Open { get; set; }
    }
}