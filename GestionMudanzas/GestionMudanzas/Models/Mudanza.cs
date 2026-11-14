namespace GestionMudanzas.Models
{
    public class Mudanza
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        public DateTime FechaServicio { get; set; }
        public string DireccionOrigen { get; set; } = "";
        public string DireccionDestino { get; set; } = "";
        public decimal Costo { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}
