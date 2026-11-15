using GestionMudanzas.Domain.Core;

namespace GestionMudanzas.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string NombreCompleto { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string CorreoElectronico { get; set; } = "";
        public string Direccion { get; set; } = "";
    }
}
