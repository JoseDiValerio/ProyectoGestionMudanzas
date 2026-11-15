using GestionMudanzas.Domain.Core;

namespace GestionMudanzas.Domain.Entities
{
    public class Mudanza : BaseEntity
    {
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        public DateTime FechaServicio { get; set; }
        public string DireccionOrigen { get; set; } = "";
        public string DireccionDestino { get; set; } = "";
        public decimal Costo { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public ICollection<MudanzaEmpleado> MudanzasEmpleados { get; set; } = new List<MudanzaEmpleado>();
    }
}
