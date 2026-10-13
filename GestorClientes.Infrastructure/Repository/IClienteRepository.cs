using GestorClientes.Domain;

namespace GestorClientes.Infrastructure.Repository;

/// <summary>
/// Contrato de acceso a datos de clientes (CRUD). Las consultas devuelven datos
/// y las operaciones de escritura devuelven un bool indicando si tuvieron éxito.
/// </summary>
public interface IClienteRepository
{
    Task<List<Cliente>> GetAllClientesAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> GetClienteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteCedulaAsync(string cedula, int idExcluido, CancellationToken cancellationToken = default);
    Task<bool> CreateClienteAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<bool> UpdateClienteAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<bool> DeleteClienteAsync(int id, CancellationToken cancellationToken = default);
}
