using FluentValidation;

namespace DBro.Web.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Harus berupa email")
            .NotEmpty().WithMessage("Email tidak boleh kosong");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password tidak boleh kosong");

        RuleFor(x => x.Nama)
            .NotEmpty().WithMessage("Nama tidak boleh kosong");

        RuleFor(x => x.JenisKelamin)
            .Must(x => x != JenisKelamin.None).WithMessage("Jenis kelamin tidak boleh kosong");

        RuleFor(x => x.Alamat)
            .NotEmpty().WithMessage("Alamat tidak boleh kosong");

        RuleFor(x => x.Telepon)
            .NotEmpty().WithMessage("Telepon tidak boleh kosong");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<User>.CreateWithOptions((User)model, x => x.IncludeProperties(propertyName)));
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}