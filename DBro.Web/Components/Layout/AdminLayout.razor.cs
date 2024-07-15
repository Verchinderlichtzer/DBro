using MudBlazor;
using System.Security.Claims;

namespace DBro.Web.Components.Layout;

public partial class AdminLayout
{
    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = null!;

    [Inject] private NavigationManager NavManager { get; set; } = null!;

    public User CurrentUser { get; set; } = new();

    public List<BreadcrumbItem> BreadcrumbItems { get; set; } = [new(string.Empty, "/")];

    MudAutocomplete<KeyValuePair<string, string>>? _menuList = new();

    Dictionary<string, string> listMenu = new()
    {
        { "User", "/user" },
        { "Menu", "/menu" },
        { "Promo & Diskon", "/sales" },
        { "Pesanan", "/pesanan" },
        { "Aktivitas", "/aktivitas" }
    };

    bool _isDrawerOpen = true;
    readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = Colors.Pink.Accent4,
            Secondary = Colors.Cyan.Darken2,
            Tertiary = Colors.DeepPurple.Darken1,
            Warning = Colors.Orange.Darken1,
            AppbarBackground = Colors.Shades.White
        },
        PaletteDark = new PaletteDark()
        {
            Primary = Colors.Cyan.Accent3,
            Secondary = Colors.Blue.Accent2,
            Tertiary = Colors.Purple.Accent3,
            Warning = Colors.Orange.Darken1,
            DrawerIcon = Colors.Gray.Lighten2,
            DrawerText = Colors.Gray.Lighten2
        },
        Typography = new()
        {
            Default = new()
            {
                FontFamily = ["Roboto", "Helvetica", "Arial", "sans-serif"]
            }
        }
    };

    protected override async Task OnInitializedAsync()
    {
        ClaimsPrincipal cp = (await AuthState).User;
        CurrentUser.Email = cp.FindFirstValue(ClaimTypes.Email)!;
        CurrentUser.Nama = cp.FindFirstValue(ClaimTypes.Name)!;
        CurrentUser.JenisUser = (cp.FindFirstValue(ClaimTypes.Role)!).ToEnum<JenisUser>();
        CurrentUser.JenisKelamin = (cp.FindFirstValue(ClaimTypes.Gender)!).ToEnum<JenisKelamin>();
    }

    async Task<IEnumerable<KeyValuePair<string, string>>> CariMenu(string value, CancellationToken token)
    {
        value ??= string.Empty;
        return await Task.FromResult(listMenu.Where(x => x.Key.Contains(value, StringComparison.OrdinalIgnoreCase) || x.Value.Contains(value, StringComparison.OrdinalIgnoreCase)));
    }

    async Task PilihMenu(KeyValuePair<string, string> value)
    {
        await _menuList!.ResetAsync();
        if (!string.IsNullOrEmpty(value.Key)) NavManager.NavigateTo(value.Value);
    }

    public void Refresh() => StateHasChanged();
}
