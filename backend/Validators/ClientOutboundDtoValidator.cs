using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators;

public class ClientOutboundDtoValidator : AbstractValidator<ClientOutboundDto>
{
    public ClientOutboundDtoValidator()
    {
        RuleFor(x => x.OutboundId).GreaterThan(0);
    }
}

