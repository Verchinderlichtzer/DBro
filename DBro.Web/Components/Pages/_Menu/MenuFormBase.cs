using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Color = MudBlazor.Color;

namespace DBro.Web.Components.Pages._Menu;

public class MenuFormBase : ComponentBase
{
    [Parameter] public string Id { get; set; } = null!;

    [Parameter] public string IdEditor { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] protected IMenuService MenuService { get; set; } = null!;

    [Inject] protected IValidator<Menu> Validator { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected MudForm? _form = new();

    protected Menu _menu = new();

    protected bool _isNew;
    protected bool _isValidationRuleShow;
    protected bool _isKetentuanUploadShow;

    protected override async Task OnInitializedAsync()
    {
        _isNew = string.IsNullOrEmpty(Id);
        MenuService.IdEditor = IdEditor;

        if (!_isNew)
        {
            var response = await MenuService.FindAsync(Id);
            if (response.Item1 != null)
            {
                _menu = response.Item1;
            }
            else
            {
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
                MudDialog.Cancel();
                return;
            }
        }
    }

    protected async Task UploadFile(IBrowserFile file)
    {
        if (file.ContentType != "image/png" && file.ContentType != "image/jpg" && file.ContentType != "image/jpeg" && file.ContentType != "image/webp")
        {
            Snackbar.Add("Ekstensi file tidak diperbolehkan", MudBlazor.Severity.Error);
            return;
        }
        else if (file.Size > 1048576)
        {
            Snackbar.Add("Ukuran file terlalu besar", MudBlazor.Severity.Error);
            return;
        }
        await using MemoryStream fs = new();
        await file.OpenReadStream(1048576).CopyToAsync(fs);
        _menu.Gambar = fs.ToArray();
    }

    protected async Task SaveAsync()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            if (_isNew)
            {
                var response = await MenuService.AddAsync(_menu);
                if (response.Item1 != null)
                {
                    _menu = response.Item1;
                    MudDialog.Close(DialogResult.Ok(_menu));
                    return;
                }
                await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            }
            else
            {
                var response = await MenuService.UpdateAsync(_menu);
                if (response.Item1)
                {
                    MudDialog.Close(DialogResult.Ok(_menu));
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