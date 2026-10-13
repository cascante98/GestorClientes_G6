using System.ComponentModel.DataAnnotations;

namespace GestorClientes.Application.Dtos;

public class TelefonoDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
    [StringLength(20)]
    public string Numero { get; set; } = string.Empty;

    [StringLength(30)]
    public string Tipo { get; set; } = string.Empty;
}
