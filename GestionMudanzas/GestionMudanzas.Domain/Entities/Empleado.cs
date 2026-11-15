using GestionMudanzas.Domain.Core;

namespace GestionMudanzas.Domain.Entities
{
    public class Empleado : BaseEntity
    {
        public string NombreCompleto { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string CorreoElectronico { get; set; } = "";
        public string Cargo { get; set; } = "";
        public string Estado { get; set; } = "Activo";
        public ICollection<MudanzaEmpleado> MudanzasEmpleados { get; set; } = new List<MudanzaEmpleado>();
    }
}
