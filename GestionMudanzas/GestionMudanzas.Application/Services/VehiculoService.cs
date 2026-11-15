using GestionMudanzas.Application.Core;
using GestionMudanzas.Application.DTOs;
using GestionMudanzas.Domain.Entities;
using GestionMudanzas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Application.Services;

public class VehiculoService
{
    private readonly VehiculoRepository _repository;
    public VehiculoService(VehiculoRepository repository) => _repository = repository;

    public async Task<ServiceResult<List<Vehiculo>>> ListarAsync()
        => ServiceResult<List<Vehiculo>>.Ok(await _repository.GetAllAsync());

    public async Task<ServiceResult<Vehiculo>> ObtenerAsync(int id)
    {
        var vehiculo = await _repository.GetByIdAsync(id);
        return vehiculo == null ? ServiceResult<Vehiculo>.Error("El vehículo no existe.") : ServiceResult<Vehiculo>.Ok(vehiculo);
    }

    public async Task<ServiceResult<Vehiculo>> CrearAsync(VehiculoDTO d)
    {
        var error = Validar(d);
        if (error != null) return ServiceResult<Vehiculo>.Error(error);
        var matricula = d.Matricula.Trim().ToUpperInvariant();
        if (await _repository.ExisteMatriculaAsync(matricula)) return ServiceResult<Vehiculo>.Error("Ya existe un vehículo con esa matrícula.");
        var v = new Vehiculo { Matricula = matricula, Marca = d.Marca.Trim(), Modelo = d.Modelo.Trim(), CapacidadCarga = d.CapacidadCarga, Estado = d.Estado };
        try { await _repository.AddAsync(v); }
        catch (DbUpdateException) { return ServiceResult<Vehiculo>.Error("No se pudo registrar el vehículo. Verifique que la matrícula no esté duplicada."); }
        return ServiceResult<Vehiculo>.Ok(v, "Vehículo registrado correctamente.");
    }

    public async Task<ServiceResult<Vehiculo>> ActualizarAsync(int id, VehiculoDTO d)
    {
        var v = await _repository.GetByIdAsync(id);
        if (v == null) return ServiceResult<Vehiculo>.Error("El vehículo no existe.");
        var error = Validar(d);
        if (error != null) return ServiceResult<Vehiculo>.Error(error);
        var matricula = d.Matricula.Trim().ToUpperInvariant();
        if (await _repository.ExisteMatriculaAsync(matricula, id)) return ServiceResult<Vehiculo>.Error("Esa matrícula ya pertenece a otro vehículo.");
        v.Matricula = matricula; v.Marca = d.Marca.Trim(); v.Modelo = d.Modelo.Trim(); v.CapacidadCarga = d.CapacidadCarga; v.Estado = d.Estado;
        try { await _repository.UpdateAsync(v); }
        catch (DbUpdateException) { return ServiceResult<Vehiculo>.Error("No se pudo actualizar el vehículo."); }
        return ServiceResult<Vehiculo>.Ok(v, "Vehículo actualizado correctamente.");
    }

    public async Task<ServiceResult<string>> EliminarAsync(int id)
    {
        var v = await _repository.GetByIdAsync(id);
        if (v == null) return ServiceResult<string>.Error("El vehículo no existe.");
        try { await _repository.DeleteAsync(v); }
        catch (DbUpdateException) { return ServiceResult<string>.Error("No se puede eliminar el vehículo porque tiene mudanzas asociadas."); }
        return ServiceResult<string>.Ok("Vehículo eliminado correctamente.");
    }

    public async Task<ServiceResult<List<Vehiculo>>> ListarPorEstadoAsync(string estado)
        => ServiceResult<List<Vehiculo>>.Ok(await _repository.GetPorEstadoAsync(estado));

    private static string? Validar(VehiculoDTO d)
    {
        if (string.IsNullOrWhiteSpace(d.Matricula)) return "La matrícula es obligatoria.";
        if (string.IsNullOrWhiteSpace(d.Marca)) return "La marca es obligatoria.";
        if (string.IsNullOrWhiteSpace(d.Modelo)) return "El modelo es obligatorio.";
        if (d.CapacidadCarga <= 0) return "La capacidad de carga debe ser mayor que cero.";
        if (d.Estado != "Disponible" && d.Estado != "Fuera de servicio") return "El estado no es válido.";
        return null;
    }
}
