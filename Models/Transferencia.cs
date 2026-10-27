namespace ApiBancoDemo.Models;

public class Transferencia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int CuentaOrigenId { get; set; }
    public int CuentaDestinoId { get; set; }
    public decimal Monto { get; set; }
    public string Estado { get; set; } = "Completada";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}