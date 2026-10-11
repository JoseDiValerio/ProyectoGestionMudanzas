namespace GestionMudanzas.DTOs
{
    public class VehiculoDTO
    {
        public string Matricula { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int CapacidadCarga { get; set; }
        public string Estado { get; set; } = "Disponible";

    }
}
