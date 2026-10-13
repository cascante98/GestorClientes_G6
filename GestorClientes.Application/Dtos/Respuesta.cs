namespace GestorClientes.Application.Dtos;

/// <summary>
/// Patrón Result: estandariza la respuesta de los servicios con un indicador de éxito,
/// un mensaje, un código de estado y el dato resultante.
/// </summary>
public class Respuesta<T>
{
    public bool EsCorrecto { get; set; } = true;

    public string Mensaje { get; set; } = "Operación realizada correctamente";

    public int Codigo { get; set; } = 200;

    public T? Dato { get; set; }
}
