using FluentValidation;

namespace DBro.Web.Validators;

public class MenuValidator : AbstractValidator<Menu>
{
    public MenuValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Nama)
            .NotEmpty().WithMessage("Nama tidak boleh kosong");

        RuleFor(x => x.JenisMenu)
            .Must(x => x != JenisMenu.None).WithMessage("Jenis menu tidak boleh kosong");

        RuleFor(x => x.Harga)
            .NotEmpty().WithMessage("Harga tidak boleh kosong");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<Menu>.CreateWithOptions((Menu)model, x => x.IncludeProperties(propertyName)));
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}