using AutoMapper;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Inbound, InboundDto>().ReverseMap();
        CreateMap<Outbound, OutboundDto>().ReverseMap();
    }
}

