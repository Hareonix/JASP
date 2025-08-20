using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators;

public class ClientDtoValidator : AbstractValidator<ClientDto>
{
    public ClientDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleForEach(x => x.Outbounds).SetValidator(new ClientOutboundDtoValidator());
    }
}

