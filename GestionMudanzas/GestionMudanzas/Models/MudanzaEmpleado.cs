namespace GestionMudanzas.Models
{
    public class MudanzaEmpleado
    {
        public int MudanzaId { get; set; }
        public Mudanza? Mudanza { get; set; }
        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }
    }
}
