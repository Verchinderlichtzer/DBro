using FluentValidation;
using MudBlazor;
using Color = MudBlazor.Color;
using static DBro.Shared.Extensions.EncryptionHelper;
using DBro.Web.Services;

namespace DBro.Web.Components.Pages._User;

public class UserFormBase : ComponentBase
{
    [Parameter] public string Email { get; set; } = null!;

    [Parameter] public string IdEditor { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] protected IUserService UserService { get; set; } = null!;

    [Inject] protected IValidator<User> Validator { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected MudForm? _form = new();

    protected User _user = new();

    protected bool _isNew;
    protected bool _isValidationRuleShow;

    protected override async Task OnInitializedAsync()
    {
        _isNew = string.IsNullOrEmpty(Email);
        UserService.IdEditor = Email;

        if (!_isNew)
        {
            var response = await UserService.FindAsync(Email);
            if (response.Item1 != null)
            {
                _user = response.Item1;
            }
            else
            {
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                MudDialog.Cancel();
                return;
            }
        }
    }

    protected async Task SaveAsync()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            if (_isNew)
            {
                _user.JenisUser = JenisUser.Karyawan;
                _user.Password = Encrypt(_user.TanggalLahir!.Value.ToString("ddMMyy"));
                var response = await UserService.AddAsync(_user);
                if (response.Item1 != null)
                {
                    _user = response.Item1;
                    MudDialog.Close(DialogResult.Ok(_user));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
            else
            {
                var response = await UserService.UpdateAsync(_user);
                if (response.Item1)
                {
                    MudDialog.Close(DialogResult.Ok(_user));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
        }
    }

    public async Task OnKeyPressAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter") await SaveAsync();
    }

    protected void Cancel() => MudDialog.Cancel();
}