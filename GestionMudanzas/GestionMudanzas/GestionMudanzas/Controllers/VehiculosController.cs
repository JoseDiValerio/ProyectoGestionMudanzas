using GestionMudanzas.Application.DTOs;
using GestionMudanzas.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionMudanzas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculosController : ControllerBase
{
    private readonly VehiculoService _service;
    public VehiculosController(VehiculoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> ListarVehiculos()
        => Ok((await _service.ListarAsync()).Datos);

    [HttpGet("{id}")]
    public async Task<IActionResult> ConsultarVehiculo(int id)
    {
        var r = await _service.ObtenerAsync(id);
        return r.Exitoso ? Ok(r.Datos) : NotFound(r.Mensaje);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarVehiculo(VehiculoDTO datos)
    {
        var r = await _service.CrearAsync(datos);
        return r.Exitoso ? Ok(r.Datos) : BadRequest(r.Mensaje);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarVehiculo(int id, VehiculoDTO datos)
    {
        var r = await _service.ActualizarAsync(id, datos);
        if (r.Exitoso) return Ok(r.Datos);
        return r.Mensaje == "El vehículo no existe." ? NotFound(r.Mensaje) : BadRequest(r.Mensaje);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarVehiculo(int id)
    {
        var r = await _service.EliminarAsync(id);
        if (r.Exitoso) return Ok(r.Mensaje);
        return r.Mensaje == "El vehículo no existe." ? NotFound(r.Mensaje) : BadRequest(r.Mensaje);
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> ListarVehiculosDisponibles()
        => Ok((await _service.ListarPorEstadoAsync("Disponible")).Datos);

    [HttpGet("fuera-de-servicio")]
    public async Task<IActionResult> ListarVehiculosFueraDeServicio()
        => Ok((await _service.ListarPorEstadoAsync("Fuera de servicio")).Datos);
}
