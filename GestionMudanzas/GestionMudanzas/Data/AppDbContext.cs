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
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Mudanza> Mudanzas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<MudanzaEmpleado> MudanzasEmpleados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MudanzaEmpleado>()
                .HasKey(me => new { me.MudanzaId, me.EmpleadoId });
        }
    }
}
