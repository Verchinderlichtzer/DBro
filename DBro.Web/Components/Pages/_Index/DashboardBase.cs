using MudBlazor;

namespace DBro.Web.Components.Pages._Index;

[Authorize]
public class DashboardBase : ComponentBase
{
    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected bool _loaded;

    //protected override async Task OnInitializedAsync()
    //{
    //    Layout.BreadcrumbItems =
    //    [
    //        new("Dashboard", "/dashboard")
    //    ];
    //    Layout.Refresh();

    //    _loaded = true;

    //    //     var user = await ((AuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
    //    //     var roleClaim = user.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

    //    //     NavManager.NavigateTo(roleClaim != null && !string.IsNullOrEmpty(roleClaim.Value) ? "/dashboard-pengawas" : "/dashboard-peserta");
    //}

    protected override void OnInitialized()
    {
        Layout.BreadcrumbItems =
        [
            new("Dashboard", "/dashboard")
        ];
        Layout.Refresh();

        _loaded = true;
    }
}