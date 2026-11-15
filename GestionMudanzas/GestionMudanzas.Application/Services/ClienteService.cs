using GestionMudanzas.Application.Core;
using GestionMudanzas.Application.DTOs;
using GestionMudanzas.Domain.Entities;
using GestionMudanzas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Application.Services;

public class ClienteService
{
    private readonly ClienteRepository _repository;
    public ClienteService(ClienteRepository repository) => _repository = repository;

    public async Task<ServiceResult<List<Cliente>>> ListarAsync()
        => ServiceResult<List<Cliente>>.Ok(await _repository.GetAllAsync());

    public async Task<ServiceResult<Cliente>> ObtenerAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        return cliente == null
            ? ServiceResult<Cliente>.Error("El cliente no existe.")
            : ServiceResult<Cliente>.Ok(cliente);
    }

    public async Task<ServiceResult<Cliente>> CrearAsync(ClienteDTO datos)
    {
        var error = Validar(datos);
        if (error != null) return ServiceResult<Cliente>.Error(error);

        var correo = datos.CorreoElectronico.Trim().ToLowerInvariant();
        if (await _repository.ExisteCorreoAsync(correo))
            return ServiceResult<Cliente>.Error("Ya existe un cliente con ese correo electrónico.");

        var cliente = new Cliente {
            NombreCompleto = datos.NombreCompleto.Trim(),
            Telefono = datos.Telefono.Trim(),
            CorreoElectronico = correo,
            Direccion = datos.Direccion.Trim()
        };
        try { await _repository.AddAsync(cliente); }
        catch (DbUpdateException) { return ServiceResult<Cliente>.Error("No se pudo registrar el cliente. Verifique que el correo no esté duplicado."); }
        return ServiceResult<Cliente>.Ok(cliente, "Cliente registrado correctamente.");
    }

    public async Task<ServiceResult<Cliente>> ActualizarAsync(int id, ClienteDTO datos)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) return ServiceResult<Cliente>.Error("El cliente no existe.");
        var error = Validar(datos);
        if (error != null) return ServiceResult<Cliente>.Error(error);

        var correo = datos.CorreoElectronico.Trim().ToLowerInvariant();
        if (await _repository.ExisteCorreoAsync(correo, id))
            return ServiceResult<Cliente>.Error("Ese correo ya pertenece a otro cliente.");

        cliente.NombreCompleto = datos.NombreCompleto.Trim();
        cliente.Telefono = datos.Telefono.Trim();
        cliente.CorreoElectronico = correo;
        cliente.Direccion = datos.Direccion.Trim();
        try { await _repository.UpdateAsync(cliente); }
        catch (DbUpdateException) { return ServiceResult<Cliente>.Error("No se pudo actualizar el cliente."); }
        return ServiceResult<Cliente>.Ok(cliente, "Cliente actualizado correctamente.");
    }

    public async Task<ServiceResult<string>> EliminarAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null) return ServiceResult<string>.Error("El cliente no existe.");
        try { await _repository.DeleteAsync(cliente); }
        catch (DbUpdateException) { return ServiceResult<string>.Error("No se puede eliminar el cliente porque tiene mudanzas asociadas."); }
        return ServiceResult<string>.Ok("Cliente eliminado correctamente.");
    }

    private static string? Validar(ClienteDTO d)
    {
        if (string.IsNullOrWhiteSpace(d.NombreCompleto)) return "El nombre es obligatorio.";
        if (string.IsNullOrWhiteSpace(d.Telefono)) return "El teléfono es obligatorio.";
        if (string.IsNullOrWhiteSpace(d.CorreoElectronico)) return "El correo electrónico es obligatorio.";
        if (!d.CorreoElectronico.Contains('@') || !d.CorreoElectronico.Contains('.')) return "El correo electrónico no es válido.";
        if (string.IsNullOrWhiteSpace(d.Direccion)) return "La dirección es obligatoria.";
        return null;
    }
}
