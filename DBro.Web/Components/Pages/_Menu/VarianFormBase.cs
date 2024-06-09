using DBro.Web.Services;
using FluentValidation;
using MudBlazor;
using DBro.Shared.Models;
using Color = MudBlazor.Color;
using Microsoft.AspNetCore.Components.Forms;

namespace DBro.Web.Components.Pages._Menu;

public class VarianFormBase : ComponentBase
{
    [Parameter] public string IdMenu { get; set; } = null!;

    [Parameter] public string IdEditor { get; set; } = null!;

    [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

    [Inject] protected IMenuService MenuService { get; set; } = null!;

    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected Menu _menu = new() { VarianMenu = [] };

    protected bool _isValidationRuleShow;

    protected override async Task OnInitializedAsync()
    {
        MenuService.IdEditor = IdEditor;

        var response = await MenuService.FindAsync(IdMenu, [nameof(VarianMenu)]);
        if (response.Item1 == null)
        {
            await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
            MudDialog.Cancel();
            return;
        }
        else
        {
            _menu = response.Item1;
            if (response.Item1.VarianMenu == null || response.Item1.VarianMenu.Count == 0)
                _menu.VarianMenu = [new() { IdMenu = _menu.Id }];
            StateHasChanged();
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

    protected void AddVarian()
    {
        _menu.VarianMenu.Add(new() { IdMenu = _menu.Id });
    }

    protected void RemoveVarian(VarianMenu e)
    {
        _menu.VarianMenu.Remove(e);
    }

    protected async Task SaveAsync()
    {
        var response = await MenuService.UpdatesVarianAsync(_menu.VarianMenu);
        if (response.Item1)
        {
            MudDialog.Close(DialogResult.Ok(_menu));
            return;
        }
        await DialogService.ShowMessageBox("Error", response.Item2, yesText: "Ok");
    }

    public async Task OnKeyPressAsync(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter") await SaveAsync();
    }

    protected void Cancel() => MudDialog.Cancel();
}