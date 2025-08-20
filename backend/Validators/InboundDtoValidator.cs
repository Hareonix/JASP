using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators;

public class InboundDtoValidator : AbstractValidator<InboundDto>
{
    public InboundDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.JsonConfig).NotEmpty();
    }
}

