namespace GestionMudanzas.DTOs
{
    public class MudanzaDTO
    {
        public int ClienteId { get; set; }

        public int VehiculoId { get; set; }

        public DateTime FechaServicio { get; set; }

        public string DireccionOrigen { get; set; } = "";

        public string DireccionDestino { get; set; } = "";

        public decimal Costo { get; set; }
    }
}
