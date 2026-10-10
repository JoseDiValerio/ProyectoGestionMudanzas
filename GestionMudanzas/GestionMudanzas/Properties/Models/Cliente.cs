using System.Globalization;

namespace GestionMudanzas.Properties.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string CorreoElectronico { get; set; } = "";
        public string Direccion { get; set; } = "";
    }
}
