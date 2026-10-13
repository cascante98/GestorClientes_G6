using AutoMapper;
using GestorClientes.Application.Dtos;
using GestorClientes.Domain;

namespace GestorClientes.Application.Mapping;

/// <summary>Perfil de AutoMapper: conversión entre entidades de dominio y DTOs.</summary>
public class MapeoClases : Profile
{
    public MapeoClases()
    {
        CreateMap<Telefono, TelefonoDto>().ReverseMap();
        CreateMap<Cliente, ClienteDto>().ReverseMap();
    }
}
