using GestorClientes.Domain;
using GestorClientes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestorClientes.Infrastructure.Repository;

/// <summary>
/// Implementación con Entity Framework Core del repositorio de clientes.
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly ApplicationDbContext _context;

    public ClienteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene todos los clientes con sus teléfonos (solo lectura).</summary>
    public async Task<List<Cliente>> GetAllClientesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Telefonos)
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>Obtiene un cliente con sus teléfonos; null si no existe.</summary>
    public async Task<Cliente?> GetClienteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Telefonos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>Indica si ya existe otro cliente con la misma cédula.</summary>
    /// <param name="cedula">Cédula a buscar.</param>
    /// <param name="idExcluido">Id del cliente que se ignora (0 al crear).</param>
    public async Task<bool> ExisteCedulaAsync(string cedula, int idExcluido, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .AnyAsync(c => c.Cedula == cedula && c.Id != idExcluido, cancellationToken);
    }

    public async Task<bool> CreateClienteAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    /// <summary>
    /// Actualiza los datos del cliente y reemplaza su lista de teléfonos.
    /// </summary>
    public async Task<bool> UpdateClienteAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        var clienteExistente = await _context.Clientes
            .Include(c => c.Telefonos)
            .FirstOrDefaultAsync(c => c.Id == cliente.Id, cancellationToken);

        if (clienteExistente is null)
        {
            return false;
        }

        clienteExistente.Cedula = cliente.Cedula;
        clienteExistente.Nombre = cliente.Nombre;
        clienteExistente.Apellido1 = cliente.Apellido1;
        clienteExistente.Apellido2 = cliente.Apellido2;
        clienteExistente.Correo = cliente.Correo;
        clienteExistente.Direccion = cliente.Direccion;

        // Se reemplazan los teléfonos: se borran los anteriores y se agregan los nuevos.
        _context.Telefonos.RemoveRange(clienteExistente.Telefonos);
        clienteExistente.Telefonos = cliente.Telefonos
            .Select(t => new Telefono { Numero = t.Numero, Tipo = t.Tipo })
            .ToList();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>Borra el cliente y, en cascada, sus teléfonos.</summary>
    public async Task<bool> DeleteClienteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Telefonos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cliente is null)
        {
            return false;
        }

        _context.Telefonos.RemoveRange(cliente.Telefonos);
        _context.Clientes.Remove(cliente);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
