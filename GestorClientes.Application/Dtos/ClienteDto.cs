using System.ComponentModel.DataAnnotations;

namespace GestorClientes.Application.Dtos;

public class ClienteDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(20)]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido1 { get; set; } = string.Empty;

    [StringLength(100)]
    public string Apellido2 { get; set; } = string.Empty;

    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [StringLength(250)]
    public string Direccion { get; set; } = string.Empty;

    public List<TelefonoDto> Telefonos { get; set; } = new();
}
