using GestorClientes.Application.Dtos;

namespace GestorClientes.Application.Services;

public interface IClienteServicio
{
    Task<List<ClienteDto>> ObtenerClientesAsync(CancellationToken cancellationToken = default);
    Task<ClienteDto?> ObtenerClientePorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Respuesta<ClienteDto>> CrearClienteAsync(ClienteDto cliente, CancellationToken cancellationToken = default);
    Task<Respuesta<ClienteDto>> ActualizarClienteAsync(ClienteDto cliente, CancellationToken cancellationToken = default);
    Task<Respuesta<ClienteDto>> EliminarClienteAsync(int id, CancellationToken cancellationToken = default);
}
