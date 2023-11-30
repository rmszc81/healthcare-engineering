using FluentValidation;

namespace Healthcare.Engineering.Validator;

public class CustomerDeleteValidator : AbstractValidator<Healthcare.Engineering.DataObject.Data.CustomerDto>
{
    public CustomerDeleteValidator(ValidatorSupport support)
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("Id is required.")
            .MustAsync((id, _) => support.IdExists(id)).WithMessage("Id does not exists.");
    }
}