using MudBlazor;
using System.Security.Claims;

namespace DBro.Web.Components.Layout;

public partial class CustomerLayout
{
    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    public User CurrentUser { get; set; } = new();

    protected string relativePath = null!;

    readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = Colors.Pink.Accent4,
            Secondary = Colors.Cyan.Darken2,
            Tertiary = Colors.DeepPurple.Darken1,
            Warning = Colors.Orange.Darken1,
            AppbarBackground = Colors.DeepOrange.Default,
            AppbarText = Colors.Shades.White
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
        relativePath = NavManager.ToBaseRelativePath(NavManager.Uri);

        ClaimsPrincipal cp = (await AuthState).User;
        CurrentUser.Email = cp.FindFirstValue(ClaimTypes.Email)!;
        CurrentUser.Nama = cp.FindFirstValue(ClaimTypes.Name)!;
        CurrentUser.JenisUser = (cp.FindFirstValue(ClaimTypes.Role)!).ToEnum<JenisUser>();
        CurrentUser.JenisKelamin = (cp.FindFirstValue(ClaimTypes.Gender)!).ToEnum<JenisKelamin>();
    }

    public void Refresh()
    {
        relativePath = NavManager.ToBaseRelativePath(NavManager.Uri);
        StateHasChanged();
    }
}