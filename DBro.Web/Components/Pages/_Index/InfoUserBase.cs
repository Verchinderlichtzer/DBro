using DBro.Web.Components.Pages._User;
using static DBro.Shared.Extensions.EncryptionHelper;
using MudBlazor;

namespace DBro.Web.Components.Pages._Index;

[Authorize]
public class InfoUserBase : ComponentBase
{
    [Parameter] public string Email { get; set; } = null!;

    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IUserService UserService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected NavigationManager NavManager { get; set; } = null!;

    protected MudForm? _form = new();
    protected User _user = null!;

    protected bool _loaded;
    protected string _searchTerms = string.Empty;
    protected string _deleteMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Layout.BreadcrumbItems =
        [
            new("User", "/user")
        ];
        Layout.Refresh();

        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await UserService.FindAsync(Email);
        response.Item1.Password = Decrypt(response.Item1.Password);
        if (response.Item1 != null)
            _user = response.Item1;
        else
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
    }

    protected async Task SaveAsync()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            var response = await UserService.UpdateAsync(_user);
            if (response.Item1)
            {
                Snackbar.Add("Info berhasil diubah", Severity.Success);
                NavManager.NavigateTo("/dashboard");
                return;
            }
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
        }
    }

    public async Task OnKeyPressAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter") await SaveAsync();
    }
}