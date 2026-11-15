namespace GestionMudanzas.Application.Core;

// Resultado sencillo que permite devolver datos o un mensaje de error desde un servicio.
public class ServiceResult<T>
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = "";
    public T? Datos { get; set; }

    public static ServiceResult<T> Ok(T datos, string mensaje = "Operación realizada correctamente.")
        => new() { Exitoso = true, Mensaje = mensaje, Datos = datos };

    public static ServiceResult<T> Error(string mensaje)
        => new() { Exitoso = false, Mensaje = mensaje };
}
