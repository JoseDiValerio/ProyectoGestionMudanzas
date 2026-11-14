using GestionMudanzas.Data;
using GestionMudanzas.DTOs;
using GestionMudanzas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }

        // 1. LISTAR TODOS LOS EMPLEADOS
        [HttpGet]
        public async Task<IActionResult> ListarEmpleados()
        {
            var empleados = await _context.Empleados
                .ToListAsync();

            return Ok(empleados);
        }

        // 2. CONSULTAR EMPLEADO POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarEmpleado(int id)
        {
            var empleado = await _context.Empleados
                .FindAsync(id);

            if (empleado == null)
            {
                return NotFound("El empleado no existe.");
            }

            return Ok(empleado);
        }

        // 3. REGISTRAR UN EMPLEADO
        [HttpPost]
        public async Task<IActionResult> RegistrarEmpleado(
            EmpleadoDTO datos)
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

            if (string.IsNullOrWhiteSpace(datos.Cargo))
            {
                return BadRequest("El cargo es obligatorio.");
            }

            if (datos.Estado != "Activo" &&
                datos.Estado != "Inactivo")
            {
                return BadRequest("El estado no es válido.");
            }

            string correo = datos.CorreoElectronico.Trim().ToLower();

            bool existe = await _context.Empleados
                .AnyAsync(e => e.CorreoElectronico == correo);

            if (existe)
            {
                return BadRequest(
                    "Ya existe un empleado con ese correo electrónico.");
            }

            var empleado = new Empleado
            {
                NombreCompleto = datos.NombreCompleto.Trim(),
                Telefono = datos.Telefono.Trim(),
                CorreoElectronico = correo,
                Cargo = datos.Cargo.Trim(),
                Estado = datos.Estado
            };

            _context.Empleados.Add(empleado);

            await _context.SaveChangesAsync();

            return Ok(empleado);
        }

        // 4. EDITAR EMPLEADO
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEmpleado(
            int id, EmpleadoDTO datos)
        {
            var empleado = await _context.Empleados
                .FindAsync(id);

            if (empleado == null)
            {
                return NotFound("El empleado no existe.");
            }

            if (string.IsNullOrWhiteSpace(datos.NombreCompleto) ||
                string.IsNullOrWhiteSpace(datos.Telefono) ||
                string.IsNullOrWhiteSpace(datos.CorreoElectronico) ||
                string.IsNullOrWhiteSpace(datos.Cargo))
            {
                return BadRequest(
                    "Todos los campos son obligatorios.");
            }

            if (!datos.CorreoElectronico.Contains("@") ||
                !datos.CorreoElectronico.Contains("."))
            {
                return BadRequest(
                    "El correo electrónico no es válido.");
            }

            if (datos.Estado != "Activo" &&
                datos.Estado != "Inactivo")
            {
                return BadRequest("El estado no es válido.");
            }

            string correo = datos.CorreoElectronico.Trim().ToLower();

            bool existe = await _context.Empleados
                .AnyAsync(e =>
                    e.CorreoElectronico == correo &&
                    e.Id != id);

            if (existe)
            {
                return BadRequest(
                    "Ese correo pertenece a otro empleado.");
            }

            empleado.NombreCompleto = datos.NombreCompleto.Trim();
            empleado.Telefono = datos.Telefono.Trim();
            empleado.CorreoElectronico = correo;
            empleado.Cargo = datos.Cargo.Trim();
            empleado.Estado = datos.Estado;

            await _context.SaveChangesAsync();

            return Ok(empleado);
        }

        // 5. ELIMINAR EMPLEADO
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEmpleado(int id)
        {
            var empleado = await _context.Empleados
                .FindAsync(id);

            if (empleado == null)
            {
                return NotFound("El empleado no existe.");
            }

            bool tieneAsignaciones = await _context.MudanzasEmpleados
                .AnyAsync(me => me.EmpleadoId == id);

            if (tieneAsignaciones)
            {
                return BadRequest(
                    "No se puede eliminar el empleado porque tiene asignaciones registradas.");
            }

            _context.Empleados.Remove(empleado);

            await _context.SaveChangesAsync();

            return Ok("Empleado eliminado correctamente.");
        }
    }
}
