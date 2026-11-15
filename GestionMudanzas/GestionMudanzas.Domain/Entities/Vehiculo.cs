using GestionMudanzas.Domain.Core;

namespace GestionMudanzas.Domain.Entities
{
    public class Vehiculo : BaseEntity
    {
        public string Matricula { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int CapacidadCarga { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}
