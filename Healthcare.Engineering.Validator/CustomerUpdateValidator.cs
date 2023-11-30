using FluentValidation;

namespace Healthcare.Engineering.Validator;

public class CustomerUpdateValidator : AbstractValidator<Healthcare.Engineering.DataObject.Data.CustomerDto>
{
    public CustomerUpdateValidator(ValidatorSupport support)
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("Id is required.")
            .MustAsync((id, _) => support.IdExists(id)).WithMessage("Id does not exists.");

        RuleFor(r => r.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(256).WithMessage("FirstName cannot be longer than 256 characters.");
        
        RuleFor(r => r.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(256).WithMessage("LastName cannot be longer than 256 characters.");

        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("Email cannot be longer than 256 characters.")
            .MustAsync(async (dto, _, _) => !await support.EmailExists(dto.Email!, dto.Id)).WithMessage("Email must be unique.");
        
        RuleFor(r => r.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .MaximumLength(256).WithMessage("PhoneNumber cannot be longer than 256 characters.");
    }
}