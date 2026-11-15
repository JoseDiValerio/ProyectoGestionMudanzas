using GestionMudanzas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
        public DbSet<Mudanza> Mudanzas => Set<Mudanza>();
        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<MudanzaEmpleado> MudanzasEmpleados => Set<MudanzaEmpleado>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MudanzaEmpleado>()
                .HasKey(me => new { me.MudanzaId, me.EmpleadoId });

            modelBuilder.Entity<Vehiculo>()
                .HasIndex(v => v.Matricula)
                .IsUnique();

            modelBuilder.Entity<Empleado>()
                .HasIndex(e => e.CorreoElectronico)
                .IsUnique();
        }
    }
}
