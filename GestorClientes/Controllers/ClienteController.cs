using GestorClientes.Application.Dtos;
using GestorClientes.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorClientes.Controllers
{
    /// <summary>
    /// Controla el flujo de las peticiones HTTP de clientes. No contiene lógica de negocio:
    /// delega todo en IClienteServicio.
    /// </summary>
    public class ClienteController : Controller
    {
        private readonly IClienteServicio _clienteServicio;

        public ClienteController(IClienteServicio clienteServicio)
        {
            _clienteServicio = clienteServicio;
        }

        /// <summary>Vista principal con el listado de clientes.</summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>Listado de clientes y teléfonos en formato JSON.</summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerClientes(CancellationToken cancellationToken)
        {
            var clientes = await _clienteServicio.ObtenerClientesAsync(cancellationToken);
            return Json(clientes);
        }

        /// <summary>Detalle de un cliente en formato JSON.</summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerClientePorId(int id, CancellationToken cancellationToken)
        {
            var cliente = await _clienteServicio.ObtenerClientePorIdAsync(id, cancellationToken);
            if (cliente is null)
            {
                return NotFound();
            }

            return Json(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteDto cliente, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = await _clienteServicio.CrearClienteAsync(cliente, cancellationToken);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarCliente([FromBody] ClienteDto cliente, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = await _clienteServicio.ActualizarClienteAsync(cliente, cancellationToken);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCliente(int id, CancellationToken cancellationToken)
        {
            var respuesta = await _clienteServicio.EliminarClienteAsync(id, cancellationToken);
            return Json(respuesta);
        }
    }
}
