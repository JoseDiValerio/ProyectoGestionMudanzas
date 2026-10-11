using GestionMudanzas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
    }
}
