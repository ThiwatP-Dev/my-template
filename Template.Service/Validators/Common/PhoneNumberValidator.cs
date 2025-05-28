using FluentValidation;

namespace Template.Service.Validators.Common;

public static class PhoneNumberValidator
{
    public static IRuleBuilderOptions<T, string> ValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches(@"^\+?\d+$")
            .WithMessage("Invalid contact");
    }
}