namespace GestionMudanzas.DTOs
{
    public class EmpleadoDTO
    {
        public string NombreCompleto { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string CorreoElectronico { get; set; } = "";
        public string Cargo { get; set; } = "";
        public string Estado { get; set; } = "Activo";
    }
}
