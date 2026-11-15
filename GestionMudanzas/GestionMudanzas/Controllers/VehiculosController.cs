using GestionMudanzas.Domain.Entities;
using GestionMudanzas.DTOs;
using GestionMudanzas.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly VehiculoRepository _repository;

        public VehiculosController(VehiculoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarVehiculos()
            => Ok(await _repository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarVehiculo(int id)
        {
            var vehiculo = await _repository.GetByIdAsync(id);
            if (vehiculo == null) return NotFound("El vehiculo no existe.");
            return Ok(vehiculo);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVehiculo(VehiculoDTO datos)
        {
            var error = Validar(datos);
            if (error != null) return BadRequest(error);

            string matricula = datos.Matricula.Trim().ToUpperInvariant();
            if (await _repository.ExisteMatriculaAsync(matricula))
                return BadRequest("Ya existe un vehiculo con esa matricula.");

            var vehiculo = new Vehiculo
            {
                Matricula = matricula,
                Marca = datos.Marca.Trim(),
                Modelo = datos.Modelo.Trim(),
                CapacidadCarga = datos.CapacidadCarga,
                Estado = datos.Estado
            };

            try
            {
                await _repository.AddAsync(vehiculo);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo registrar el vehiculo. Verifique que la matricula no esté duplicada.");
            }

            return Ok(vehiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarVehiculo(int id, VehiculoDTO datos)
        {
            var vehiculo = await _repository.GetByIdAsync(id);
            if (vehiculo == null) return NotFound("El vehiculo no existe.");

            var error = Validar(datos);
            if (error != null) return BadRequest(error);

            string matricula = datos.Matricula.Trim().ToUpperInvariant();
            if (await _repository.ExisteMatriculaAsync(matricula, id))
                return BadRequest("Esa matricula ya pertenece a otro vehiculo.");

            vehiculo.Matricula = matricula;
            vehiculo.Marca = datos.Marca.Trim();
            vehiculo.Modelo = datos.Modelo.Trim();
            vehiculo.CapacidadCarga = datos.CapacidadCarga;
            vehiculo.Estado = datos.Estado;

            try
            {
                await _repository.UpdateAsync(vehiculo);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo actualizar el vehiculo. Verifique que la matricula no esté duplicada.");
            }

            return Ok(vehiculo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarVehiculo(int id)
        {
            var vehiculo = await _repository.GetByIdAsync(id);
            if (vehiculo == null) return NotFound("El vehiculo no existe.");

            try
            {
                await _repository.DeleteAsync(vehiculo);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el vehiculo porque tiene mudanzas asociadas.");
            }

            return Ok("Vehiculo eliminado correctamente.");
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> ListarVehiculosDisponibles()
            => Ok(await _repository.GetPorEstadoAsync("Disponible"));

        [HttpGet("fuera-de-servicio")]
        public async Task<IActionResult> ListarVehiculosFueraDeServicio()
            => Ok(await _repository.GetPorEstadoAsync("Fuera de servicio"));

        private static string? Validar(VehiculoDTO datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Matricula)) return "La matricula es obligatoria.";
            if (string.IsNullOrWhiteSpace(datos.Marca)) return "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(datos.Modelo)) return "El modelo es obligatorio.";
            if (datos.CapacidadCarga <= 0) return "La capacidad de carga debe ser mayor que cero.";
            if (datos.Estado != "Disponible" && datos.Estado != "Fuera de servicio")
                return "El estado no es valido.";
            return null;
        }
    }
}
