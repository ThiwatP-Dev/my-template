using FluentValidation;

namespace Template.Service.Validators.Common;

public static class EmailValidator
{
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .EmailAddress()
            .WithMessage("Invalid contact");
    }
}