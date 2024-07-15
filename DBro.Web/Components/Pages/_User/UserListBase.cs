using MudBlazor;

namespace DBro.Web.Components.Pages._User;

[Authorize]
public class UserListBase : ComponentBase
{
    [CascadingParameter] public AdminLayout Layout { get; set; } = null!;

    [Inject] protected IUserService UserService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    protected Func<User, bool> FilterSearch => x => $"{x.Email} {x.Nama} {x.JenisKelamin.GetDescription()} {x.TanggalLahir} {x.Alamat} {x.Telepon} {x.JenisUser}".Search(_searchTerms);

    protected MudMessageBox? _deleteConfirmation = new();

    protected List<User> _userList = null!;

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
        UserService.IdEditor = Layout.CurrentUser.Email;

        await LoadDataAsync();
        _loaded = true;
    }

    protected async Task LoadDataAsync()
    {
        var response = await UserService.GetAsync();
        if (response.Item1 != null)
            _userList = response.Item1;
        else
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
    }

    protected async Task OpenFormAsync(User user = null!)
    {
        bool isNew = user == null;
        var parameters = new DialogParameters { ["Email"] = user?.Email, ["IdEditor"] = UserService.IdEditor };
        var form = await DialogService.Show<UserForm>(isNew ? "Tambah User" : $"Edit \"{user!.Email}\"", parameters).Result;

        if (form!.Data is User model)
            Snackbar.Add(isNew ? "User berhasil ditambah" : "User berhasil diubah", Severity.Success);
        await LoadDataAsync();
    }

    protected async Task DeleteAsync(User user)
    {
        _deleteMessage = $"Hapus \"{user.Email}\"?";
        bool? result = await _deleteConfirmation!.ShowAsync();
        if (result == false)
        {
            var response = await UserService.DeleteAsync(user.Email);
            if (response.Item1)
            {
                Snackbar.Add("User berhasil dihapus", Severity.Success);
                await LoadDataAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Item2))
                    await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                Snackbar.Add("User gagal dihapus", Severity.Error);
            }
        }
    }
}