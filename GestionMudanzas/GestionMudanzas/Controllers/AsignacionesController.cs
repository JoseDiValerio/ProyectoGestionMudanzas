using GestionMudanzas.Infrastructure.Context;
using GestionMudanzas.DTOs;
using GestionMudanzas.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsignacionesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. ASIGNAR EMPLEADOS A UNA MUDANZA
        [HttpPost]
        public async Task<IActionResult> AsignarEmpleados(
            AsignarEmpleadoDTO datos)
        {
            if (datos.MudanzaId <= 0)
            {
                return BadRequest("Debe seleccionar una mudanza.");
            }

            if (datos.EmpleadosIds == null ||
                datos.EmpleadosIds.Count == 0)
            {
                return BadRequest(
                    "Debe seleccionar al menos un empleado.");
            }

            if (datos.EmpleadosIds.Any(id => id <= 0) ||
                datos.EmpleadosIds.Distinct().Count() !=
                datos.EmpleadosIds.Count)
            {
                return BadRequest(
                    "Los empleados seleccionados no son válidos o están repetidos.");
            }

            // Buscar la mudanza
            var mudanza = await _context.Mudanzas
                .FindAsync(datos.MudanzaId);

            if (mudanza == null)
            {
                return NotFound("La mudanza no existe.");
            }

            if (mudanza.Estado == "Cancelada" ||
                mudanza.Estado == "Completada")
            {
                return BadRequest(
                    "No se pueden asignar empleados a una mudanza finalizada.");
            }

            // Buscar los empleados seleccionados
            var empleados = await _context.Empleados
                .Where(e => datos.EmpleadosIds.Contains(e.Id))
                .ToListAsync();

            if (empleados.Count != datos.EmpleadosIds.Count)
            {
                return BadRequest(
                    "Uno o varios empleados no existen.");
            }

            // Comprobar que todos estén activos
            if (empleados.Any(e => e.Estado != "Activo"))
            {
                return BadRequest(
                    "No se pueden asignar empleados inactivos.");
            }

            // Comprobar si ya están asignados
            bool yaAsignados = await _context.MudanzasEmpleados
                .AnyAsync(me =>
                    me.MudanzaId == datos.MudanzaId &&
                    datos.EmpleadosIds.Contains(me.EmpleadoId));

            if (yaAsignados)
            {
                return BadRequest(
                    "Uno o varios empleados ya están asignados a esta mudanza.");
            }

            // Comprobar disponibilidad de cada empleado
            DateTime inicio = mudanza.FechaServicio;
            DateTime fin = inicio.AddHours(4);

            foreach (var empleado in empleados)
            {
                bool ocupado = await _context.MudanzasEmpleados
                    .AnyAsync(me =>
                        me.EmpleadoId == empleado.Id &&
                        me.MudanzaId != mudanza.Id &&
                        me.Mudanza!.Estado != "Cancelada" &&
                        me.Mudanza.Estado != "Completada" &&
                        inicio < me.Mudanza.FechaServicio.AddHours(4) &&
                        me.Mudanza.FechaServicio < fin);

                if (ocupado)
                {
                    return BadRequest(
                        "El empleado " + empleado.NombreCompleto +
                        " ya tiene otra mudanza asignada en ese horario.");
                }
            }

            // Registrar las asignaciones
            foreach (var empleado in empleados)
            {
                var asignacion = new MudanzaEmpleado
                {
                    MudanzaId = mudanza.Id,
                    EmpleadoId = empleado.Id
                };

                _context.MudanzasEmpleados.Add(asignacion);
            }

            await _context.SaveChangesAsync();

            return Ok(
                "Los empleados fueron asignados correctamente.");
        }

        // 2. CONSULTAR EMPLEADOS DE UNA MUDANZA
        [HttpGet("mudanza/{mudanzaId}")]
        public async Task<IActionResult> ConsultarEmpleadosMudanza(
            int mudanzaId)
        {
            var mudanza = await _context.Mudanzas
                .FindAsync(mudanzaId);

            if (mudanza == null)
            {
                return NotFound("La mudanza no existe.");
            }

            var empleados = await _context.MudanzasEmpleados
                .Where(me => me.MudanzaId == mudanzaId)
                .Include(me => me.Empleado)
                .Select(me => me.Empleado)
                .ToListAsync();

            return Ok(empleados);
        }

        // 3. CONSULTAR MUDANZAS DE UN EMPLEADO
        [HttpGet("empleado/{empleadoId}")]
        public async Task<IActionResult> ConsultarMudanzasEmpleado(
            int empleadoId)
        {
            var empleado = await _context.Empleados
                .FindAsync(empleadoId);

            if (empleado == null)
            {
                return NotFound("El empleado no existe.");
            }

            var mudanzas = await _context.MudanzasEmpleados
                .Where(me => me.EmpleadoId == empleadoId)
                .Include(me => me.Mudanza)
                    .ThenInclude(m => m!.Cliente)
                .Include(me => me.Mudanza)
                    .ThenInclude(m => m!.Vehiculo)
                .Select(me => me.Mudanza)
                .ToListAsync();

            return Ok(mudanzas);
        }

        // 4. ELIMINAR UNA ASIGNACIÓN
        [HttpDelete("{mudanzaId}/{empleadoId}")]
        public async Task<IActionResult> EliminarAsignacion(
            int mudanzaId, int empleadoId)
        {
            var asignacion = await _context.MudanzasEmpleados
                .FindAsync(mudanzaId, empleadoId);

            if (asignacion == null)
            {
                return NotFound(
                    "La asignación no existe.");
            }

            _context.MudanzasEmpleados.Remove(asignacion);

            await _context.SaveChangesAsync();

            return Ok(
                "La asignación fue eliminada correctamente.");
        }
    }
}
