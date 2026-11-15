using GestionMudanzas.Application.DTOs;
using GestionMudanzas.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionMudanzas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _service;
    public ClientesController(ClienteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> ListarClientes()
    {
        var r = await _service.ListarAsync();
        return Ok(r.Datos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ConsultarCliente(int id)
    {
        var r = await _service.ObtenerAsync(id);
        return r.Exitoso ? Ok(r.Datos) : NotFound(r.Mensaje);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarCliente(ClienteDTO datos)
    {
        var r = await _service.CrearAsync(datos);
        return r.Exitoso ? Ok(r.Datos) : BadRequest(r.Mensaje);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditarCliente(int id, ClienteDTO datos)
    {
        var r = await _service.ActualizarAsync(id, datos);
        if (r.Exitoso) return Ok(r.Datos);
        if (r.Mensaje == "El cliente no existe.") return NotFound(r.Mensaje);
        return BadRequest(r.Mensaje);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarCliente(int id)
    {
        var r = await _service.EliminarAsync(id);
        if (r.Exitoso) return Ok(r.Mensaje);
        if (r.Mensaje == "El cliente no existe.") return NotFound(r.Mensaje);
        return BadRequest(r.Mensaje);
    }
}
