using GestionMudanzas.Domain.Entities;
using GestionMudanzas.DTOs;
using GestionMudanzas.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteRepository _repository;

        public ClientesController(ClienteRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarClientes()
            => Ok(await _repository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarCliente(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return NotFound("El cliente no existe.");
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCliente(ClienteDTO datos)
        {
            var error = Validar(datos);
            if (error != null) return BadRequest(error);

            var cliente = new Cliente
            {
                NombreCompleto = datos.NombreCompleto.Trim(),
                Telefono = datos.Telefono.Trim(),
                CorreoElectronico = datos.CorreoElectronico.Trim(),
                Direccion = datos.Direccion.Trim()
            };

            try
            {
                await _repository.AddAsync(cliente);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo registrar el cliente. Verifique que el correo no esté duplicado.");
            }

            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCliente(int id, ClienteDTO datos)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return NotFound("El cliente no existe.");

            var error = Validar(datos);
            if (error != null) return BadRequest(error);

            cliente.NombreCompleto = datos.NombreCompleto.Trim();
            cliente.Telefono = datos.Telefono.Trim();
            cliente.CorreoElectronico = datos.CorreoElectronico.Trim();
            cliente.Direccion = datos.Direccion.Trim();

            try
            {
                await _repository.UpdateAsync(cliente);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo actualizar el cliente. Verifique que el correo no esté duplicado.");
            }

            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return NotFound("El cliente no existe.");

            try
            {
                await _repository.DeleteAsync(cliente);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el cliente porque tiene servicios asociados.");
            }

            return Ok("Cliente eliminado correctamente.");
        }

        private static string? Validar(ClienteDTO datos)
        {
            if (string.IsNullOrWhiteSpace(datos.NombreCompleto)) return "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(datos.Telefono)) return "El teléfono es obligatorio.";
            if (string.IsNullOrWhiteSpace(datos.CorreoElectronico)) return "El correo electrónico es obligatorio.";
            if (!datos.CorreoElectronico.Contains("@") || !datos.CorreoElectronico.Contains("."))
                return "El correo electrónico no es válido.";
            if (string.IsNullOrWhiteSpace(datos.Direccion)) return "La dirección es obligatoria.";
            return null;
        }
    }
}
