
using GestionMudanzas.Data;
using GestionMudanzas.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMudanzas.Models;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. LISTAR TODOS LOS CLIENTES
        [HttpGet]
        public async Task<IActionResult> ListarClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();

            return Ok(clientes);
        }

        // 2. CONSULTAR UN CLIENTE POR SU ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("El cliente no existe.");
            }

            return Ok(cliente);
        }

        // 3. REGISTRAR UN CLIENTE
        [HttpPost]
        public async Task<IActionResult> RegistrarCliente(ClienteDTO datos)
        {
            if (string.IsNullOrWhiteSpace(datos.NombreCompleto))
            {
                return BadRequest("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(datos.Telefono))
            {
                return BadRequest("El teléfono es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(datos.CorreoElectronico))
            {
                return BadRequest("El correo electrónico es obligatorio.");
            }

            if (!datos.CorreoElectronico.Contains("@") ||
                !datos.CorreoElectronico.Contains("."))
            {
                return BadRequest("El correo electrónico no es válido.");
            }

            if (string.IsNullOrWhiteSpace(datos.Direccion))
            {
                return BadRequest("La dirección es obligatoria.");
            }

            var cliente = new Cliente();

            cliente.NombreCompleto = datos.NombreCompleto;
            cliente.Telefono = datos.Telefono;
            cliente.CorreoElectronico = datos.CorreoElectronico;
            cliente.Direccion = datos.Direccion;

            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // 4. EDITAR UN CLIENTE
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCliente(
            int id, ClienteDTO datos)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("El cliente no existe.");
            }

            if (string.IsNullOrWhiteSpace(datos.NombreCompleto))
            {
                return BadRequest("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(datos.Telefono))
            {
                return BadRequest("El teléfono es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(datos.CorreoElectronico))
            {
                return BadRequest("El correo electrónico es obligatorio.");
            }

            if (!datos.CorreoElectronico.Contains("@") ||
                !datos.CorreoElectronico.Contains("."))
            {
                return BadRequest("El correo electrónico no es válido.");
            }

            if (string.IsNullOrWhiteSpace(datos.Direccion))
            {
                return BadRequest("La dirección es obligatoria.");
            }

            cliente.NombreCompleto = datos.NombreCompleto;
            cliente.Telefono = datos.Telefono;
            cliente.CorreoElectronico = datos.CorreoElectronico;
            cliente.Direccion = datos.Direccion;

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // 5. ELIMINAR UN CLIENTE
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("El cliente no existe.");
            }

            _context.Clientes.Remove(cliente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(
                    "No se puede eliminar el cliente porque tiene servicios asociados."
                );
            }

            return Ok("Cliente eliminado correctamente.");
        }
    }
}
