using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators;

public class OutboundDtoValidator : AbstractValidator<OutboundDto>
{
    public OutboundDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.JsonConfig).NotEmpty();
    }
}

