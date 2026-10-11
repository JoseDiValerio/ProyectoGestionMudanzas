
using GestionMudanzas.Data;
using GestionMudanzas.DTOs;
using GestionMudanzas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehiculosController(AppDbContext context)
        {
            _context = context;
        }

        // 1. LISTAR TODOS LOS VEHICULOS
        [HttpGet]
        public async Task<IActionResult> ListarVehiculos()
        {
            var vehiculos = await _context.Vehiculos.ToListAsync();

            return Ok(vehiculos);
        }

        // 2. CONSULTAR UN VEHICULO POR SU ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo == null)
            {
                return NotFound("El vehiculo no existe.");
            }

            return Ok(vehiculo);
        }

        // 3. REGISTRAR UN VEHICULO
        [HttpPost]
        public async Task<IActionResult> RegistrarVehiculo(VehiculoDTO datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Matricula))
            {
                return BadRequest("La matricula es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(datos.Marca))
            {
                return BadRequest("La marca es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(datos.Modelo))
            {
                return BadRequest("El modelo es obligatorio.");
            }

            if (datos.CapacidadCarga <= 0)
            {
                return BadRequest("La capacidad de carga debe ser mayor que cero.");
            }

            if (datos.Estado != "Disponible" &&
                datos.Estado != "Fuera de servicio")
            {
                return BadRequest("El estado no es valido.");
            }

            string matricula = datos.Matricula.Trim().ToUpper();

            bool existe = await _context.Vehiculos
                .AnyAsync(v => v.Matricula == matricula);

            if (existe)
            {
                return BadRequest("Ya existe un vehiculo con esa matricula.");
            }

            var vehiculo = new Vehiculo();

            vehiculo.Matricula = matricula;
            vehiculo.Marca = datos.Marca.Trim();
            vehiculo.Modelo = datos.Modelo.Trim();
            vehiculo.CapacidadCarga = datos.CapacidadCarga;
            vehiculo.Estado = datos.Estado;

            _context.Vehiculos.Add(vehiculo);

            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        // 4. EDITAR UN VEHICULO
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarVehiculo(
            int id, VehiculoDTO datos)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo == null)
            {
                return NotFound("El vehiculo no existe.");
            }

            if (string.IsNullOrWhiteSpace(datos.Matricula))
            {
                return BadRequest("La matricula es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(datos.Marca))
            {
                return BadRequest("La marca es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(datos.Modelo))
            {
                return BadRequest("El modelo es obligatorio.");
            }

            if (datos.CapacidadCarga <= 0)
            {
                return BadRequest("La capacidad de carga debe ser mayor que cero.");
            }

            if (datos.Estado != "Disponible" &&
                datos.Estado != "Fuera de servicio")
            {
                return BadRequest("El estado no es valido.");
            }

            string matricula = datos.Matricula.Trim().ToUpper();

            bool existe = await _context.Vehiculos
                .AnyAsync(v => v.Matricula == matricula && v.Id != id);

            if (existe)
            {
                return BadRequest("Esa matricula ya pertenece a otro vehiculo.");
            }

            vehiculo.Matricula = matricula;
            vehiculo.Marca = datos.Marca.Trim();
            vehiculo.Modelo = datos.Modelo.Trim();
            vehiculo.CapacidadCarga = datos.CapacidadCarga;
            vehiculo.Estado = datos.Estado;

            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        // 5. ELIMINAR UN VEHICULO
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo == null)
            {
                return NotFound("El vehiculo no existe.");
            }

            _context.Vehiculos.Remove(vehiculo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(
                    "No se puede eliminar el vehiculo porque tiene mudanzas asociadas."
                );
            }

            return Ok("Vehiculo eliminado correctamente.");
        }

        // 6. LISTAR VEHICULOS DISPONIBLES
        [HttpGet("disponibles")]
        public async Task<IActionResult> ListarVehiculosDisponibles()
        {
            var vehiculos = await _context.Vehiculos
                .Where(v => v.Estado == "Disponible")
                .ToListAsync();

            return Ok(vehiculos);
        }

        // 6. LISTAR VEHICULOS FUERA DE SERVICIO
        [HttpGet("fuera-de-servicio")]
        public async Task<IActionResult> ListarVehiculosFueraDeServicio()
        {
            var vehiculos = await _context.Vehiculos
                .Where(v => v.Estado == "Fuera de servicio")
                .ToListAsync();

            return Ok(vehiculos);
        }
    }
}
