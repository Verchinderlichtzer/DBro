using MudBlazor;

namespace DBro.Web.Components.Pages._Index;

[Authorize]
public class HomeBase : ComponentBase
{
    [CascadingParameter] public MainLayout Layout { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected bool _hasLoaded;

    //protected override async Task OnInitializedAsync()
    //{
    //    Layout.BreadcrumbItems =
    //    [
    //        new("Home", "/home")
    //    ];
    //    Layout.Refresh();

    //    _hasLoaded = true;

    //    //     var user = await ((AuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
    //    //     var roleClaim = user.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

    //    //     NavManager.NavigateTo(roleClaim != null && !string.IsNullOrEmpty(roleClaim.Value) ? "/home-pengawas" : "/home-peserta");
    //}

    protected override void OnInitialized()
    {
        Layout.BreadcrumbItems =
        [
            new("Home", "/home")
        ];
        Layout.Refresh();

        _hasLoaded = true;

        //     var user = await ((AuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
        //     var roleClaim = user.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        //     NavManager.NavigateTo(roleClaim != null && !string.IsNullOrEmpty(roleClaim.Value) ? "/home-pengawas" : "/home-peserta");
    }
}