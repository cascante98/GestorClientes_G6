using System.Text.RegularExpressions;

namespace GestorClientes.Domain;

/// <summary>
/// Entidad de dominio que representa a un cliente del negocio.
/// Un cliente puede tener cero o muchos teléfonos.
/// </summary>
public class Cliente
{
    public int Id { get; set; }

    public string Cedula { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido1 { get; set; } = string.Empty;

    public string Apellido2 { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public virtual ICollection<Telefono> Telefonos { get; set; } = new List<Telefono>();

    /// <summary>
    /// Regla de negocio: los datos obligatorios del cliente deben estar completos.
    /// </summary>
    /// <returns>true si cédula, nombre y primer apellido tienen valor.</returns>
    public bool ValidarReglaNegocioDatosObligatorios()
    {
        return !string.IsNullOrWhiteSpace(Cedula)
            && !string.IsNullOrWhiteSpace(Nombre)
            && !string.IsNullOrWhiteSpace(Apellido1);
    }

    /// <summary>
    /// Regla de negocio: si se indica un correo, debe tener un formato válido.
    /// El correo es opcional (se guarda como cadena vacía cuando no se indica).
    /// </summary>
    /// <returns>true si el correo está vacío o tiene formato válido.</returns>
    public bool ValidarReglaNegocioCorreo()
    {
        if (string.IsNullOrWhiteSpace(Correo))
        {
            return true;
        }

        return Regex.IsMatch(Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// Regla de negocio: un cliente no puede tener números de teléfono repetidos.
    /// </summary>
    /// <returns>true si todos los números son distintos.</returns>
    public bool ValidarReglaNegocioTelefonosNoRepetidos()
    {
        var numeros = Telefonos.Select(t => t.Numero.Trim()).ToList();
        return numeros.Count == numeros.Distinct().Count();
    }

    /// <summary>
    /// Agrupa todas las reglas de negocio; si alguna falla, el cliente no es válido.
    /// </summary>
    public bool ValidarReglasDeNegocio()
    {
        return ValidarReglaNegocioDatosObligatorios()
            && ValidarReglaNegocioCorreo()
            && ValidarReglaNegocioTelefonosNoRepetidos();
    }
}
