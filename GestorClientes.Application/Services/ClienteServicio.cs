using AutoMapper;
using GestorClientes.Application.Dtos;
using GestorClientes.Domain;
using GestorClientes.Infrastructure.Repository;

namespace GestorClientes.Application.Services;

/// <summary>
/// Lógica de negocio de clientes. Depende de abstracciones (IClienteRepository, IMapper),
/// no de implementaciones (principio de Inversión de Dependencias).
/// </summary>
public class ClienteServicio : IClienteServicio
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMapper _mapper;

    public ClienteServicio(IClienteRepository clienteRepository, IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _mapper = mapper;
    }

    /// <summary>Listado de todos los clientes con sus teléfonos.</summary>
    public async Task<List<ClienteDto>> ObtenerClientesAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllClientesAsync(cancellationToken);
        return _mapper.Map<List<ClienteDto>>(clientes);
    }

    /// <summary>Detalle de un cliente; null si no existe.</summary>
    public async Task<ClienteDto?> ObtenerClientePorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetClienteByIdAsync(id, cancellationToken);
        return cliente is null ? null : _mapper.Map<ClienteDto>(cliente);
    }

    /// <summary>Registra un cliente nuevo validando las reglas de negocio.</summary>
    public async Task<Respuesta<ClienteDto>> CrearClienteAsync(ClienteDto cliente, CancellationToken cancellationToken = default)
    {
        var clienteEntity = _mapper.Map<Cliente>(cliente);
        clienteEntity.Id = 0;
        clienteEntity.FechaCreacion = DateTime.Now;

        var error = await ValidarAsync(clienteEntity, cancellationToken);
        if (error is not null)
        {
            return error;
        }

        if (!await _clienteRepository.CreateClienteAsync(clienteEntity, cancellationToken))
        {
            return Fallo("No se pudo crear el cliente.", 500);
        }

        return new Respuesta<ClienteDto>
        {
            Mensaje = "Cliente registrado correctamente.",
            Dato = _mapper.Map<ClienteDto>(clienteEntity)
        };
    }

    /// <summary>Modifica un cliente existente validando las reglas de negocio.</summary>
    public async Task<Respuesta<ClienteDto>> ActualizarClienteAsync(ClienteDto cliente, CancellationToken cancellationToken = default)
    {
        var clienteEntity = _mapper.Map<Cliente>(cliente);

        var error = await ValidarAsync(clienteEntity, cancellationToken);
        if (error is not null)
        {
            return error;
        }

        if (!await _clienteRepository.UpdateClienteAsync(clienteEntity, cancellationToken))
        {
            return Fallo("No se encontró el cliente a actualizar.", 404);
        }

        return new Respuesta<ClienteDto>
        {
            Mensaje = "Cliente actualizado correctamente.",
            Dato = _mapper.Map<ClienteDto>(clienteEntity)
        };
    }

    /// <summary>Elimina un cliente y sus teléfonos.</summary>
    public async Task<Respuesta<ClienteDto>> EliminarClienteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!await _clienteRepository.DeleteClienteAsync(id, cancellationToken))
        {
            return Fallo("No se encontró el cliente a eliminar.", 404);
        }

        return new Respuesta<ClienteDto> { Mensaje = "Cliente eliminado correctamente." };
    }

    /// <summary>
    /// Valida reglas de negocio del dominio y la cédula única.
    /// Devuelve una respuesta de error, o null si todo es válido.
    /// </summary>
    private async Task<Respuesta<ClienteDto>?> ValidarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        if (!cliente.ValidarReglaNegocioDatosObligatorios())
        {
            return Fallo("Cédula, nombre y primer apellido son obligatorios.", 400);
        }

        if (!cliente.ValidarReglaNegocioCorreo())
        {
            return Fallo("El correo electrónico no tiene un formato válido.", 400);
        }

        if (cliente.Telefonos.Any(t => !t.ValidarReglaNegocioNumero()))
        {
            return Fallo("Los teléfonos deben tener entre 8 y 15 dígitos.", 400);
        }

        if (!cliente.ValidarReglaNegocioTelefonosNoRepetidos())
        {
            return Fallo("El cliente tiene números de teléfono repetidos.", 400);
        }

        if (await _clienteRepository.ExisteCedulaAsync(cliente.Cedula, cliente.Id, cancellationToken))
        {
            return Fallo("Ya existe un cliente con esa cédula.", 409);
        }

        return null;
    }

    private static Respuesta<ClienteDto> Fallo(string mensaje, int codigo)
    {
        return new Respuesta<ClienteDto> { EsCorrecto = false, Mensaje = mensaje, Codigo = codigo };
    }
}
