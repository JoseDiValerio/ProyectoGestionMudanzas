namespace GestionMudanzas.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string Matricula { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int CapacidadCarga { get; set; }
        public string Estado { get; set; } = "Disponible";

    }
}
