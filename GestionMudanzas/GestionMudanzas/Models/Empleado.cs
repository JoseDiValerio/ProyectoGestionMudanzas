
using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Models
{
    [Index(nameof(CorreoElectronico), IsUnique = true)]
    public class Empleado
    {
        public int Id { get; set; }

        public string NombreCompleto { get; set; } = "";

        public string Telefono { get; set; } = "";

        public string CorreoElectronico { get; set; } = "";

        public string Cargo { get; set; } = "";

        public string Estado { get; set; } = "Activo";

        public ICollection<MudanzaEmpleado> MudanzasEmpleados { get; set; } = new List<MudanzaEmpleado>();
    }
}
