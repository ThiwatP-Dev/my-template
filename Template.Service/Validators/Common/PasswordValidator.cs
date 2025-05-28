using FluentValidation;

namespace Template.Service.Validators.Common;

public static class PasswordValidator
{
    /// <summary>
    /// Validates a password based on the following rules:
    /// 1. Minimum length of 8 characters
    /// 2. At least one uppercase letter (A–Z)
    /// 3. At least one lowercase letter (a–z)
    /// 4. At least one digit (0–9)
    /// 5. At least one special character (@, !, *, -, _)
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@!\*\-_]).{8,}$")
            .WithMessage("Invalid password");
    }
}