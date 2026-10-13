namespace GestorClientes.Domain;

/// <summary>
/// Entidad de dominio que representa un número de teléfono ligado a un cliente.
/// </summary>
public class Telefono
{
    public int Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    /// <summary>Tipo de teléfono: Móvil, Casa, Trabajo, etc.</summary>
    public string Tipo { get; set; } = string.Empty;

    public int FkCliente { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    /// <summary>
    /// Regla de negocio: el número debe contener entre 8 y 15 dígitos
    /// (se permiten +, espacios y guiones como separadores).
    /// </summary>
    public bool ValidarReglaNegocioNumero()
    {
        var digitos = Numero.Count(char.IsDigit);
        var caracteresValidos = Numero.All(c => char.IsDigit(c) || c is '+' or ' ' or '-');
        return caracteresValidos && digitos is >= 8 and <= 15;
    }
}
