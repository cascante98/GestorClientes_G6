using GestorClientes.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestorClientes.Infrastructure.Data;

/// <summary>
/// Contexto de Entity Framework Core (SQLite) para el gestor de clientes.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }

    public DbSet<Telefono> Telefonos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasIndex(e => e.Cedula).IsUnique();
            entity.Property(e => e.Cedula).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellido1).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellido2).HasMaxLength(100);
            entity.Property(e => e.Correo).HasMaxLength(150);
            entity.Property(e => e.Direccion).HasMaxLength(250);
        });

        modelBuilder.Entity<Telefono>(entity =>
        {
            entity.ToTable("Telefono");
            entity.HasIndex(e => e.FkCliente);
            entity.Property(e => e.Numero).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Tipo).HasMaxLength(30);

            // Borrar un cliente borra en cascada sus teléfonos.
            entity.HasOne(t => t.Cliente)
                .WithMany(c => c.Telefonos)
                .HasForeignKey(t => t.FkCliente)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
