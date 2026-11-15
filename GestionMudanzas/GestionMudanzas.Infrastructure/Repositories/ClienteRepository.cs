using GestionMudanzas.Domain.Entities;
using GestionMudanzas.Infrastructure.Context;
using GestionMudanzas.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Infrastructure.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>
    {
        public ClienteRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null)
        {
            var consulta = DbSet.Where(c => c.CorreoElectronico == correo);
            if (excluirId.HasValue)
                consulta = consulta.Where(c => c.Id != excluirId.Value);

            return await consulta.AnyAsync();
        }
    }
}
