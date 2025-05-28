using FluentValidation;
using Template.Service.Dto;
using Template.Service.Validators.Common;

namespace Template.Service.Validators;

public class ValidatedObjectDtoValidator : AbstractValidator<ValidatedObjectDto>
{
    public ValidatedObjectDtoValidator()
    {
        RuleFor(x => x.Email).ValidEmail();
        RuleFor(x => x.PhoneNumber).ValidPhoneNumber();
        RuleFor(x => x.Password).ValidPassword();
    }
}