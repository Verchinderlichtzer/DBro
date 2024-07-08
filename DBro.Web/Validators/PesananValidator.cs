using FluentValidation;

namespace DBro.Web.Validators;

public class PesananValidator : AbstractValidator<Pesanan>
{
    public PesananValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Bayar)
            .Must((x, y) => y > x.Total).WithMessage("Pembayaran harus dibayar penuh");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<Pesanan>.CreateWithOptions((Pesanan)model, x => x.IncludeProperties(propertyName)));
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}