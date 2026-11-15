namespace GestionMudanzas.DTOs
{
    public class AsignarEmpleadoDTO
    {
        public int MudanzaId { get; set; }
        public List<int> EmpleadosIds { get; set; } = new List<int>();
    }
}
