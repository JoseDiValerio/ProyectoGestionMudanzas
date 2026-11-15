using GestionMudanzas.Domain.Entities;
using GestionMudanzas.Infrastructure.Context;
using GestionMudanzas.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Infrastructure.Repositories
{
    public class VehiculoRepository : BaseRepository<Vehiculo>
    {
        public VehiculoRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExisteMatriculaAsync(string matricula, int? excluirId = null)
        {
            var consulta = DbSet.Where(v => v.Matricula == matricula);
            if (excluirId.HasValue)
                consulta = consulta.Where(v => v.Id != excluirId.Value);

            return await consulta.AnyAsync();
        }

        public Task<List<Vehiculo>> GetPorEstadoAsync(string estado)
            => DbSet.Where(v => v.Estado == estado).ToListAsync();
    }
}
