using GestionMudanzas.Data;
using GestionMudanzas.DTOs;
using GestionMudanzas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MudanzasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MudanzasController(AppDbContext context)
        {
            _context = context;
        }

        // 1. LISTAR TODAS LAS MUDANZAS
        [HttpGet]
        public async Task<IActionResult> ListarMudanzas()
        {
            var mudanzas = await _context.Mudanzas
                .Include(m => m.Cliente)
                .Include(m => m.Vehiculo)
                .ToListAsync();

            return Ok(mudanzas);
        }

        // 2. CONSULTAR UNA MUDANZA POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarMudanza(int id)
        {
            var mudanza = await _context.Mudanzas
                .Include(m => m.Cliente)
                .Include(m => m.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mudanza == null)
            {
                return NotFound("La mudanza no existe.");
            }

            return Ok(mudanza);
        }

        // 3. REGISTRAR UNA MUDANZA
        [HttpPost]
        public async Task<IActionResult> RegistrarMudanza(
            MudanzaDTO datos)
        {
            if (datos.ClienteId <= 0)
            {
                return BadRequest("Debe seleccionar un cliente.");
            }

            if (datos.VehiculoId <= 0)
            {
                return BadRequest("Debe seleccionar un vehículo.");
            }

            if (datos.FechaServicio <= DateTime.Now)
            {
                return BadRequest(
                    "La fecha del servicio debe ser futura.");
            }

            if (string.IsNullOrWhiteSpace(datos.DireccionOrigen) ||
                string.IsNullOrWhiteSpace(datos.DireccionDestino))
            {
                return BadRequest(
                    "Las direcciones son obligatorias.");
            }

            if (datos.Costo < 0)
            {
                return BadRequest(
                    "El costo no puede ser negativo.");
            }

            // Comprobar que el cliente exista
            var cliente = await _context.Clientes
                .FindAsync(datos.ClienteId);

            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }

            // Comprobar que el vehículo exista
            var vehiculo = await _context.Vehiculos
                .FindAsync(datos.VehiculoId);

            if (vehiculo == null)
            {
                return BadRequest("El vehículo no existe.");
            }

            // Comprobar que el vehículo esté disponible
            if (vehiculo.Estado != "Disponible")
            {
                return BadRequest(
                    "El vehículo no está disponible.");
            }

            // Comprobar conflictos de fecha y hora
            DateTime inicio = datos.FechaServicio;
            DateTime fin = inicio.AddHours(4);

            bool ocupado = await _context.Mudanzas.AnyAsync(m =>
                m.VehiculoId == datos.VehiculoId &&
                m.Estado != "Cancelada" &&
                inicio < m.FechaServicio.AddHours(4) &&
                m.FechaServicio < fin);

            if (ocupado)
            {
                return BadRequest(
                    "El vehículo ya tiene una mudanza programada en ese horario.");
            }

            var mudanza = new Mudanza
            {
                ClienteId = datos.ClienteId,
                VehiculoId = datos.VehiculoId,
                FechaServicio = datos.FechaServicio,
                DireccionOrigen = datos.DireccionOrigen.Trim(),
                DireccionDestino = datos.DireccionDestino.Trim(),
                Costo = datos.Costo,
                Estado = "Pendiente"
            };

            _context.Mudanzas.Add(mudanza);

            await _context.SaveChangesAsync();

            return Ok(mudanza);
        }

        // 4. EDITAR UNA MUDANZA
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarMudanza(
            int id, MudanzaDTO datos)
        {
            var mudanza = await _context.Mudanzas
                .FindAsync(id);

            if (mudanza == null)
            {
                return NotFound("La mudanza no existe.");
            }

            if (mudanza.Estado == "Cancelada" ||
                mudanza.Estado == "Completada")
            {
                return BadRequest(
                    "No se puede modificar una mudanza cancelada o completada.");
            }

            if (datos.FechaServicio <= DateTime.Now)
            {
                return BadRequest(
                    "La fecha debe ser futura.");
            }

            if (datos.Costo < 0)
            {
                return BadRequest(
                    "El costo no puede ser negativo.");
            }

            if (datos.ClienteId <= 0 ||
                datos.VehiculoId <= 0)
            {
                return BadRequest(
                    "Debe seleccionar un cliente y un vehículo.");
            }

            if (string.IsNullOrWhiteSpace(datos.DireccionOrigen) ||
                string.IsNullOrWhiteSpace(datos.DireccionDestino))
            {
                return BadRequest(
                    "Las direcciones son obligatorias.");
            }

            var cliente = await _context.Clientes
                .FindAsync(datos.ClienteId);

            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }

            var vehiculo = await _context.Vehiculos
                .FindAsync(datos.VehiculoId);

            if (vehiculo == null)
            {
                return BadRequest("El vehículo no existe.");
            }

            if (vehiculo.Estado != "Disponible")
            {
                return BadRequest(
                    "El vehículo no está disponible.");
            }

            DateTime inicio = datos.FechaServicio;
            DateTime fin = inicio.AddHours(4);

            bool ocupado = await _context.Mudanzas.AnyAsync(m =>
                m.Id != id &&
                m.VehiculoId == datos.VehiculoId &&
                m.Estado != "Cancelada" &&
                inicio < m.FechaServicio.AddHours(4) &&
                m.FechaServicio < fin);

            if (ocupado)
            {
                return BadRequest(
                    "El vehículo ya tiene una mudanza en ese horario.");
            }

            mudanza.ClienteId = datos.ClienteId;
            mudanza.VehiculoId = datos.VehiculoId;
            mudanza.FechaServicio = datos.FechaServicio;
            mudanza.DireccionOrigen = datos.DireccionOrigen.Trim();
            mudanza.DireccionDestino = datos.DireccionDestino.Trim();
            mudanza.Costo = datos.Costo;

            await _context.SaveChangesAsync();

            return Ok(mudanza);
        }

        // 5. CAMBIAR ESTADO DE UNA MUDANZA
        [HttpPut("{id}/estado/{estado}")]
        public async Task<IActionResult> CambiarEstado(
            int id, string estado)
        {
            var mudanza = await _context.Mudanzas
                .FindAsync(id);

            if (mudanza == null)
            {
                return NotFound("La mudanza no existe.");
            }

            string[] estadosPermitidos =
            {
                "Pendiente",
                "Programada",
                "En proceso",
                "Completada"
            };

            string nuevoEstado = estadosPermitidos
                .FirstOrDefault(e =>
                    e.Equals(estado, StringComparison.OrdinalIgnoreCase))
                ?? "";

            if (nuevoEstado == "")
            {
                return BadRequest("El estado no es válido.");
            }

            if (mudanza.Estado == "Cancelada" ||
                mudanza.Estado == "Completada")
            {
                return BadRequest(
                    "La mudanza ya está finalizada.");
            }

            if (nuevoEstado == "Programada")
            {
                var vehiculo = await _context.Vehiculos
                    .FindAsync(mudanza.VehiculoId);

                if (vehiculo == null ||
                    vehiculo.Estado != "Disponible")
                {
                    return BadRequest(
                        "El vehículo no está disponible.");
                }
            }

            mudanza.Estado = nuevoEstado;

            await _context.SaveChangesAsync();

            return Ok(mudanza);
        }

        // 6. CANCELAR UNA MUDANZA
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarMudanza(int id)
        {
            var mudanza = await _context.Mudanzas
                .FindAsync(id);

            if (mudanza == null)
            {
                return NotFound("La mudanza no existe.");
            }

            if (mudanza.Estado == "Completada" ||
                mudanza.Estado == "Cancelada")
            {
                return BadRequest(
                    "La mudanza ya está finalizada.");
            }

            mudanza.Estado = "Cancelada";

            await _context.SaveChangesAsync();

            return Ok("Mudanza cancelada correctamente.");
        }
    }
}
